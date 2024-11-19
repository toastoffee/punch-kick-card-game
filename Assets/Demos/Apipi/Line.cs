using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Line : MonoBehaviour {
  public Image[] lineImgs;
  public RectTransform lineImgTr;
  public Transform end;
  public float width;

  public void Update() {
    var height = Vector2.Distance(transform.position, end.position);
    lineImgTr.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    lineImgTr.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    lineImgTr.up = end.position - transform.position;
  }

  public Tween ShootTween() {
    return null;
  }
}
