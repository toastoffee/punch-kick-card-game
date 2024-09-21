using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientQuery : Singleton<IngredientQuery>
{
  public string GetRichText(string name) {
    switch (name) {
      case "ore":
        return "<color=#FF1F47>ore</color>";
      case "iron":
        return "<color=#939393>iron</color>";
    }

    return "nothing";
  }
}
