using System.Collections;
using System.Collections.Generic;
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
    private State state;
    private PlaceAnchor placingAnchor;
    private BuyListItem buyListItem;
    private Rot placingRot;
    private Vector3 replaceSrcPos;

    public void StartBuyPlace(BuyListItem buyListItem) {
      if (state != State.IDLE) {
        return;
      }
      var placeAnchorPrefab = buyListItem.placingPrefab;
      this.buyListItem = buyListItem;
      state = State.BUY_PLACING;
      placingAnchor = Instantiate(placeAnchorPrefab);
      placingAnchor.transform.position = 99999 * Vector2.one;
    }

    public void StartRePlace(PlaceAnchor placeAnchor) {
      if (state != State.IDLE) {
        return;
      }
      placingAnchor = placeAnchor;
      state = State.RE_PLACING;
      replaceSrcPos = placeAnchor.transform.position;
    }

    public void Update() {
      if (state == State.IDLE) {
        return;
      }
      if (Input.GetMouseButton(1)) {
        placingAnchor.OnEndPlace();
        EndPlace();
      }
      if (state == State.IDLE) {
        return;
      }

      if (Input.GetKeyDown(KeyCode.R)) {
        placingRot = (Rot)(((int)placingRot + 1) % 4);
      }

      var cellSize = Consts.cellSize;
      var rotation = Quaternion.Euler(0, 0, (int)placingRot * -90);

      var mousePos = (Vector2)Input.mousePosition;
      mousePos = Camera.main.ScreenToWorldPoint(mousePos);
      var offset = placingAnchor.anchorOffset * cellSize;
      offset = rotation * offset;
      var pos = mousePos - offset;
      pos = pos / cellSize;
      pos = new Vector2(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y)) * cellSize;

      placingAnchor.isPlacing = true;
      PlaceAnchor.isShowingRange = true;
      placingAnchor.transform.position = pos + offset;
      placingAnchor.transform.rotation = rotation;

      if (Input.GetMouseButtonDown(0)) {
        if (!placingAnchor.CanPlace) {
          return;
        }
        if (state == State.BUY_PLACING) {
          if (!Status.Instance.CanAfford(buyListItem.price)) {
            return;
          }
          Status.Instance.RemoveMoney(buyListItem.price);
          placingAnchor.OnEndPlace();
          placingAnchor = Instantiate(buyListItem.placingPrefab);
          placingAnchor.transform.position = 99999 * Vector2.one;
        } else if (state == State.RE_PLACING) {
          placingAnchor.OnEndPlace();
          placingAnchor = null;
          EndPlace();
        }
      }
    }

    public void EndPlace() {
      do {
        if (state == State.BUY_PLACING) {
          if (placingAnchor == null) {
            break;
          }
          placingAnchor.OnEndPlace();
          Destroy(placingAnchor.gameObject);
          placingAnchor = null;

        } else if (state == State.RE_PLACING) {
          if (placingAnchor == null) {
            break;
          }
          placingAnchor.transform.position = replaceSrcPos;
        }
      } while (false);
      state = State.IDLE;
      buyListItem = null;
      PlaceAnchor.isShowingRange = false;
    }
  }
}
