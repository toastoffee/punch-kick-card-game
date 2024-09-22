using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace ToffeeFactory {
  public class Status : MonoSingleton<Status> {
    [SerializeField]
    private TMP_Text moneyText;
    public int money;

    private Tween moneyTextPunchTween;

    public void AddMoney(int add) {
      money += add;
      if (moneyTextPunchTween != null) {
        moneyTextPunchTween.Kill(complete: true);
      }

      moneyTextPunchTween = moneyText.transform.DOPunchScale(0.15f * Vector3.one, 0.3f);
    }

    public bool CanAfford(int cost) => cost <= money;

    public void RemoveMoney(int cost) {
      AddMoney(-cost);
    }
    public void Update() {
      moneyText.text = money.ToString();
    }
  }
}