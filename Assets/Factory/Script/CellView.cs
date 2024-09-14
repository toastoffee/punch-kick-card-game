using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellView : MonoBehaviour {
  public AutoLoader sprLoader;
  public Image cellSprImg;
  public GameObject debugPosObj;
  public TMP_Text debugPosX, debugPosY;

  private TheFactoryGame.Cell m_cell;
  private SeqNumChecker m_drawSeq;

  public void SetCell(TheFactoryGame.Cell cell) {
    m_cell = cell;
  }

  public void Update() {
    DebugShowPos();
    Draw();
  }

  private void DebugShowPos() {
    if (!m_cell.debugShowPos) {
      debugPosObj.SetActive(false);
    } else {
      debugPosObj.SetActive(true);
      debugPosX.text = m_cell.x.ToString();
      debugPosY.text = m_cell.y.ToString();
    }
  }

  private void Draw() {
    if (!m_drawSeq.Check(m_cell.drawSeq)) {
      return;
    }

    if (m_cell.inst == null) {
      cellSprImg.gameObject.SetActive(false);
      return;
    }

    cellSprImg.gameObject.SetActive(true);
    switch (m_cell.inst.model.modelId) {
      case "pipe":
        Draw_Pipe();
        break;
      default:
        Draw_Default();
        break;
    }
  }

  private void Draw_Pipe() {
    var pipeState = m_cell.inst.data.pipeState;
    int flag = pipeState.portFlag;
    if (flag == 0) {
      var spr = sprLoader.Load<Sprite>("pipe_iso");
      cellSprImg.sprite = spr;
    } else {
      switch (flag) {
        case 0b1010:
        case 0b0101:
        case 0b0001:
        case 0b0010:
        case 0b0100:
        case 0b1000:
          var strSpr = sprLoader.Load<Sprite>("pipe_str");
          cellSprImg.sprite = strSpr;
          switch (flag) {
            case 0b1010:
            case 0b0010:
            case 0b1000:
              cellSprImg.transform.rotation = Quaternion.identity;
              break;
            default:
              cellSprImg.transform.rotation = Quaternion.Euler(0, 0, 90);
              break;
          }
          break;
        default:
          var corSpr = sprLoader.Load<Sprite>("pipe_cor");
          cellSprImg.sprite = corSpr;
          switch (flag) {
            case 0b1100:
              cellSprImg.transform.rotation = Quaternion.identity;
              break;
            case 0b0110:
              cellSprImg.transform.rotation = Quaternion.Euler(0, 0, -90);
              break;
            case 0b0011:
              cellSprImg.transform.rotation = Quaternion.Euler(0, 0, -180);
              break;
            case 0b1001:
              cellSprImg.transform.rotation = Quaternion.Euler(0, 0, -270);
              break;
          }
          break;
      }
    }
  }

  private void Draw_Default() {
    cellSprImg.transform.rotation = Quaternion.identity;
    var spr = sprLoader.Load<Sprite>(m_cell.inst.model.sprId);
    cellSprImg.sprite = spr;
  }

  public void EventOnClick() {
    //Debug.Log($"[Cell Click] {m_cell.x} {m_cell.y}");
    TheFactoryGame.Instance.OnCellClick(m_cell);
  }
}
