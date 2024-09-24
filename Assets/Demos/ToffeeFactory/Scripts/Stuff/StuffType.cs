using System;
using UnityEngine;

namespace ToffeeFactory {
  [Serializable]
  public enum StuffType {
    NONE,
    
    IronMine,
    CoalMine,
    CooperMine,
    
    IronOre,
    CoalOre,
    CooperOre,
    
    IronIngot,
    CooperIngot,
    
    IronPlate,
    CooperWire,
    
    CircuitBoard
  }
}