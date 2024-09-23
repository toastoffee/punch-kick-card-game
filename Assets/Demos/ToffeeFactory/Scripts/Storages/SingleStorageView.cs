using TMPro;
using UnityEngine;
using DG.Tweening;

namespace ToffeeFactory {
  public class SingleStorageView : MonoBehaviour {

    [SerializeField]
    private TMP_Text typeText, countText;

    public void UpdateDisplay(SingleStorage storage) {

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

    public void ShakeCountText() {
      Sequence sequence = DOTween.Sequence();
      sequence.Append(countText.transform.DOScale(new Vector3(1.3f, 0.8f, 1f), 0.1f));
      sequence.Append(countText.transform.DOScale( new Vector3(0.8f, 1.3f, 1f), 0.1f));
      sequence.Append(countText.transform.DOScale( new Vector3(1f, 1f, 1f), 0.1f));      
    }
    
  }
}