using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ToffeeFactory {
  public class MachineMenuFormulaOptionItem : MonoBehaviour {
    public ProduceFormula formula;
    public TMP_Text nameText;
    private ComponentFinder<MachineMenu> m_menuFinder;

    public void Render(ProduceFormula formula) {
      this.formula = formula;
      nameText.text = formula.formulaName;
    }

    public void OnClick() {
      m_menuFinder.Get(this).OnFormulaClick(formula);
    }
  }
}
