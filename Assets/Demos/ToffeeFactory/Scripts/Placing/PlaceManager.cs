using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace ToffeeFactory {
  public class PlaceManager : MonoSingleton<PlaceManager> {
    public enum State {
      IDLE,
      BUY_PLACING,
      RE_PLACING
    }
    public enum Rot {
      UP, RIGHT, DOWN, LEFT
    }

    public static readonly Dictionary<Rot, Vector2> rot2up = new Dictionary<Rot, Vector2>{
      {Rot.UP, Vector2.up },
      {Rot.RIGHT, Vector2.right},
      {Rot.DOWN, Vector2.down},
      {Rot.LEFT, Vector2.left},
    };

    public interface IRotHandler {
      void OnPlaceRot(Rot rot);
    }

    public struct PlaceContext {
      public State state;
      public PlaceAnchor placingAnchor;
      public BuyListItem buyListItem;
      public Rot placingRot;
      public Vector3 replaceSrcPos;

      public void StartBuyPlace(BuyListItem buyListItem) {
        var placeAnchorPrefab = buyListItem.placingPrefab;
        this.buyListItem = buyListItem;
        state = State.BUY_PLACING;
        placingAnchor = Instantiate(placeAnchorPrefab);
        placingAnchor.transform.position = 99999 * Vector2.one;
        placingAnchor.OnStartPlace();
      }

      public void StartRePlace(PlaceAnchor placeAnchor) {
        placingAnchor = placeAnchor;
        state = State.RE_PLACING;
        replaceSrcPos = placeAnchor.transform.position;
        placingAnchor.OnStartPlace();
      }
    }

    private PlaceContext m_ctx;

    public void StartBuyPlace(BuyListItem buyListItem) {
      if (m_ctx.state != State.IDLE) {
        return;
      }
      m_ctx = new PlaceContext();
      m_ctx.StartBuyPlace(buyListItem);
    }

    public void StartRePlace(PlaceAnchor placeAnchor) {
      if (m_ctx.state != State.IDLE) {
        return;
      }
      m_ctx = new PlaceContext();
      m_ctx.StartRePlace(placeAnchor);
    }

    public void Update() {
      if (m_ctx.state == State.IDLE) {
        return;
      }
      if (Input.GetMouseButton(1)) {
        m_ctx.placingAnchor.OnEndPlace();
        EndPlace();
      }
      if (m_ctx.state == State.IDLE) {
        return;
      }

      if (Input.GetKeyDown(KeyCode.R)) {
        m_ctx.placingRot = (Rot)(((int)m_ctx.placingRot + 1) % 4);

        // rotate ports
        var ports = m_ctx.placingAnchor.GetComponentsInChildren<Port>();
        foreach (var port in ports) {
          var dir = port.transform.localPosition;
          dir = dir.Rotate(Vector3.forward, -90f);

          port.transform.DOLocalMove(dir, 0.1f);
        }

        m_ctx.placingAnchor.BroadcastMessage(nameof(IRotHandler.OnPlaceRot), m_ctx.placingRot, SendMessageOptions.DontRequireReceiver);
      }

      var cellSize = Consts.cellSize;
      var rotation = Quaternion.Euler(0, 0, (int)m_ctx.placingRot * -90);

      var mousePos = (Vector2)Input.mousePosition;
      mousePos = Camera.main.ScreenToWorldPoint(mousePos);
      var offset = m_ctx.placingAnchor.anchorOffset * cellSize;
      offset = rotation * offset;
      var pos = mousePos - offset;
      pos = pos / cellSize;
      pos = new Vector2(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y)) * cellSize;

      PlaceAnchor.isShowingRange = true;
      m_ctx.placingAnchor.transform.position = pos + offset;
      //m_ctx.placingAnchor.transform.rotation = rotation;

      if (Input.GetMouseButtonDown(0)) {
        if (!m_ctx.placingAnchor.CanPlace) {
          return;
        }
        // if (m_ctx.state == State.BUY_PLACING) {
        //   if (!Status.Instance.CanAfford(m_ctx.buyListItem.price)) {
        //     return;
        //   }
        //   Status.Instance.RemoveMoney(m_ctx.buyListItem.price);
        //   m_ctx.placingAnchor.OnEndPlace();
        //
        //   var cached = m_ctx.buyListItem;
        //   m_ctx = new PlaceContext();
        //   m_ctx.StartBuyPlace(cached);
        // }
        if (m_ctx.state == State.BUY_PLACING) {   //! modified
          if (!RepositoryStorageSet.Instance.IsSufficient(m_ctx.buyListItem.costs)) {
            return;
          }
          RepositoryStorageSet.Instance.TryConsume(m_ctx.buyListItem.costs);
          m_ctx.placingAnchor.OnEndPlace();

          var cached = m_ctx.buyListItem;
          m_ctx = new PlaceContext();
          m_ctx.StartBuyPlace(cached);
        } else if (m_ctx.state == State.RE_PLACING) {
          m_ctx.placingAnchor.OnEndPlace();
          m_ctx.placingAnchor = null;
          EndPlace();
        }
      }
    }

    public void EndPlace() {
      do {
        if (m_ctx.state == State.BUY_PLACING) {
          if (m_ctx.placingAnchor == null) {
            break;
          }
          m_ctx.placingAnchor.OnEndPlace();
          Destroy(m_ctx.placingAnchor.gameObject);
          m_ctx.placingAnchor = null;

        } else if (m_ctx.state == State.RE_PLACING) {
          if (m_ctx.placingAnchor == null) {
            break;
          }
          m_ctx.placingAnchor.transform.position = m_ctx.replaceSrcPos;
        }
      } while (false);
      m_ctx.state = State.IDLE;
      m_ctx.buyListItem = null;
      PlaceAnchor.isShowingRange = false;
    }
  }
}
