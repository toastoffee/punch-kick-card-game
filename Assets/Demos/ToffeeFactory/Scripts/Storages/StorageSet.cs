

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToffeeFactory {
  public class StorageSet : MonoBehaviour {
    

    private List<SingleStorage> _storages = new List<SingleStorage>();
    
    [SerializeField]
    
    
    public void SetStorageSize(int storageNum) {
      
      // 1. new storages space
      _storages = new List<SingleStorage>();
      for (int i = 0; i < storageNum; i++) {
        _storages.Add(new SingleStorage(64));
      }
      
      // 2. initialize storage blocks
      
    }
    

    public void Clear() {
      foreach (var storage in _storages) {
        storage.Clear();
      }
    }
    


  }
}