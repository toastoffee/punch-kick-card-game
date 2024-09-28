using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ToffeeFactory {
  public class GlobalClickReceiver : MonoSingleton<GlobalClickReceiver>, IPointerClickHandler {
    public GameObject blockerObj;
    private Stack<GameObject>[] m_listenerArrs = new Stack<GameObject>[2] {
      new Stack<GameObject>(),
      new Stack<GameObject>()
    };

    public void Update() {
      var hasListener = false;
      foreach (var listenerArr in m_listenerArrs) {
        if (listenerArr != null) {
          foreach (var listener in listenerArr) {
            if (listener != null) {
              hasListener = true;
              break;
            }
          }
        }
      }
      blockerObj.SetActive(hasListener);
    }

    public void OnPointerClick(PointerEventData eventData) {
      if (eventData.button >= PointerEventData.InputButton.Middle) {
        return;
      }
      GameObject listener = null;
      var listeners = m_listenerArrs[(int)eventData.button];
      while (listener == null && listeners.Count > 0) {
        listener = listeners.Pop();
      }
      if (listener == null) {
        return;
      }
      listener.BroadcastMessage(nameof(ICallBack.OnGlobalClick), (int)eventData.button);
    }

    public void AddListener(GameObject obj, int idx) {
      if (idx > 1) {
        return;
      }
      m_listenerArrs[idx].Push(obj);
    }

    public interface ICallBack {
      void OnGlobalClick(int buttonId);
    }
  }
}
