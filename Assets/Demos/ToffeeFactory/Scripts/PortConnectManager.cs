using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.UIElements;

namespace ToffeeFactory {
  [Serializable]
  public class ConnectionRef {
    public Port srcPort;
    public Port dstPort;
    public List<GameObject> pipeObjs = new List<GameObject>();
    public List<GameObject> midPortObjs = new List<GameObject>();
    private bool m_isAlive;

    public ConnectionRef() {
      m_isAlive = true;
    }

    public void Disconnect() {
      if (!m_isAlive) {
        return;
      }
      m_isAlive = false;
      foreach (var obj in pipeObjs) {
        GameObject.Destroy(obj);
      }
      foreach (var obj in midPortObjs) {
        GameObject.Destroy(obj);
      }
    }
  }

  public class PortConnectManager : MonoSingleton<PortConnectManager> {
    private struct Context {
      public bool isRunning;
      public Port srcPort;
      public GameObject lastSegmentSrc;
      private List<GameObject> createdMidPorts;
      private List<GameObject> createdPipes;
      private Vector3 m_cachedMidPortPos;

      private TimeFlag m_interactLock;
      private const float INTERACT_LOCK = 0.4f;

      public bool hasAddMidPortCache { get; private set; }

      public void Start(Port srcPort) {
        isRunning = true;
        createdMidPorts = new List<GameObject>();
        createdPipes = new List<GameObject>();
        this.srcPort = srcPort;
        lastSegmentSrc = srcPort.gameObject;
      }

      public void TryComplete(Port port) {
        if (m_interactLock.LessThan(INTERACT_LOCK)) {
          return;
        }
        m_interactLock.SetTrue();

        isRunning = false;
        srcPort.connectedPort = port;
        port.connectedPort = srcPort;

        var pipe = Instance.BuildPipe(lastSegmentSrc.transform.position, port.transform.position);
        pipe.transform.parent = lastSegmentSrc.transform;

        var res = new ConnectionRef() {
          srcPort = srcPort,
          dstPort = port,
          pipeObjs = createdPipes,
          midPortObjs = createdMidPorts,
        };

        srcPort.connectionRef = res;
        port.connectionRef = res;
      }

      public async void TryAddMidPort(Vector3 position) {
        if (m_interactLock.LessThan(INTERACT_LOCK)) {
          return;
        }
        m_interactLock.SetTrue();

        var src = lastSegmentSrc.transform.position;
        var end = position;

        var midPortObj = Instantiate(Instance.midPortPrefab);
        midPortObj.transform.position = end;
        createdMidPorts.Add(midPortObj);

        var pipe = Instance.BuildPipe(src, end);
        pipe.transform.parent = lastSegmentSrc.transform;
        createdPipes.Add(pipe);

        var buildCtx = new PipeCheckValidTaskCtx {
          pipe = pipe,
          midPort = midPortObj,
        };

        await CheckValid(buildCtx);

        if (buildCtx.isValid) {
          midPortObj.transform.parent = pipe.transform;
          Instance.m_ctx.lastSegmentSrc = midPortObj; //!m_ctx的方法被async方式调用时，m_ctx会被box
        } else {
          createdMidPorts.Remove(midPortObj);
          createdPipes.Remove(pipe);
          Destroy(midPortObj);
          Destroy(pipe);
        }
      }

      public void WriteAddMidPortCache(Vector3 position) {
        hasAddMidPortCache = true;
        m_cachedMidPortPos = position;
      }

      public Vector3 PopAddMidPortCache() {
        hasAddMidPortCache = false;
        return m_cachedMidPortPos;
      }

      public void Dispose() {
        isRunning = false;
        foreach (var obj in createdMidPorts.Union(createdPipes)) {
          Destroy(obj);
        }
      }

      private class PipeCheckValidTaskCtx {
        public GameObject pipe;
        public GameObject midPort;
        public bool isValid;
      }


