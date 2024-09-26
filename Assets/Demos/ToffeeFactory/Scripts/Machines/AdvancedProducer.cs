using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace ToffeeFactory {
  public class AdvancedProducer : AdvancedMachine {
    
    public List<Port> inPorts;
    public List<Port> outPorts;

    [SerializeField]
    private StorageSet storageSet;

    [SerializeField]
    private FormulaFamily producerType;
    
    private ProduceFormula formula;

    [SerializeField]
    private bool useFixedFormula;

    [SerializeField]
    private ProduceFormula fixedFormula;

    private float produceCounter;
    private List<float> outPortCounters = new List<float>();

    [SerializeField]
    private LoadingBar loadingBar;

    [SerializeField]
    private TMP_Text equationText;

    [SerializeField]
    private Transform icon;

    [SerializeField]
    private CustomButton switchFormulaBtn;

    [SerializeField]
    private SelectorFolder formulaSelectorFolder;

    [SerializeField]
    private CustomButton formulaSelector;

    private void Start() {
      
      // set ports belonging
      foreach (var port in inPorts) {
        port.machineBelong = this;
      }
      foreach (var port in outPorts) {
        port.machineBelong = this;
      }

      InitializeFormulaSelector();
      
      // attach switch formula Btn
      switchFormulaBtn.clickHandler = () => { formulaSelectorFolder.Unfold(); };

      if (useFixedFormula) {
        SwitchFormula(fixedFormula);
      } else {
        SwitchFormula(FormulaLibrary.Instance.GetFormulasOfFamily(producerType)[0]); 
      }
    }

    private void InitializeFormulaSelector() {
      var familyFormulas = FormulaLibrary.Instance.GetFormulasOfFamily(producerType);

      foreach (var f in familyFormulas) {
        var selector = Instantiate(formulaSelector, formulaSelectorFolder.transform.position, Quaternion.identity);
        selector.transform.localScale = Vector3.zero;
        
        formulaSelectorFolder.AddUnit(selector.transform);
        
        selector.clickHandler = () => { SwitchFormula(f); formulaSelectorFolder.Fold(); };
        selector.GetComponentInChildren<TMP_Text>().text = f.formulaName;
      }
      
      formulaSelectorFolder.Fold();
    }
    
    private void SwitchFormula(ProduceFormula f) {
      if (formula == f) {
        return;
      }
      SetFormula(f);
    }
    
    private void SetFormula(ProduceFormula f) {
      formula = f;
      
      // set restrict storage set
      storageSet.SetStorageSize(inPorts.Count + outPorts.Count);
      var types = formula.GetStuffInvolved();
      for (int i = 0; i < types.Count; i++) {
        storageSet.SetStorageRestrict(i, types[i]);
      }
      
      // reset counters
      produceCounter = 0f;
      outPortCounters = new List<float>(outPorts.Count) { 0 };
      
      // update Equation Text
      equationText.text = FormulaLibrary.GetFormulaStr(f);
      
      // update port text
      for (int i = 0; i < outPorts.Count; i++) {
        outPorts[i].typeText.text = StuffQuery.GetRichText(f.products[i].type);
      }
    }

    private void Update() {
      if (formula != null) {
       
        // produce
        if (IsIngredientsSufficient() && IsStorageRemainForProducts()) {
          produceCounter += Time.deltaTime;
          loadingBar.SetBarState(produceCounter, formula.produceInterval);

          if (produceCounter > formula.produceInterval) {
            produceCounter = 0f;
            Produce();
          }
        } else {
          loadingBar.SetPaused();
        }
      
        // serve
        for (int i = 0; i < outPortCounters.Count; i++) {
          outPortCounters[i] += Time.deltaTime;
        
          // try transport ingredient
          if (outPortCounters[i] > pipeInterval) {
            StuffLoad supply = new StuffLoad(formula.products[i].type, 1);
            if (storageSet.IsSufficient(supply)) {
              if (outPorts[i].isConnected && outPorts[i].connectedPort.machineBelong.ReceiveStuffLoad(supply.Copy())) {
                storageSet.TryConsume(supply);
                outPortCounters[i] = 0f;
              } 
            }
          }
        }
        
        // fold formula selector
        if (Input.GetMouseButtonDown(1)) {
          formulaSelectorFolder.Fold();
        }
        
      }
    }

    private void Produce() {
      // 1.consume
      foreach (var ingredient in formula.ingredients) {
        storageSet.TryConsume(ingredient);
      }
      
      // 2.produce
      foreach (var product in formula.products) {
        storageSet.TryAdd(product);
      }
      
      ShakeIcon();
    }
    
    public override bool ReceiveStuffLoad(StuffLoad load) {
      if (storageSet.IsSpaceRemained(load)) {
        storageSet.TryAdd(load);
        return true;
      }
      return false;
    }
    
    private bool IsIngredientsSufficient() {
      foreach (var ingredient in formula.ingredients) {
        if (!storageSet.IsSufficient(ingredient)) {
          return false;
        }
      }
      return true;
    }
    
    private bool IsStorageRemainForProducts() {
      foreach (var product in formula.products) {
        if (storageSet.IsSpaceRemained(product)) {
          return true;
        }
      }
      return false;
    }
    
    private void ShakeIcon() {
      Sequence sequence = DOTween.Sequence();
      sequence.Append(icon.DOScale(new Vector3(1.3f, 0.8f, 1f), 0.1f));
      sequence.Append(icon.DOScale( new Vector3(0.8f, 1.3f, 1f), 0.1f));
      sequence.Append(icon.DOScale( new Vector3(1f, 1f, 1f), 0.1f));
    }
    
  } 
}
