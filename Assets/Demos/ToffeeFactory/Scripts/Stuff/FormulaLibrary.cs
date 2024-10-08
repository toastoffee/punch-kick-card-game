using System.Collections.Generic;

namespace ToffeeFactory {
  public enum FormulaFamily {
    Mine,
    Miner,
    Furnace,
    Maker
  }
  public class FormulaLibrary : MonoSingleton<FormulaLibrary> {
    public List<ProduceFormula> formulas;
    
    public List<ProduceFormula> GetFormulasOfFamily(FormulaFamily family) {
      List<ProduceFormula> res = new List<ProduceFormula>();
      
      foreach (var f in formulas) {
        if (f.family == family) {
          res.Add(f);    
        }
      }
      
      return res;
    }
    
    public static string GetFormulaStr(ProduceFormula formula) {
      string equation = "";
      for (int i = 0; i < formula.ingredients.Count; i++) {
        equation += $"{GetNumString(formula.ingredients[i].count)} {StuffQuery.GetRichText(formula.ingredients[i].type)} ";

        if (i != formula.ingredients.Count - 1) {
          equation += "+ ";
        }
      }
      equation += "= ";
      
      for (int i = 0; i < formula.products.Count; i++) {
        equation += $"{GetNumString(formula.products[i].count)} {StuffQuery.GetRichText(formula.products[i].type)} ";

        if (i != formula.products.Count - 1) {
          equation += "+ ";
        }
      }

      return equation;
    }

    public static string GetNumString(int num) {
      if (num == 42) {
        return "<size=11>\u221e</size>";
      } else {
        return $"{num}";
      }
    }
  }
  
}