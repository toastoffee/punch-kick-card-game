using System.Collections.Generic;

namespace ToffeeFactory {
  public enum FormulaFamily {
    Miner,
    Furnace
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
  }
}