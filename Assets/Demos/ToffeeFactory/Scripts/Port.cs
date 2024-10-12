using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ToffeeFactory {
  

  public class Port : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField] private PortType m_type;

    private Port m_connectedPort = null;
    public bool isConnected => m_connectedPort != null;
    public PortType type => m_type;
    public Port connectedPort { get => m_connectedPort; set => m_connectedPort = value; }

    [HideInInspector]
    public GameObject connectPipe = null;

    [HideInInspector]
    public Machine affiliated = null;

    [HideInInspector]
    public AdvancedMachine machineBelong;

    [SerializeField]
    private SpriteRenderer spr;

    [SerializeField]
    private Sprite unConnectSprite, connectSprite;

    [SerializeField]
    private float hoverSwellSize, pressedShrinkSize;

    [SerializeField]
    private float hoverSwellDuration, pressedShrinkDuration, recoverDuration;

    public TMP_Text typeText, countText;

    private Tween _exitTween, _enterTween, _clickTween;
    
    private Tween exitTween {
      set {
        _exitTween = value;
      }
      get => _exitTween;
    }
    
    private Tween enterTween {
      set {
        _enterTween = value;
      }
      get => _enterTween;
    }
    
    private Tween clickTween {
      set {
        _clickTween = value;
      }
      get => _clickTween;
    }
    
    private void Start() {
      spr.sprite = unConnectSprite;

      var pos = transform.position;
      pos += new Vector3(2, 2, 0);
      pos /= 4;
      pos.x = Mathf.RoundToInt(pos.x);
      pos.y = Mathf.RoundToInt(pos.y);
      pos *= 4;
      pos -= new Vector3(2, 2, 0);
      transform.position = pos;
    }

    private void Update() {
      if (isConnected) {
        spr.sprite = connectSprite;
      } else {
        spr.sprite = unConnectSprite;
      }
    }

    public void Connect() {
      if (!isConnected) {
        // handle connection click event
        PortConnectManager.Instance.HandleConnect(this);
      }
    }

    public void Disconnect() {
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
      sequence.Append(spr.transform.DOScale(pressedShrinkSize * Vector3.one, pressedShrinkDuration));
      sequence.Append(spr.transform.DOScale(hoverSwellSize * Vector3.one, hoverSwellDuration));
      clickTween = sequence;
    }
    public void OnPointerEnter(PointerEventData eventData) {
      enterTween = spr.transform.DOScale(hoverSwellSize * Vector3.one, hoverSwellDuration);
    }
    public void OnPointerExit(PointerEventData eventData) {
      enterTween?.Kill();
      clickTween?.Kill();
      exitTween = spr.transform.DOScale(Vector3.one, recoverDuration);
    }
  }

  public enum PortType {
    In,
    Out
  }

}
