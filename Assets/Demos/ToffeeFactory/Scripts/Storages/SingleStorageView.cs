using TMPro;
using UnityEngine;

namespace ToffeeFactory {
  public class SingleStorageView {

    [SerializeField]
    private TMP_Text typeText, countText;

    public void UpdateDisplay(SingleStorage storage) {

      if (storage.typeRestrict) {
        typeText.text = storage.type;
        countText.text = $"{storage.count}/{storage.capacity}"; 
      } else {
        typeText.text = "";
        countText.text = "";
      }
    }
    
  }
}