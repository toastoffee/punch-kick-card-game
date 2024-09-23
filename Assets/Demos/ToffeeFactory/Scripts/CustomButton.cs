using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ToffeeFactory {
  public class CustomButton : MonoBehaviour, IPointerClickHandler {
    
    public Action clickHandler;
    
    public void OnPointerClick(PointerEventData eventData) {
      clickHandler?.Invoke();
    }
  } 
}
