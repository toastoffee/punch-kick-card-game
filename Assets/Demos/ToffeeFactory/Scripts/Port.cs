using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ToffeeFactory {

  public class Port : MonoBehaviour , IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler{
  
    [SerializeField] private PortType m_type;
    
    private Port m_connectedPort = null;
    public bool isConnected => m_connectedPort != null;
    public PortType type => m_type;
    public Port connectedPort { get => m_connectedPort; set => m_connectedPort = value; }

    [HideInInspector]
    public GameObject connectPipe = null;
    
    [HideInInspector]
    public Machine affiliated = null;

    [SerializeField]
    private SpriteRenderer spr;

    [SerializeField]
    private Sprite unConnectSprite, connectSprite;

    [SerializeField]
    private float hoverSwellSize, pressedShrinkSize;
    
    [SerializeField]
    private float hoverSwellDuration, pressedShrinkDuration, recoverDuration;

    private void Start() {
      spr.sprite = unConnectSprite;
    }

    private void Update() {
      if (isConnected) {
        spr.sprite = connectSprite;
      } else {
        spr.sprite = unConnectSprite;
      }
    }

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
      
      
      Sequence sequence = DOTween.Sequence();
      sequence.Append(transform.DOScale(pressedShrinkSize * Vector3.one, pressedShrinkDuration));
      sequence.Append(transform.DOScale( Vector3.one, hoverSwellDuration));
    }
    public void OnPointerEnter(PointerEventData eventData) {
      transform.DOScale(hoverSwellSize * Vector3.one, hoverSwellDuration);
    }
    public void OnPointerExit(PointerEventData eventData) {
      transform.DOScale(Vector3.one, recoverDuration);
    }
  }

  public enum PortType {
    In,
    Out
  }

}
