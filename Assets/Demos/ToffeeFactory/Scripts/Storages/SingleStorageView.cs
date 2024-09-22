using TMPro;
using UnityEngine;

namespace ToffeeFactory {
  public class SingleStorageView {

    [SerializeField]
    private TMP_Text typeText, countText;

    public void UpdateDisplay(SingleStorage storage) {
      typeText.text = storage.type;
      countText.text = $"{storage.count}/{storage.capacity}";
    }
    
  }
}