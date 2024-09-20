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
    private int money;

    private Tween moneyTextPunchTween;

    public void AddMoney(int add) {
      money += add;
      moneyText.text = money.ToString();
      if (moneyTextPunchTween != null) {
        moneyTextPunchTween.Kill(complete: true);
      }

      moneyTextPunchTween = moneyText.transform.DOPunchScale(0.15f * Vector3.one, 0.3f);
    }
  }
}