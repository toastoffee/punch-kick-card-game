using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ToffeeFactory {
  public class BuyListItem : MonoBehaviour {
    public PlaceAnchor placingPrefab;
    public int price;
    public string buyName;

    public TMP_Text nameText, priceText;

    public void Update() {
      nameText.text = buyName;
      priceText.text = price.ToString();
    }

    public void EventOnClick() {
      PlaceManager.Instance.StartPlace(this);
    }
  }
}