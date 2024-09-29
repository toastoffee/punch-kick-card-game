using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ToffeeFactory {

  public class MachineMenuStorageItem : MonoBehaviour {
    [SerializeField]
    private TMP_Text typeText, countText;
    public SingleStorage storage;

    public void Render(SingleStorage storage) {
      this.storage = storage;
      if (storage.typeRestrict) {
        typeText.text = StuffQuery.GetRichText(storage.type);
        countText.text = $"{storage.count}/{storage.capacity}";
      } else {
        if (storage.count > 0) {
          typeText.text = StuffQuery.GetRichText(storage.type);
          countText.text = $"{storage.count}/{storage.capacity}";
        } else {
          typeText.text = "无";
          countText.text = $"{storage.count}/{storage.capacity}";
        }
      }
    }

    public void Update() {
      if (storage == null) {
        return;
      }
      Render(storage);
    }
  }
}
