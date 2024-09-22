using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ToffeeFactory {
  public class AdvancedProducer : Machine {
    
    public List<Port> inPorts;
    public List<Port> outPorts;

    public StorageSet storageSet;

    private void Start() {
      storageSet.SetStorageSize(inPorts.Count + outPorts.Count);
    }

    public override bool ReceiveIngredient(Ingredient ingredient) {
      throw new System.NotImplementedException();
    }
  } 
}
