using UnityEngine;

namespace ToffeeFactory {
  namespace ToffeeFactory {

    public class Port : MonoBehaviour {
    
      [SerializeField] private PortType m_type;
    
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
  
    public enum PortType {
      In,
      Out
    }
  
  }
}