      private async Task CheckValid(PipeCheckValidTaskCtx ctx) {
        await UniTask.DelayFrame(2, PlayerLoopTiming.FixedUpdate);

        var midPortCollision = ctx.midPort.GetComponentInChildren<CircuitCollision>();
        var pipeCollision = ctx.pipe.GetComponentInChildren<CircuitCollision>();

        if (midPortCollision.isLegal && pipeCollision.isLegal) {
          ctx.isValid = true;
        } else {
          ctx.isValid = false;
        }
      }
    }

    private GameObject m_portInConnecting => m_ctx.lastSegmentSrc;
    private bool isHandlingConnecting => m_ctx.isRunning;

    [SerializeField]
    private LineRenderer connectionPipe;

    [SerializeField]
    private LineRenderer previewPipe;

    [SerializeField]
    private LineRenderer forbiddenPipe;

    [SerializeField]
    private TMP_Text distText;

    [SerializeField]
    private float pipeMaxLength;

    private bool isPipeOverSized = false;

    [SerializeField]
    private LineRenderer rangeCircle;

    [SerializeField]
    private Color legalColor, illegalColor;

    [SerializeField]
    private GameObject midPortPrefab;

    private static readonly int Color1 = Shader.PropertyToID("_Color");

    private Context m_ctx;

    private TimeFlag m_writeAddMidPortFlag = new TimeFlag();
    private TimeFlag m_clickPortFlag = new TimeFlag();

    private bool checkConnectionLegal(Port a, Port b) {
      return a.type != b.type && !isPipeOverSized;
    }

    public void HandleConnect(Port port) {
      m_clickPortFlag.SetTrue();
      if (!isHandlingConnecting) { // if no port in connecting, then set port in connecting
        m_ctx = new Context();
        m_ctx.Start(port);
      } else {                                                // port in connecting, check if connection can set up
        if (checkConnectionLegal(m_ctx.srcPort, port)) { // if legal -> connect set up
          Port outPort = port.type == PortType.Out ? port : m_ctx.srcPort;

          m_ctx.TryComplete(port);
        }
      }
    }

    private GameObject BuildPipe(Vector3 src, Vector3 end) {
      LineRenderer pipe = Instantiate(connectionPipe);

      Vector3[] poses = BeautifyPath(src, end, 0.25f);
      pipe.positionCount = poses.Length;
      pipe.SetPositions(poses);
      var boxCol = pipe.gameObject.AddComponent<BoxCollider2D>();
      boxCol.isTrigger = true;
      var longIdx = boxCol.size[1] > boxCol.size[0] ? 1 : 0;
      var newSize = boxCol.size;
      newSize[longIdx] -= 3;
      boxCol.size = newSize;

      var absDelta = end - src;
      absDelta.x = Mathf.Abs(absDelta.x);
      absDelta.y = Mathf.Abs(absDelta.y);
      var isVer = absDelta.y > absDelta.x;
      pipe.GetComponent<CircuitCollision>().isVerWire = isVer;

      return pipe.gameObject;
    }

    private void BuildCirclePoses(int resolution) {

      List<Vector3> poses = new List<Vector3>();

      for (int i = 0; i <= resolution; i++) {
        float angle = Mathf.Deg2Rad * 360f * ((float)i / resolution);

        float x = Mathf.Cos(angle) * pipeMaxLength;
        float y = Mathf.Sin(angle) * pipeMaxLength;

        poses.Add(new Vector3(x, y, transform.position.z));
      }

      rangeCircle.positionCount = poses.Count;
      rangeCircle.SetPositions(poses.ToArray());
    }

    private Vector3[] BeautifyPath(Vector3 start, Vector3 end, float zipDist) {
      Vector3 frontNorm = new Vector3((end - start).x, 0, 0).normalized;

      Vector3 startZipPos = start + frontNorm * zipDist;
      Vector3 endZipPos = end - frontNorm * zipDist;

      return new[] {
        start,
        startZipPos,
        endZipPos,
        end
      };
    }

