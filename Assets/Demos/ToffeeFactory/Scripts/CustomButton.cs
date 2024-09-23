using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ToffeeFactory {
  public class CustomButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    
    public Action clickHandler;
    
    [SerializeField]
    private float hoverSwellSize = 1.1f, pressedShrinkSize = 0.9f;
    
    [SerializeField]
    private float hoverSwellDuration = 0.1f, pressedShrinkDuration = 0.1f, recoverDuration = 0.1f;
    
    public void OnPointerClick(PointerEventData eventData) {
      if (eventData.button == PointerEventData.InputButton.Left) {
        clickHandler?.Invoke(); 
      }
      
      Sequence sequence = DOTween.Sequence();
      sequence.Append(transform.DOScale(pressedShrinkSize * Vector3.one, pressedShrinkDuration));
      sequence.Append(transform.DOScale( Vector3.one, hoverSwellDuration));
      sequence.Append(transform.DOScale(hoverSwellSize * Vector3.one, hoverSwellDuration));
    }
    public void OnPointerEnter(PointerEventData eventData) {
      transform.DOScale(hoverSwellSize * Vector3.one, hoverSwellDuration);
    }
    public void OnPointerExit(PointerEventData eventData) {
      transform.DOScale(Vector3.one, recoverDuration);
    }
  } 
}
