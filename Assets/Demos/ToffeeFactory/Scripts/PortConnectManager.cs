using System;
using TMPro;
using UnityEngine;

namespace ToffeeFactory {
  public class PortConnectManager : MonoSingleton<PortConnectManager> {

    private Port m_portInConnecting = null;
    private bool isHandlingConnecting => m_portInConnecting != null;

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
    private Transform pipeArea;
    
    private bool checkConnectionLegal(Port a, Port b) {
      return a.type != b.type && !isPipeOverSized;
    }
    
    public void HandleConnect(Port port) {
      if (!isHandlingConnecting) { // if no port in connecting, then set port in connecting
        m_portInConnecting = port; 
      } else {                                                // port in connecting, check if connection can set up
        if (checkConnectionLegal(m_portInConnecting, port)) { // if legal -> connect set up
          Port outPort = port.type == PortType.Out ? port : m_portInConnecting;
          
          m_portInConnecting.connectedPort = port;
          port.connectedPort = m_portInConnecting;
          m_portInConnecting = null;
          
          // instantiate connection line
          LineRenderer pipe = Instantiate(connectionPipe);
          
          var o = pipe.gameObject;
          port.connectPipe = o;
          port.connectedPort.connectPipe = o;
          
          Vector3[] poses = BeautifyPath(outPort.transform.position, outPort.connectedPort.transform.position, 0.25f);
          pipe.positionCount = poses.Length;
          pipe.SetPositions(poses);

        }
      }
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


      pipeArea.localScale = new Vector3(pipeMaxLength*2, pipeMaxLength*2, 1);
      pipeArea.gameObject.SetActive(false);
      
      previewPipe.gameObject.SetActive(false);
      forbiddenPipe.gameObject.SetActive(false);
      
      distText.gameObject.SetActive(false);
    }

    private void Update() {
      // press mouse right button
      if (Input.GetMouseButtonDown(1)) {
        if (m_portInConnecting) {
          // set port in connecting NULL
          m_portInConnecting = null;
        }
      }
      
      // draw pipe preview
      if (isHandlingConnecting) {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));
        worldPosition.Set(worldPosition.x, worldPosition.y, 0f);

        Vector3 centerPos = new Vector3();
        Vector3[] poses = BeautifyPath(m_portInConnecting.transform.position, worldPosition, 0.25f, ref centerPos);
        float distance = (m_portInConnecting.transform.position - worldPosition).magnitude;


        pipeArea.position = m_portInConnecting.transform.position;
        pipeArea.gameObject.SetActive(true);
        
        if (distance <= pipeMaxLength) {
          previewPipe.gameObject.SetActive(true);
          previewPipe.positionCount = poses.Length;
          previewPipe.SetPositions(poses);
        
          distText.gameObject.SetActive(true);
          distText.transform.position = centerPos;
          distText.text = "管道距离:" + $"{distance:N1}m";
          
          forbiddenPipe.gameObject.SetActive(false);

          isPipeOverSized = false;
        } 
        else {
          forbiddenPipe.gameObject.SetActive(true);
          forbiddenPipe.positionCount = poses.Length;
          forbiddenPipe.SetPositions(poses);
        
          distText.gameObject.SetActive(true);
          distText.transform.position = centerPos;
          distText.text = "管道过长！！！" + $"({distance:N1}m)";
          
          previewPipe.gameObject.SetActive(false);

          isPipeOverSized = true;
        }

      } else {
        pipeArea.gameObject.SetActive(false);
        previewPipe.gameObject.SetActive(false);
        forbiddenPipe.gameObject.SetActive(false);
        distText.gameObject.SetActive(false);
      }
      
      
    }
  }
}