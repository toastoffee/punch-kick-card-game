using System.Collections.Generic;
using UnityEngine;

namespace ToffeeFactory {
  public class CoreStorageSet : MonoSingleton<CoreStorageSet> {

    [SerializeField]
    private List<SingleStorage> storages;

    public void TryAdd(StuffLoad load) {
      var copy = load.Copy();

      for (int i = 0; i < storages.Count; i++) {
        bool isChanged = storages[i].TryAdd(copy);
      }
    }

    public void TryConsume(StuffLoad load) {
      var copy = load.Copy();
      
      for (int i = storages.Count-1; i >= 0; i--) {
        bool isChanged = storages[i].TryConsume(copy);
      }
    }

    
    public bool IsSufficient(StuffLoad load) {
      var copy = load.Copy();
      
      foreach (var storage in storages) {
        storage.TryProvide(copy);
      }
      
      if (copy.count == 0) {
        return true;
      } else {
        return false;
      }
    }

    public bool IsSpaceRemained(StuffLoad load) {
      var copy = load.Copy();
      
      foreach (var storage in storages) {
        storage.TryContain(copy);
      }

      if (copy.count != load.count) {
        return true;
      } else {
        return false;
      }
    }
    
  }
}