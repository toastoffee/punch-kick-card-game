using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ToffeeFactory {

  public class Port : MonoBehaviour , IPointerClickHandler{
  
    [SerializeField] private PortType m_type;
  
    private Port m_connectedPort = null;
    public bool isConnected => m_connectedPort != null;
    public PortType type => m_type;
    public Port connectedPort { get => m_connectedPort; set => m_connectedPort = value; }

    public GameObject connectPipe = null;
    
    public Machine affiliated = null;
    
    
    private void Connect() {
      if (!isConnected) {
        // handle connection click event
        PortConnectManager.Instance.HandleConnect(this);
      }
    }

    private void Disconnect() {
      // check if is connected
      if (isConnected) {
        // disconnect and destroy pipe
        connectedPort.connectPipe = null;
        connectedPort.connectedPort = null;
        connectedPort = null;
        
        Destroy(connectPipe);
        connectPipe = null;
      }
    }

    public void OnPointerClick(PointerEventData eventData) {
      if (eventData.button == PointerEventData.InputButton.Left) {
        Connect();
      } else if (eventData.button == PointerEventData.InputButton.Right) {
        Disconnect();
      }
    }
  }

  public enum PortType {
    In,
    Out
  }

}
