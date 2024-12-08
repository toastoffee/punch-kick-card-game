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
      public IProduceFormulaHodler formulaHolder;
      public List<ProduceFormula> formulaOptions;
      public SeqNumHolder seqNumHolder;
      private SeqNumChecker m_infoSeqChecker;

      public bool isRunning { get; private set; }
      public void Start(GameObject machineObj) {
        isRunning = true;
        this.machineObj = machineObj;
        seqNumHolder = machineObj.GetComponentInChildren<SeqNumHolder>();
        Instance.m_showStateTween.GoToState(ShowState.SHOW);

        RenderMachineInfo(machineObj);
      }

      private void RenderMachineInfo(GameObject machineObj) {
        RenderStorage();
        var formulaHolder = machineObj.GetComponentInChildren<IProduceFormulaHodler>();

        Instance.formulaGroupRoot.gameObject.SetActive(formulaHolder != null);
        this.formulaHolder = formulaHolder;

        if (formulaHolder != null) {
          formulaOptions = new List<ProduceFormula>();
          var optionHolders = machineObj.GetComponentsInChildren<FormulaOptionHolder>();
          foreach (var holder in optionHolders) {
            formulaOptions.Add(holder.formula);
          }
          RenderFormulaOptions();
        }
      }

      public void Update() {
        if (formulaHolder == null) {
          return;
        }
        var formula = formulaHolder.GetProduceFormula();
        if (formula == null) {
          Instance.formulaNameText.text = Instance.formulaContentText.text = string.Empty;
        } else {
          Instance.formulaNameText.text = formula.formulaName;
          Instance.formulaContentText.text = FormulaLibrary.GetFormulaStr(formula);
        }
        if (m_infoSeqChecker.ConsumeUpdate(seqNumHolder.Read("info"))) {
          RenderMachineInfo(machineObj);
        }
      }

      public void Dispose() {
        if (!isRunning) {
          return;
        }
        isRunning = false;
      }

      public void OnFormulaOptionClick(ProduceFormula formula) {
        formulaHolder.SetProduceFormula(formula);
      }

      private void RenderStorage() {
        var storageViews = machineObj.GetComponentsInChildren<SingleStorageView>();
        Instance.storageContainer.DestroyAllChildren();

        var cnt = storageViews.Length;
        for (int i = 0; i < cnt; i++) {
          var src = storageViews[i];
          var itemView = Instantiate(Instance.storageItemPrefab, Instance.storageContainer);
          itemView.Render(src.storage);
        }
      }

      private void RenderFormulaOptions() {
        Instance.formulaContainer.DestroyAllChildren();
        foreach (var formula in formulaOptions) {
          var optionView = Instantiate(Instance.formulaItemPrefab, Instance.formulaContainer);
          optionView.Render(formula);
        }
      }
    }
    private enum ShowState {
      HIDE, SHOW
    }
    public CanvasGroup rootCanvasGroup;
    public TMP_Text nameText;
    public MachineMenuStorageItem storageItemPrefab;
    public RectTransform storageContainer;
    public RectTransform formulaGroupRoot;
    public TMP_Text formulaNameText;
    public TMP_Text formulaContentText;
    public MachineMenuFormulaOptionItem formulaItemPrefab;
    public RectTransform formulaContainer;

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
      if (m_ctx.isRunning && Input.GetMouseButton(1)) {
        m_ctx.Dispose();
        m_ctx = new Context();
      }
      if (!m_ctx.isRunning && Time.time - m_lastActiveTime > FADE_LOCK) {
        m_showStateTween.GoToState(ShowState.HIDE);
      }
      if (!m_ctx.isRunning) {
        return;
      }
      var machineObj = m_ctx.machineObj;
      nameText.text = machineObj.name;

      m_ctx.Update();
    }

    public void ShowMachine(GameObject machineObj) {
      m_ctx.Dispose();
      m_ctx = new Context();
      m_ctx.Start(machineObj);
    }

    public void OnGlobalClick(int buttonId) {
      if (m_ctx.isRunning) {
        m_ctx.Dispose();
        m_ctx = new Context();
      }
    }

    public void OnFormulaClick(ProduceFormula formula) {
      if (m_ctx.isRunning) {
        m_ctx.OnFormulaOptionClick(formula);
      }
    }
  }

}
