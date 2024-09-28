using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ToffeeFactory {
  public class PlaceAnchor : MonoBehaviour, IMachineMouseHoldCallback {
    public Vector2 anchorOffset;
    public Vector2Int occupySize;
    public bool disableReplace;
    private PlaceOccupy occupy;
    private static Collider2D[] collideRes = new Collider2D[10];
    public static bool isShowingRange;

    private bool isOverlaped;
    private bool m_isPlacing;
    private float m_lastReleaseTime;

    public bool CanPlace => !isOverlaped;

    private void Awake() {
      var occupyPrefab = Resources.Load<PlaceOccupy>("Prefabs/occupy");
      occupy = Instantiate(occupyPrefab, transform);
      occupy.scale.localScale = new Vector3(occupySize.x - 0.05f, occupySize.y - 0.05f, 1);
    }

    private void Update() {
      if (!m_isPlacing && !isShowingRange) {
        occupy.spriteRenderer.gameObject.SetActive(false);
      } else {
        occupy.spriteRenderer.gameObject.SetActive(true);
      }
      if (!m_isPlacing) {
        occupy.spriteRenderer.color = occupy.rangeColor;
      } else {
        occupy.spriteRenderer.color = isOverlaped ? occupy.unavailColor : occupy.availColor;
      }
    }

    private void FixedUpdate() {
      if (!m_isPlacing) {
        return;
      }
      var filter = new ContactFilter2D();
      filter.SetLayerMask(LayerMask.GetMask("Occupy"));
      isOverlaped = Physics2D.OverlapCollider(occupy.boxCollider, filter, collideRes) > 0;
    }

    public void OnEndPlace() {
      m_isPlacing = false;
      m_lastReleaseTime = Time.realtimeSinceStartup;
      BroadcastMessage(nameof(IMachinePlaceCallback.OnMachinePlaceEnd), SendMessageOptions.DontRequireReceiver);
    }

    public void OnStartPlace() {
      m_isPlacing = true;
      TFUtils.DisconnectAllChildrenPort(gameObject);
      BroadcastMessage(nameof(IMachinePlaceCallback.OnMachinePlaceStart), SendMessageOptions.DontRequireReceiver);
    }

    public void TryInvokePlace() {
      if (disableReplace || m_isPlacing) {
        return;
      }
      if (Time.realtimeSinceStartup - m_lastReleaseTime < 0.1f) {
        return;
      }
      PlaceManager.Instance.StartRePlace(this);
    }

    public void OnLeftHoldDone() {
      TryInvokePlace();
    }

    public void OnRightHoldDone() {
      TFUtils.DeleteMachine(gameObject);
    }
  }

  public interface IMachinePlaceCallback {
    void OnMachinePlaceEnd();
    void OnMachinePlaceStart();
  }
}
