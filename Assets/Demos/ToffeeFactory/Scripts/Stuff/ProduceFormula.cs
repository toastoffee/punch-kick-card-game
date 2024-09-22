using System;
using System.Collections.Generic;

namespace ToffeeFactory {
  
  [Serializable]
  public class ProduceFormula {
    public float produceInterval;

    public List<StuffLoad> ingredients;
    public List<StuffLoad> products;

    public List<StuffType> GetStuffInvolved() {
      List<StuffType> types = new List<StuffType>();

      foreach (var load in ingredients) {
        if (!types.Contains(load.type)) {
          types.Add(load.type);
        }
      }
      
      foreach (var load in products) {
        if (!types.Contains(load.type)) {
          types.Add(load.type);
        }
      }
      return types;
    }
  }
}