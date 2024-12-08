using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnGame {
  using Text = TMPro.TMP_Text;
  public class CentralHintUI : MonoSingleton<CentralHintUI> {
    private class ControlLock {
      public bool isActive;
    }
    public class Control {
      private ControlLock m_lock;
      public bool isActive => m_lock.isActive;
      public Control() {
        m_lock = Instance._GetLock();
      }
      public void Close() {
        m_lock.isActive = false;
      }
      public void ShowHint(Hint hint, params object[] formatParams) {
        var format = Instance.hintFormatTable[hint];
        var hintStr = string.Format(format, formatParams);
        Instance.hintText.text = hintStr;
      }
    }
    public enum Hint {
      SELECTING_TARGET,
    }
    private class HintFormatTable : TemplateTable<Hint, string> {
      protected override void OnCreateTable(Dictionary<Hint, string> dict) {
        dict[Hint.SELECTING_TARGET] = "{0}: Ñ¡ÔñÄ¿±ê";
      }
    }
    private Control m_control;
    private HintFormatTable hintFormatTable = new HintFormatTable();

    public GameObject hintObj;
    public Text hintText;

    private void Update() {
      if (m_control == null || !m_control.isActive) {
        hintObj.SetActive(false);
      } else {
        hintObj.SetActive(true);
      }
    }

    private ControlLock _GetLock() {
      return new ControlLock() { isActive = true };
    }

    public Control GetControl() {
      hintText.text = "";
      var control = new Control();
      m_control = control;
      return control;
    }
  }
}
