using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace ToffeeFactory {
  public class AdvancedContainer : AdvancedMachine{

    public List<Port> inPorts;
    public List<Port> outPorts;

    [SerializeField]
    private StorageSet storageSet;
    
    private List<float> outPortCounters = new List<float>();
    
    [SerializeField]
    private Transform icon;

    private StuffType containType = StuffType.NONE;

    [SerializeField]
    private int storageCounts;
    
    [SerializeField]
    private CustomButton clearBtn;
    
    private void Start() {
      // set ports belonging
      foreach (var port in inPorts) {
        port.machineBelong = this;
      }
      foreach (var port in outPorts) {
        port.machineBelong = this;
      }
      
      // set storages
      storageSet.SetStorageSize(storageCounts);
      
      // reset counters
      outPortCounters = new List<float>(outPorts.Count) { 0 };
      
      // attach switch formula Btn
      clearBtn.clickHandler = () => { ClearContainer(); };
    }
    
    private void Update() {
      if (containType != StuffType.NONE) {
      
        // serve
        for (int i = 0; i < outPortCounters.Count; i++) {
          outPortCounters[i] += Time.deltaTime;
        
          // try transport ingredient
          if (outPortCounters[i] > pipeInterval) {
            StuffLoad supply = new StuffLoad(containType, 1);
            if (storageSet.IsSufficient(supply)) {
              if (outPorts[i].isConnected && outPorts[i].connectedPort.machineBelong.ReceiveStuffLoad(supply.Copy())) {
                storageSet.TryConsume(supply);
                outPortCounters[i] = 0f;
              } 
            }
          }
        }
        
      }
    }

    private void ClearContainer() {
      for (int i = 0; i < storageCounts; i++) {
        storageSet.UnlockStorage(i);
      }
      
      storageSet.Clear();
      containType = StuffType.NONE;
      
      // clear port text
      for (int i = 0; i < outPorts.Count; i++) {
        outPorts[i].typeText.text = "";
      }
      
      // shake icon
      ShakeIcon();
    }
    
    private void DetermineContainType(StuffType type) {
      containType = type;
      
      for (int i = 0; i < storageCounts; i++) {
        storageSet.SetStorageRestrict(i, type);
      }
      
      // update port text
      for (int i = 0; i < outPorts.Count; i++) {
        outPorts[i].typeText.text = StuffQuery.GetRichText(type);
      }
    }
    
    public override bool ReceiveStuffLoad(StuffLoad load) {
      if (containType == StuffType.NONE) {
        DetermineContainType(load.type);
        
        storageSet.TryAdd(load);
        return true;
      }
      else if (containType == load.type && storageSet.IsSpaceRemained(load)) {
        storageSet.TryAdd(load);
        return true;
      } else {
        return false;
      }
    }
    
    private void ShakeIcon() {
      Sequence sequence = DOTween.Sequence();
      sequence.Append(icon.DOScale(new Vector3(1.3f, 0.8f, 1f), 0.1f));
      sequence.Append(icon.DOScale( new Vector3(0.8f, 1.3f, 1f), 0.1f));
      sequence.Append(icon.DOScale( new Vector3(1f, 1f, 1f), 0.1f));
    }
    
  }
}