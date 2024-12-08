using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnGame {

  public class OptionUI : SeqNumModelBinder<TeamProp> {
    private class OptionAdapter : SimpleLayout.Adapter {
      public OptionUI host;
      public override int count => host.m_teaProp.skillProps.SafeCount();
      public override void OnRender(int position, SimpleLayout.ViewCache viewCache) {
        var prop = host.m_teaProp.skillProps[position];
        var view = viewCache.GetView<OptionView>();
        view.Render(prop, host.m_teaProp);
      }
    }

    public SimpleLayout simpleLayout;
    private TeaProp m_teaProp;

    private void Awake() {
      var adapter = new OptionAdapter() { host = this };
      simpleLayout.SetAdapter(adapter);
    }

    public override void OnSeqNumUpdate(TeamProp prop) {
      var teaProp = prop.teaProps[prop.frontIndex];
      if (teaProp == m_teaProp) {
        return;
      }
      m_teaProp = teaProp;

      simpleLayout.NotifyUpdate();
    }
  }
}
