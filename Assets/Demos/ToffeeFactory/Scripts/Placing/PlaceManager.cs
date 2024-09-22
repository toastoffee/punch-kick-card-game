using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToffeeFactory {
  public class PlaceManager : MonoSingleton<PlaceManager> {
    public enum State {
      IDLE,
      PLACING,
    }
    public Vector2 cellSize;
    private State state;
    private PlaceAnchor placingAnchor;
    private BuyListItem buyListItem;

    public void StartPlace(BuyListItem buyListItem) {
      if (state != State.IDLE) {
        return;
      }
      var placeAnchorPrefab = buyListItem.placingPrefab;
      this.buyListItem = buyListItem;
      state = State.PLACING;
      placingAnchor = Instantiate(placeAnchorPrefab);
      placingAnchor.transform.position = 99999 * Vector2.one;
    }

    public void Update() {
      if (state == State.IDLE) {
        return;
      }
      if (Input.GetMouseButton(1)) {
        EndPlace();
      }
      if (state == State.IDLE) {
        return;
      }

      var mousePos = (Vector2)Input.mousePosition;
      mousePos = Camera.main.ScreenToWorldPoint(mousePos);
      var offset = placingAnchor.anchorOffset * cellSize;
      var pos = mousePos - offset;
      pos = pos / cellSize;
      pos = new Vector2(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y)) * cellSize;

      placingAnchor.transform.position = pos + offset;

      if (Input.GetMouseButtonDown(0)) {
        if (!Status.Instance.CanAfford(buyListItem.price)) {
          return;
        }
        Status.Instance.RemoveMoney(buyListItem.price);
        placingAnchor = Instantiate(buyListItem.placingPrefab);
        placingAnchor.transform.position = 99999 * Vector2.one;
      }
    }

    public void EndPlace() {
      Destroy(placingAnchor.gameObject);
      state = State.IDLE;
      buyListItem = null;
      placingAnchor = null;
    }
  }
}
