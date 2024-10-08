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

    public List<StuffLoad> costs;

    public void Update() {
      nameText.text = buyName;

      string costStr = "";

      for (int i = 0; i < costs.Count; i++) {
        if (i != 0) {
          costStr += " ";
        }
        costStr += costs[i].count + "" + StuffQuery.GetRichText(costs[i].type);
      }
      priceText.text = costStr;
    }

    public void EventOnClick() {
      PlaceManager.Instance.StartBuyPlace(this);
    }
  }
}