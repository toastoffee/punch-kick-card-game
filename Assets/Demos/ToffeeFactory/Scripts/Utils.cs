using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToffeeFactory {
  public static class TFUtils {
    public static void DisconnectAllChildrenPort(GameObject rootObj) {
      var ports = rootObj.GetComponentsInChildren<Port>();
      foreach (var port in ports) {
        port.Disconnect();
      }
    }
  }
}
