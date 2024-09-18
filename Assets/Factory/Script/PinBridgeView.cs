using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinBridgeView : MonoBehaviour {
  public TheFactoryGame.PinBridge pinBridge;
  public RectTransform strench;

  void Update() {
    var fromPos = pinBridge.fromPin.cell.cellViewPos;
    var toPos = pinBridge.toPin.cell.cellViewPos;

    transform.position = fromPos;
    strench.up = toPos - fromPos;
    strench.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Vector2.Distance(fromPos, toPos));
  }
}
