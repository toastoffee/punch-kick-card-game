using System;
using System.Collections.Generic;

namespace ToffeeFactory {
  
  [Serializable]
  public class ProduceFormula {

    public FormulaFamily family;
    public string formulaName;
    public float produceInterval;
    public List<StuffLoad> ingredients = new List<StuffLoad>();
    public List<StuffLoad> products = new List<StuffLoad>();

    public StuffType storeType;
    
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