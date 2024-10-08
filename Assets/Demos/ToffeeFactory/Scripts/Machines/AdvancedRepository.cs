using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToffeeFactory {
  public class AdvancedRepository : AdvancedMachine, IProduceFormulaHodler {
    public List<Port> inPorts;
    
    public StuffType _stuffType = StuffType.NONE;
    
    private static ResourcesLoadCache<FormulaOptionHolder> holderPrefabCache = new ResourcesLoadCache<FormulaOptionHolder>("Prefabs/formula_holder");

    private void Start() {
      foreach (var port in inPorts) {
        port.machineBelong = this;
      }

      foreach (var type in RepositoryStorageSet.Instance._stuffTypes) {
        ProduceFormula f = new ProduceFormula();
        f.storeType = type;
        f.formulaName = "储存" + StuffQuery.GetRichText(type);
        
        var holder = Instantiate(holderPrefabCache.Res, transform);
        holder.formula = f;

      }
      
      _stuffType = RepositoryStorageSet.Instance._stuffTypes[0];
      
      RepositoryStorageSet.Instance.Register(this);
    }

    private void OnDestroy() {
      if (RepositoryStorageSet.Instance != null) {
        RepositoryStorageSet.Instance.Unregister(this); 
      }
    }

    public override bool ReceiveStuffLoad(StuffLoad load) {
      if (load.type == _stuffType && RepositoryStorageSet.Instance.IsSpaceRemained(load)) {
        RepositoryStorageSet.Instance.TryAdd(load);
        return true;
      }
      return false;
    }
    
    
    public override IEnumerable<Port> GetAllPorts() {
      return inPorts;
    }
    public ProduceFormula GetProduceFormula() {
      ProduceFormula formula = new ProduceFormula();
      formula.formulaName = "存储" + StuffQuery.GetRichText(_stuffType);

      return formula;
    }
    public void SetProduceFormula(ProduceFormula formula) {
      Debug.Log(formula.formulaName);
      if (_stuffType != formula.storeType) {
        _stuffType = formula.storeType;
        RepositoryStorageSet.Instance.UpdateTypes();
      }
    }
  }
}