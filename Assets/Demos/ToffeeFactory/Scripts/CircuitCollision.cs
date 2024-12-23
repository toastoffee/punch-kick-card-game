using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToffeeFactory {

  public class CircuitCollision : MonoBehaviour {
    private void Awake() {
      Debug.Assert(gameObject.layer == LayerMask.NameToLayer("Circuit"));
    }

    private void OnTriggerStay2D(Collider2D collision) {
    }
  }
}
