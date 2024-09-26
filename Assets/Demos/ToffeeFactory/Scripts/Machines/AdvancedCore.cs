using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ToffeeFactory {
  public class AdvancedCore : AdvancedMachine {
    
    public List<Port> inPorts;

    private void Start() {
      foreach (var port in inPorts) {
        port.machineBelong = this;
      }
    }

    public override bool ReceiveStuffLoad(StuffLoad load) {
      if (CoreStorageSet.Instance.IsSpaceRemained(load)) {
        CoreStorageSet.Instance.TryAdd(load);
        return true;
      }
      return false;
    }

  } 
}
