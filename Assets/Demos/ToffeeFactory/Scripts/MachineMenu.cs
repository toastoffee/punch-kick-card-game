using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

namespace ToffeeFactory {
  public class MachineMenu : MonoSingleton<MachineMenu>, GlobalClickReceiver.ICallBack {
    public struct Context {
      public GameObject machineObj;
      public SingleStorageView[] storageViews;
      public bool isRunning { get; private set; }
      public void Start(GameObject machineObj) {
        isRunning = true;
        this.machineObj = machineObj;
        Instance.m_showStateTween.GoToState(ShowState.SHOW);
        GlobalClickReceiver.Instance.AddListener(Instance.gameObject, 1);
        storageViews = machineObj.GetComponentsInChildren<SingleStorageView>();
      }
      public void Dispose() {
        if (!isRunning) {
          return;
        }
        isRunning = false;
      }
    }
    private enum ShowState {
      HIDE, SHOW
    }
    public CanvasGroup rootCanvasGroup;
    public TMP_Text nameText;

    private Context m_ctx;
    private float m_lastActiveTime;
    private const float FADE_LOCK = 0.05f;
    private UIStateTransitionTween<ShowState> m_showStateTween;

    public void Start() {
      m_showStateTween = new UIStateTransitionTween<ShowState>(ShowState.SHOW);
      m_showStateTween.SetTweenProvider(ShowState.SHOW, ShowState.HIDE, () => {
        var tween = rootCanvasGroup.DOFade(0, 0.2f);
        return tween;
      });
      m_showStateTween.SetTweenProvider(ShowState.HIDE, ShowState.SHOW, () => {
        var tween = rootCanvasGroup.DOFade(1, 0.2f);
        return tween;
      });
      m_showStateTween.GoToState(ShowState.HIDE);
    }
    public void Update() {
      if (!m_ctx.isRunning && Time.time - m_lastActiveTime > FADE_LOCK) {
        m_showStateTween.GoToState(ShowState.HIDE);
      }
      if (!m_ctx.isRunning) {
        return;
      }
      var machineObj = m_ctx.machineObj;
      nameText.text = machineObj.name;

      for (int i = 0, cnt = m_ctx.storageViews.SafeCount(); i < cnt; i++) {
        var sto = m_ctx.storageViews[i].storage;
      }
    }

    public void ShowMachine(GameObject machineObj) {
      m_ctx.Dispose();
      m_ctx = new Context();
      m_ctx.Start(machineObj);
    }

    public void OnGlobalClick(int buttonId) {
      if (buttonId == 1 && m_ctx.isRunning) {
        m_ctx.Dispose();
        m_ctx = new Context();
      }
    }
  }
}
