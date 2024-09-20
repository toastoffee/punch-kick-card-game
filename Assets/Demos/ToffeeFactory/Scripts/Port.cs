using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToffeeFactory {

  public enum PortType {
    In,
    Out
  }
  
  public class Port : MonoBehaviour {
    private PortType m_type;
    private Port m_connectedPort = null;
    private bool isConnected => m_connectedPort != null;
    public PortType type => m_type;
    public Port connectedPort { get => m_connectedPort; set => m_connectedPort = value; }
    
    public void Connect() {
      if (!isConnected) {
        // handle connection click event
        PortConnectManager.Instance.HandleConnect(this);
      }
    }
  }


  public class PortConnectManager : MonoSingleton<PortConnectManager> {

    private Port m_portInConnecting = null;
    private bool isHandlingConnecting => m_portInConnecting != null;

    [SerializeField]
    private LineRenderer connectionPipe;

    private bool checkConnectionLegal(Port a, Port b) {
      return a.type != b.type;
    }
    
    public void HandleConnect(Port port) {
      if (!isHandlingConnecting) { // if no port in connecting, then set port in connecting
        m_portInConnecting = port; 
      } else {  // port in connecting, check if connection can set up
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
    }
  }
}

