using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour {
  public Line linePrefab;
  public RectTransform hero, strikeHero;
  public RectTransform enemy, strikeEnemy;
  public Transform canvasTransform;

  public void ShootBtn() {
    var param = new Line.Param {
      end = enemy,
      origin = hero,
      hero = hero,
      heroStrike = strikeHero,
    };
    var line = Instantiate(linePrefab, canvasTransform);
    line.BroadcastMessage(nameof(line.Shoot), param);
  }
}
