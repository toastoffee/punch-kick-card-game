using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToolListItemView : MonoBehaviour {
  [SerializeField]
  private string toolId;
  [SerializeField]
  private TMP_Text text;

  public void Start() {
    text.text = toolId;
  }

  public void EventOnClick() {
    TheFactoryGame.Instance.OnToolClick(toolId);
  }
}