    private Vector3[] BeautifyPath(Vector3 start, Vector3 end, float zipDist, ref Vector3 centerPos) {
      Vector3 frontNorm = new Vector3((end - start).x, 0, 0).normalized;

      Vector3 startZipPos = start + frontNorm * zipDist;
      Vector3 endZipPos = end - frontNorm * zipDist;

      centerPos = (startZipPos + endZipPos) / 2f;

      return new[] {
        start,
        startZipPos,
        endZipPos,
        end
      };
    }

    private void Start() {
      BuildCirclePoses(100);
      rangeCircle.material.SetColor(Color1, legalColor);
      rangeCircle.gameObject.SetActive(false);

      previewPipe.gameObject.SetActive(false);
      forbiddenPipe.gameObject.SetActive(false);

      distText.gameObject.SetActive(false);

    }

    private void Update() {
      //至少0.1s前点了midport
      if (m_ctx.isRunning && m_ctx.hasAddMidPortCache && !m_writeAddMidPortFlag.LessThan(0.1f)) {
        var pos = m_ctx.PopAddMidPortCache();
        //至少0.2秒内没点过port
        if (!m_clickPortFlag.LessThan(0.2f)) {
          m_ctx.TryAddMidPort(pos);
        }
      }

      // press mouse right button
      if (Input.GetMouseButtonUp(1)) {
        if (m_ctx.isRunning) {
          m_ctx.Dispose();
          m_ctx = new();
        }
      }

      // draw pipe preview
      if (isHandlingConnecting) {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));
        worldPosition.Set(worldPosition.x, worldPosition.y, 0f);

        var toMouse = worldPosition - m_portInConnecting.transform.position;
        if (toMouse.x.Abs() > toMouse.y.Abs()) {
          toMouse.y = 0;
        } else {
          toMouse.x = 0;
        }
        worldPosition = m_portInConnecting.transform.position + toMouse;

        worldPosition = TFUtils.SnapToGrid(worldPosition);

        Vector3 centerPos = new Vector3();
        Vector3[] poses = BeautifyPath(m_portInConnecting.transform.position, worldPosition, 0.25f, ref centerPos);
        float distance = (m_portInConnecting.transform.position - worldPosition).magnitude;

        UpdateRangeCircle(centerPos, poses, distance);

        if (Input.GetMouseButtonUp(0)) {
          m_ctx.WriteAddMidPortCache(worldPosition);
          m_writeAddMidPortFlag.SetTrue();
        }

      } else {
        rangeCircle.gameObject.SetActive(false);
        previewPipe.gameObject.SetActive(false);
        forbiddenPipe.gameObject.SetActive(false);
        distText.gameObject.SetActive(false);
      }
    }

    private void UpdateRangeCircle(Vector3 centerPos, Vector3[] poses, float distance) {
      rangeCircle.transform.position = m_portInConnecting.transform.position;
      rangeCircle.gameObject.SetActive(true);

      if (distance <= pipeMaxLength) {
        rangeCircle.material.SetColor(Color1, legalColor);

        previewPipe.gameObject.SetActive(true);
        previewPipe.positionCount = poses.Length;
        previewPipe.SetPositions(poses);

        distText.gameObject.SetActive(true);
        distText.transform.position = centerPos;
        distText.text = "管道距离:" + $"{distance:N1}m";

        forbiddenPipe.gameObject.SetActive(false);

        isPipeOverSized = false;
      } else {
        rangeCircle.material.SetColor(Color1, illegalColor);

        forbiddenPipe.gameObject.SetActive(true);
        forbiddenPipe.positionCount = poses.Length;
        forbiddenPipe.SetPositions(poses);

        distText.gameObject.SetActive(true);
        distText.transform.position = centerPos;
        distText.text = "管道过长！！！" + $"({distance:N1}m)";

        previewPipe.gameObject.SetActive(false);

        isPipeOverSized = true;
      }
    }
  }
}