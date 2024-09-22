using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ToffeeFactory {
  public class AdvancedProducer : AdvancedMachine {
    
    public List<Port> inPorts;
    public List<Port> outPorts;

    public StorageSet storageSet;

    private void Start() {
      storageSet.SetStorageSize(inPorts.Count + outPorts.Count);
      
    }
    
    public override bool ReceiveStuffLoad(StuffLoad load) {
      throw new NotImplementedException();
    }
  } 
}
