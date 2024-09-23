using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToffeeFactory {
  public class PlaceAnchor : MonoBehaviour {
    public Vector2 anchorOffset;
    public Vector2Int occupySize;
    public bool isPlacing;
    private PlaceOccupy occupy;
    private static Collider2D[] collideRes = new Collider2D[10];
    public static bool isShowingRange;

    private bool isOverlaped;

    public bool CanPlace => !isOverlaped;

    private void Awake() {
      var occupyPrefab = Resources.Load<PlaceOccupy>("Prefabs/occupy");
      occupy = Instantiate(occupyPrefab, transform);
      occupy.transform.localScale = new Vector3(occupySize.x - 0.05f, occupySize.y - 0.05f, 1);
    }

    private void Update() {
      if (!isPlacing && !isShowingRange) {
        occupy.spriteRenderer.gameObject.SetActive(false);
      } else {
        occupy.spriteRenderer.gameObject.SetActive(true);
      }
      if (!isPlacing) {
        occupy.spriteRenderer.color = occupy.rangeColor;
      } else {
        occupy.spriteRenderer.color = isOverlaped ? occupy.unavailColor : occupy.availColor;
      }
    }

    private void FixedUpdate() {
      if (!isPlacing) {
        return;
      }
      var filter = new ContactFilter2D();
      filter.SetLayerMask(LayerMask.GetMask("Occupy"));
      isOverlaped = Physics2D.OverlapCollider(occupy.boxCollider, filter, collideRes) > 0;
    }
  }
}
