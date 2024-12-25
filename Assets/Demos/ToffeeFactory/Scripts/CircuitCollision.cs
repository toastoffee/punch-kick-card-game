using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToffeeFactory {

  public class CircuitCollision : MonoBehaviour {
    public bool isWire;
    public bool isVerWire;
    public bool isLegal { get; private set; }
    private List<CircuitCollision> collisions = new List<CircuitCollision>();

    private void Start() {
      isLegal = true;
      Debug.Assert(gameObject.layer == LayerMask.NameToLayer("Circuit"));
    }

    private void OnTriggerEnter2D(Collider2D collider) {
      var collision = collider.GetComponent<CircuitCollision>();
      if (collision == null) {
        return;
      }
      collisions.Add(collision);
      OnCollisionsChange();
    }

    private void OnTriggerExit2D(Collider2D collider) {
      var collision = collider.GetComponent<CircuitCollision>();
      if (collision == null) {
        return;
      }
      collisions.Remove(collision);
      OnCollisionsChange();
    }

    private void OnCollisionsChange() {
      collisions.RemoveAll(x => x == null);
      isLegal = true;
      if (collisions.SafeCount() == 0) {
        return;
      }
      if (!isWire) {
        foreach (var collision in collisions) {
          if (!collision.isWire) {
            isLegal = false;
            return;
          }
        }
      } else {
        foreach (var collision in collisions) {
          if (collision.isWire && collision.isVerWire == this.isVerWire) {
            isLegal = false;
            return;
          }
        }
      }
    }
  }
}
