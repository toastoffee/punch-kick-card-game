using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace ToffeeFactory {
  public class AdvancedProducer : AdvancedMachine {
    
    public List<Port> inPorts;
    public List<Port> outPorts;

    [SerializeField]
    private StorageSet storageSet;

    [SerializeField]
    private FormulaFamily producerType;
    [SerializeField]
    private ProduceFormula formula;

    private float produceCounter;
    private List<float> outPortCounters;

    [SerializeField]
    private LoadingBar loadingBar;

    [SerializeField]
    private TMP_Text equationText;

    [SerializeField]
    private Transform icon;

    [SerializeField]
    private int testFormulaIdx;
    
    private void Start() {
      
      // set ports belonging
      foreach (var port in inPorts) {
        port.machineBelong = this;
      }
      foreach (var port in outPorts) {
        port.machineBelong = this;
      }

      SetFormula(FormulaLibrary.Instance.GetFormulasOfFamily(producerType)[testFormulaIdx]);
      
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
    }

    private void Update() {

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
      for (int i = 0; i < outPorts.Count; i++) {
        outPortCounters[i] += Time.deltaTime;
        
        // try transport ingredient
        if (outPortCounters[i] > pipeInterval) {
          StuffLoad supply = new StuffLoad(formula.products[i].type, 1);
          if (storageSet.IsSufficient(supply)) {
            if (outPorts[i].isConnected && outPorts[i].connectedPort.machineBelong.ReceiveStuffLoad(supply.Copy())) {
              storageSet.TryConsume(supply);
              outPortCounters[i] = 0f;
              
              //!TODO SHAKE SHACK 
            } 
          }
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
    
  } 
}
