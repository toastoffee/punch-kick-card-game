using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ToffeeFactory {
  public class MachineMouseInteract : MonoBehaviour, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler, IMachinePlaceCallback {
    public float[] holdTimes;
    public Transform[] holdProgBar;
    public GameObject[] holdProgObj;
    public GameObject blocker;
    private float[] mouseDownTime = new float[2];
    private int[] holdFlags = new int[2];
    private TimeFlag placeFlag;

    private bool m_isPlacing;

    private const float PLACE_LOCK = 0.3F;

    private void Update() {
      for (int i = 0; i < 2; i++) {
        var delta = Time.realtimeSinceStartup - mouseDownTime[i];
        var flag = holdFlags[i];
        if (flag == 1) {
          var prog = Mathf.Clamp01(delta / holdTimes[i]);
          holdProgBar[i].transform.SetLocalScaleX(prog);
          holdProgObj[i].SetActive(true);
        }
        if (flag == 1 && delta > holdTimes[i]) {
          holdFlags[i] = 2;
          holdProgObj[i].SetActive(false);
          if (i == 0) {
            transform.parent.BroadcastMessage(nameof(IMachineMouseCallback.OnLeftHoldDone), SendMessageOptions.DontRequireReceiver);
          } else if (i == 1) {
            transform.parent.BroadcastMessage(nameof(IMachineMouseCallback.OnRightHoldDone), SendMessageOptions.DontRequireReceiver);
          }
        }
      }

      if (!m_isPlacing && !placeFlag.IsTrue(PLACE_LOCK)) {
        blocker.SetActive(false);
      }
    }

    private void _OnHoldStop(int idx) {
      holdFlags[idx] = 0;
      holdProgBar[idx].SetLocalScaleX(0);
      holdProgObj[idx].SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData) {
      if (placeFlag.IsTrue(PLACE_LOCK)) {
        return;
      }
      if (eventData.button >= PointerEventData.InputButton.Middle) {
        return;
      }
      if (holdFlags[(int)eventData.button] > 0) {
        return;
      }
      holdFlags[(int)eventData.button] = 1;
      mouseDownTime[(int)eventData.button] = Time.realtimeSinceStartup;
    }

    public void OnPointerExit(PointerEventData eventData) {
      if (placeFlag.IsTrue(PLACE_LOCK)) {
        return;
      }
      if (eventData.button >= PointerEventData.InputButton.Middle) {
        return;
      }
      _OnHoldStop((int)eventData.button);
    }

    public void OnPointerUp(PointerEventData eventData) {
      if (placeFlag.IsTrue(PLACE_LOCK)) {
        return;
      }
      if (eventData.button >= PointerEventData.InputButton.Middle) {
        return;
      }
      _OnHoldStop((int)eventData.button);
    }

    public void OnMachinePlaceStart() {
      _OnHoldStop(0);
      m_isPlacing = true;
      blocker.SetActive(true);
    }

    public void OnMachinePlaceEnd() {
      m_isPlacing = false;
      placeFlag.SetTrue();
      _OnHoldStop(0);
    }
  }

  public interface IMachineMouseCallback {
    void OnLeftHoldDone();
    void OnRightHoldDone();
  }
}