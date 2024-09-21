using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace ToffeeFactory {
  
  public class ProducerMachine : Machine {

    public List<Port> inPorts;

    public List<Port> outPorts;

    public List<Storage> inContains;
    
    public List<Storage> outContains;
    
    // define the formula
    public float produceInterval;
    public List<Ingredient> ingredients;
    public List<Ingredient> produces;

    private float produceCounter;
    private List<float> pipeCounter;

    [SerializeField]
    private LoadingBar loadingBar;

    [SerializeField]
    private TMP_Text equationText;

    [SerializeField]
    private Transform icon;

    private void UpdateContainerTexts() {
      for (int i = 0; i < inContains.Count; i++) {
        inPorts[i].typeText.text = IngredientQuery.Instance.GetRichText(inContains[i].name);
        inPorts[i].countText.text = $"{inContains[i].count}/{inContains[i].max}";
      }
      for (int i = 0; i < outContains.Count; i++) {
        outPorts[i].typeText.text = IngredientQuery.Instance.GetRichText(outContains[i].name);
        outPorts[i].countText.text = $"{outContains[i].count}/{outContains[i].max}";
      }

    }
    
    private bool CheckIngredientReady() {
      foreach (var ingredient in ingredients) {
        if (!CheckIngredient(ingredient)) {
          return false;
        }
      }
      return true;
    }

    private bool CheckIngredient(Ingredient load) {
      foreach (var sto in inContains) {
        if (sto.isSufficient(load)) {
          return true;
        }
      }
      return false;
    }

    private void ConsumeIngredient(Ingredient load) {
      foreach (var sto in inContains) {
        sto.TryConsume(load);
      }
    }
    
    private void AddIngredient(Ingredient load) {
      foreach (var sto in outContains) {
        sto.TryAdd(load);
      }
    }

    private bool IsStorageFull() {
      foreach (var sto in outContains) {
        if (!sto.isFull) {
          return false;
        }
      }
      return true;
    }
    
    private void Produce() {
      // consume
      foreach (var load in ingredients) {
        ConsumeIngredient(load);
      }
      
      // produce
      foreach (var load in produces) {
        AddIngredient(load);
      }
      
      IconShake();
      InContainerTextShake();
      OutContainerTextShake();
    }


    private void IconShake() {
      Sequence sequence = DOTween.Sequence();
      sequence.Append(icon.DOScale(new Vector3(1.3f, 0.8f, 1f), 0.1f));
      sequence.Append(icon.DOScale( new Vector3(0.8f, 1.3f, 1f), 0.1f));
      sequence.Append(icon.DOScale( new Vector3(1f, 1f, 1f), 0.1f));
    }

    private void InContainerTextShake() {
      for (int i = 0; i < inContains.Count; i++) {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(inPorts[i].countText.transform.DOScale(new Vector3(1.2f, 0.9f, 1f), 0.08f));
        sequence.Append(inPorts[i].countText.transform.DOScale( new Vector3(0.9f, 1.2f, 1f), 0.08f));
        sequence.Append(inPorts[i].countText.transform.DOScale( new Vector3(1f, 1f, 1f), 0.08f));
      }
    }
    
    private void OutContainerTextShake() {
      for (int i = 0; i < outContains.Count; i++) {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(outPorts[i].countText.transform.DOScale(new Vector3(1.2f, 0.9f, 1f), 0.08f));
        sequence.Append(outPorts[i].countText.transform.DOScale( new Vector3(0.9f, 1.2f, 1f), 0.08f));
        sequence.Append(outPorts[i].countText.transform.DOScale( new Vector3(1f, 1f, 1f), 0.08f));
      }
    }

    private void UpdateEquationText() {
      string equation = "";
      for (int i = 0; i < ingredients.Count; i++) {
        equation += $"{ingredients[i].count} {IngredientQuery.Instance.GetRichText(ingredients[i].name)} ";

        if (i != ingredients.Count - 1) {
          equation += "+ ";
        }
      }
      equation += "= ";
      
      for (int i = 0; i < produces.Count; i++) {
        equation += $"{produces[i].count} {IngredientQuery.Instance.GetRichText(produces[i].name)} ";

        if (i != produces.Count - 1) {
          equation += "+ ";
        }
      }

      equationText.text = equation;
    }
    
    private void Start() {
      foreach (var port in inPorts) {
        port.affiliated = this;
      }
      foreach (var port in outPorts) {
        port.affiliated = this;
      }
      foreach (var sto in inContains) {
        sto.count = 0;
      }
      foreach (var sto in outContains) {
        sto.count = 0;
      }
      
      produceCounter = 0;
      pipeCounter = new List<float>(outPorts.Count){0};
      
      UpdateEquationText();
    }

    private void Update() {
      
      UpdateContainerTexts();

      loadingBar.SetPaused();
      // produce own product
      if (!IsStorageFull() && CheckIngredientReady()) {
        produceCounter += Time.deltaTime;
        loadingBar.SetBarState(produceCounter, produceInterval);

        if (produceCounter > produceInterval) {
          produceCounter = 0f;
          
          Produce();
        }
      } 
      
      // forms all down-stream machines
      for (int i = 0; i < outPorts.Count; i++) {

        pipeCounter[i] += Time.deltaTime;

        // try transport ingredient
        if (pipeCounter[i] > pipeInterval && outContains[i].count > 0) {
          Ingredient load = new Ingredient() {
            name = outContains[i].name,
            count = 1,
          };
          if (outPorts[i].isConnected && outPorts[i].connectedPort.affiliated.ReceiveIngredient(load)) {
            outContains[i].count -= 1;
            pipeCounter[i] = 0f;
            OutContainerTextShake();
          }
        }
      }
      
    }
    
    public override bool ReceiveIngredient(Ingredient ingredient) {
      foreach (var sto in inContains) {
        if (sto.TryAdd(ingredient)) {
          InContainerTextShake();
          return true;
        }
      }
      return false;
    }
  }
}