using System;
using UnityEngine;

namespace ToffeeFactory {
  public class PortConnectManager : MonoSingleton<PortConnectManager> {

    private Port m_portInConnecting = null;
    private bool isHandlingConnecting => m_portInConnecting != null;

    [SerializeField]
    private LineRenderer connectionPipe;

    [SerializeField]
    private LineRenderer previewPipe;
    
    private bool checkConnectionLegal(Port a, Port b) {
      return a.type != b.type;
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
    
    private void Start() {
      previewPipe.gameObject.SetActive(false);
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

        Vector3[] poses = BeautifyPath(m_portInConnecting.transform.position, worldPosition, 0.25f);

        previewPipe.gameObject.SetActive(true);
        previewPipe.positionCount = poses.Length;
        previewPipe.SetPositions(poses);
      } else {
        previewPipe.gameObject.SetActive(false);
      }
      
      
    }
  }
}