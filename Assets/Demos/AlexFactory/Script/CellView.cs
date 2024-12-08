using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AlexFactory {
  
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
      if (!m_drawSeq.ConsumeUpdate(m_cell.drawSeq)) {
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

    private void Draw_Machine() {
      var spr = sprLoader.Load<Sprite>("machine_str");
    }

    private void Draw_Pipe() {

    }

    private void Draw_Default() {
      cellSprImg.transform.rotation = Quaternion.identity;
      var spr = sprLoader.Load<Sprite>(m_cell.inst.model.sprId);
      cellSprImg.sprite = spr;
      var rot = Quaternion.identity;
      switch (m_cell.inst.rot) {
        case TheFactoryGame.InstRot.UP:
          break;
        case TheFactoryGame.InstRot.RIGHT:
          rot = Quaternion.Euler(0, 0, 90);
          break;
        case TheFactoryGame.InstRot.DOWN:
          rot = Quaternion.Euler(0, 0, 180);
          break;
        case TheFactoryGame.InstRot.LEFT:
          rot = Quaternion.Euler(0, 0, 270);
          break;
      }
      cellSprImg.transform.rotation = rot;
    }

    public void EventOnClick() {
      //Debug.Log($"[Cell Click] {m_cell.x} {m_cell.y}");
      TheFactoryGame.Instance.OnCellClick(m_cell);
    }
  }

}