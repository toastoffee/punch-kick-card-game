using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace ToffeeFactory {
  public static class TFUtils {
    public static void DisconnectAllChildrenPort(GameObject rootObj) {
      var ports = rootObj.GetComponentsInChildren<Port>();
      foreach (var port in ports) {
        port.Disconnect();
      }
    }

    public static void DeleteMachine(GameObject gameObject) {
      DisconnectAllChildrenPort(gameObject);
      GameObject.Destroy(gameObject);
    }

    public static Vector3 SnapToGrid(Vector3 pos) {
      pos += new Vector3(2, 2, 0);
      pos /= 4;
      pos.x = Mathf.RoundToInt(pos.x);
      pos.y = Mathf.RoundToInt(pos.y);
      pos *= 4;
      pos -= new Vector3(2, 2, 0);
      return pos;
    }
  }
}
