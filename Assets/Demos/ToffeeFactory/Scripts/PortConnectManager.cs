using ToffeeFactory.ToffeeFactory;
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
          
          Vector3[] poses = 
          { outPort.transform.position, 
            outPort.connectedPort.transform.position };
          
          pipe.SetPositions(poses);

        }
      }
      
      
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
      
    }
  }
}