using System;

using UnityEngine;

namespace ToffeeFactory {
  public abstract class Machine : MonoBehaviour {
    // may have a lot of ports

    public abstract bool ReceiveIngredient(Ingredient ingredient);
  }
}