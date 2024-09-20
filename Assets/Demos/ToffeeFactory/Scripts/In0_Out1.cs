
using System;
using TMPro;
using UnityEngine;

namespace ToffeeFactory {
  public class In0_Out1 : Machine{
    // only got one out
    public Port outPort;

    [SerializeField]
    private string mineType;

    [SerializeField]
    private TMP_Text outContainerText;

    [SerializeField]
    private int maxOutContainer;

    private int outContain = 0;

    [SerializeField]
    private float produceInterval;

    private float produceCounter = 0;

    [SerializeField]
    private float pipeInterval;

    private float pipeCounter = 0;
    
    private void Start() {
      outPort.affiliated = this;

      produceCounter = 0f;
      pipeCounter = 0f;
      
      outContain = 0;
    }

    public override bool ReceiveIngredient(Ingredient ingredient) {
      // no receive
      return false;
    }

    public void Update() {
      produceCounter += Time.deltaTime;
      pipeCounter += Time.deltaTime;
      
      if (produceCounter > produceInterval && outContain < 10) {
        produceCounter = 0;
        outContain += 1;
      }
      
      // check if downstream needed
      if (outPort.isConnected) {
        
      }
    }
  }
}