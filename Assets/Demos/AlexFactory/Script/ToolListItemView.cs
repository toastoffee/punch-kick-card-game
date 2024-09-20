using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AlexFactory {
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

    public void ChangeMachineToolId()
    {
      switch (toolId)
      {
        case "machine-left-right":
          toolId = "machine-up-down";
          break;
        case "machine-up-down":
          toolId = "machine-right-left";
          break;
        case "machine-right-left":
          toolId = "machine-down-up";
          break;
        case "machine-down-up":
          toolId = "machine-left-right";
          break;
      }
      text.text = toolId;
      TheFactoryGame.Instance.OnToolClick(toolId);
    }
  }

}
