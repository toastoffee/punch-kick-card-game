using UnityEngine;
using System.Collections.Generic;

namespace ToffeeFactory {
  public abstract class AdvancedMachine : MonoBehaviour {
    public static float pipeInterval = 0.5f;
    private SeqNumHolder seqNumHolder;

    public abstract bool ReceiveStuffLoad(StuffLoad load);

    public abstract IEnumerable<Port> GetAllPorts();

    protected virtual void Awake() {
      seqNumHolder = gameObject.AddComponent(typeof(SeqNumHolder)) as SeqNumHolder;
    }

    protected void NotifyMachineSeqNum(string id) {
      seqNumHolder.NotifyUpdate(id);
    }
  }

  public interface IProduceFormulaHodler {
    ProduceFormula GetProduceFormula();
    void SetProduceFormula(ProduceFormula formula);
  }
}