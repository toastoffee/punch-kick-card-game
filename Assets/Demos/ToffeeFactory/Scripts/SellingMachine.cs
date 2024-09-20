using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

namespace ToffeeFactory {
  public class SellingMachine : Machine {
    [Serializable]
    public class Price {
      public string name;
      public int price;
    }

    public List<Port> inPorts;
    public List<Port> outPorts;

    [TableList]
    public List<Price> prices;

    public override bool ReceiveIngredient(Ingredient ingredient) {
      var price = prices.Find(x => x.name == ingredient.name);
      if (price == null) {
        return true;
      }

      Status.Instance.AddMoney(price.price * ingredient.count);
      return true;
    }
  }
}
