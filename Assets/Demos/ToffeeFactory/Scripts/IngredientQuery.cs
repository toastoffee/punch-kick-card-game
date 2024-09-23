using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientQuery : Singleton<IngredientQuery>
{
  public string GetRichText(string name) {
    switch (name) {
      case "ironMine":  // 铁矿
        return "<color=#FF171D>铁矿</color>";
      case "coalMine":  // 煤炭矿
        return "<color=#9A6C4E>煤炭矿</color>";
      case "cooperMine":  // 铜矿
        return "<color=#00DB65>铜矿</color>";
      
      case "ironOre":   // 铁矿石
        return "<color=#FF1F47>铁矿石</color>";
      case "coalOre":  // 煤炭
        return "<color=#874743>煤炭</color>";
      case "cooperOre":  // 铜矿石
        return "<color=#27BC4F>铜矿石</color>";
      
      case "ironIngot":      // 铁锭
        return "<color=#939393>铁锭</color>";
      
      
    }

    return "nothing";
  }
}
