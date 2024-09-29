using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToffeeFactory {
  public class AdvancedRepository : AdvancedMachine {
    public List<Port> inPorts;
    
    public StuffType _stuffType = StuffType.NONE;
    

    private void Start() {
      foreach (var port in inPorts) {
        port.machineBelong = this;
      }
      
      _stuffType = RepositoryStorageSet.Instance._stuffTypes[0];
      
      RepositoryStorageSet.Instance.Register(this);
    }

    private void OnDestroy() {
      RepositoryStorageSet.Instance.Unregister(this);
    }

    public override bool ReceiveStuffLoad(StuffLoad load) {
      if (load.type == _stuffType && RepositoryStorageSet.Instance.IsSpaceRemained(load)) {
        RepositoryStorageSet.Instance.TryAdd(load);
        return true;
      }
      return false;
    }
    
    
    public override IEnumerable<Port> GetAllPorts() {
      return inPorts;
    }
  }
}