using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TurnGame {
  using Text = TMP_Text;
  public class TeaView : SeqNumModelBinder<TeaProp> {
    public float frontUpDist;
    public RectTransform container;
    public RectTransform hpBar;
    public Text nameText;
    public Text hpText;
    public Text atkText;
    public Text defText;
    public override void OnSeqNumUpdate(TeaProp prop) {
      nameText.text = prop.name;
      hpText.text = $"{"HP:"} {prop.hp} / {prop.maxHp}";
      atkText.text = $"ATK: {prop.atk}";
      defText.text = $"DEF: {prop.def} {(prop.def == 0 ? "" : $"({(prop.defRatio * 100).ToString("0")}%)")}";

      var upDist = prop.isFront ? frontUpDist : 0;
      upDist *= prop.isEnemy ? -1 : 1;
      container.localPosition = Vector3.up * upDist;

      var hpRatio = prop.hp / ((float)(prop.maxHp)).NotLessThanZero();
      hpRatio = Mathf.Clamp01(hpRatio);
      hpBar.SetLocalScaleX(hpRatio);
    }

    public void OnClick() {
      TurnGame.Instance.OnTeaClick(prop);
    }
  }
}
