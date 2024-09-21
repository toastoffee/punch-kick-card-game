using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace ToffeeFactory {
  public class LoadingBar : MonoBehaviour {
    [SerializeField]
    private TMP_Text timeText;

    [SerializeField]
    private Transform solid;

    public void SetPaused() {
      timeText.text = "生产中止";
      solid.transform.localScale = new Vector3(0f, 1f, 1f);
    }
  
    public void SetBarState(float currentDuration, float totalDuration) {
      float ratio = currentDuration / totalDuration;
      solid.transform.localScale = new Vector3(ratio, 1f, 1f);

      timeText.text = String.Format("{0:N1}s", currentDuration);
    }
  
  } 
}
