using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Line : MonoBehaviour {
  public Image[] lineImgs;
  public RectTransform lineImgTr;
  public RectTransform end;
  public RectTransform rectTransform;
  public CanvasGroup alpha;
  public float width;
  public float t1Time;
  public float t2Time;
  public float t3Time;

  private void Awake() {
    rectTransform = transform as RectTransform;
    Update();
  }

  public void Update() {
    var height = Vector2.Distance(lineImgTr.position, end.position);
    lineImgTr.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    lineImgTr.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    lineImgTr.up = end.position - lineImgTr.position;
  }

  public class ShootCtx {
    public Line line;
    public RectTransform target;
    public RectTransform origin;

    public RectTransform ctrlOrigin;
    public RectTransform ctrlEnd;

    public RectTransform hero;

    public float tweenValue;
    public float originT, endT;

    public float heroT;

    public void Tick() {
      line.rectTransform.position = origin.position;

      var originPos = Vector3.Lerp(origin.position, target.position, originT);
      var targetPos = Vector3.Lerp(origin.position, target.position, endT);

      ctrlOrigin.position = origin.position;
      ctrlEnd.position = targetPos;


      var heroPos = Vector3.Lerp(origin.position, target.position, heroT);
      hero.position = heroPos;
    }

    public void T1Setter(float value) {
      originT = 0;
      endT = value;
      line.alpha.alpha = value;
    }

    public void T2Setter(float value) {
      heroT = value;
      //originT = value;
      //endT = 1;
      //line.alpha.alpha = 1 - value;
    }

    public Tween GetShootTween() {
      var seq = DOTween.Sequence();
      var t1 = DOTween.To(() => 0f, T1Setter, 1, line.t1Time);
      seq.Append(t1);

      var t2 = DOTween.To(() => 0f, T2Setter, 1, line.t1Time);
      seq.Append(t2);

      var obj = line.gameObject;
      seq.OnComplete(() => { Destroy(obj); });
      seq.OnUpdate(Tick);
      seq.Goto(0, true);
      Tick();
      return seq;
    }
  }

  public class Param {
    public RectTransform origin;
    public RectTransform end;
    public RectTransform hero, heroStrike;
  }

  public void Shoot(object arg) {
    var param = arg as Param;

    var ctx = new ShootCtx() {
      line = this,
      target = param.end,
      origin = param.origin,
      ctrlOrigin = lineImgTr,
      ctrlEnd = end,
      hero = param.heroStrike,
    };

    ctx.GetShootTween();
  }
}
