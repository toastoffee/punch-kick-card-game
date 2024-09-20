using System;
using System.Collections.Generic;
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
    }

    private void Update() {
      
      // produce own product
      if (!IsStorageFull() && CheckIngredientReady()) {
        produceCounter += Time.deltaTime;

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
          }
        }
      }
      
    }

    public override bool ReceiveIngredient(Ingredient ingredient) {
      foreach (var sto in inContains) {
        if (sto.TryAdd(ingredient)) {
          return true;
        }
      }
      return false;
    }
  }
}