using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TurnGame {
  using Text = TMP_Text;
  public class DuelUI : SeqNumModelBinder<DuelProp> {
    public class ApAdpater : SimpleLayout.Adapter {
      public DuelUI host;

      public override int count {
        get {
          var prop = host.prop.playerProp;
          return prop.maxAp;
        }
      }

      public override void OnRender(int position, SimpleLayout.ViewCache viewCache) {
        var prop = host.prop.playerProp;
        var view = viewCache.GetView<ApPointView>();
        view.SetAp(position < prop.ap);
      }
    }

    public GameObject endTurnBtnObj;
    public Text turnHintText;
    public Text apCntText;
    public SimpleLayout apLayout;
    private ApAdpater apAdpater;

    private void Awake() {
      apAdpater = new ApAdpater() { host = this };
      apLayout.SetAdapter(apAdpater);
    }
    public override void OnSeqNumUpdate(DuelProp prop) {
      endTurnBtnObj.SetActive(prop.inPlayerTurn);

      turnHintText.text = prop.inPlayerTurn ? "我方回合" : "敌方回合";
      apCntText.text = prop.playerProp.ap.ToString();
      apLayout.NotifyUpdate();
    }
  }
}
