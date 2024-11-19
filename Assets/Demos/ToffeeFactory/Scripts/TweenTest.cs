using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TweenTest : MonoBehaviour {
  // Start is called before the first frame update
  void Start() {
    var tweenA = GetTween("A");
    var tweenB = GetTween("B");
    var seq = DOTween.Sequence();
    seq.Insert(0.5f, tweenB);
    seq.Insert(0, tweenA);
    //seq.Append(tweenA).Append(tweenB);
    seq.Complete();

    //var tweenC = GetTween("C");
    var seqB = DOTween.Sequence();
    seqB.InsertCallback(seqB.Duration(), () => { Debug.Log("seqB callback"); });
    //seqB.Goto(seqB.Duration() - 0.001f, andPlay: true);

  }

  // Update is called once per frame
  void Update() {

  }

  private Tween GetTween(string id) {
    float x = 0;
    return DOTween.To(() => x, (val) => {
      x = val;
      Debug.Log($"{id} setting {val} in frame {Time.frameCount}");
    }, 1, 1);
  }
}
