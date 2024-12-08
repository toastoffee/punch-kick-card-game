using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TurnGame {
  using Text = TMPro.TMP_Text;
  public class OptionView : MonoBehaviour {
    public GameObject apCostObj;
    public Text apCostText;
    public Text nameText;
    private SkillProp m_skillProp;
    private TeaProp m_casterProp;

    public void Render(SkillProp prop, TeaProp casterProp) {
      m_skillProp = prop;
      m_casterProp = casterProp;
      nameText.text = prop.skillModel.name;
      apCostText.text = prop.apCost.ToString();
    }

    public void OnClick() {
      var castCtx = new CastingContext {
        caster = m_casterProp,
        skillProp = m_skillProp,
      };
      TurnGame.Instance.OnTryCastSkill(castCtx);
    }
  }
}