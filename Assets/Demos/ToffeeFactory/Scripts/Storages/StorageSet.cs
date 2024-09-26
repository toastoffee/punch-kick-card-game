using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToffeeFactory {
  public class StorageSet : MonoBehaviour {
    
    private int _storageCount;
    private List<SingleStorage> _storages = new List<SingleStorage>();
    private List<SingleStorageView> _storageViews = new List<SingleStorageView>();
    
    [SerializeField]
    private SingleStorageView viewPrefab;

    [SerializeField]
    private float viewInterval;
    
    public void SetStorageSize(int storageNum) {
      _storageCount = storageNum;
      
      // 1. new storages space
      _storages = new List<SingleStorage>();
      for (int i = 0; i < storageNum; i++) {
        _storages.Add(new SingleStorage(64));
      }
      
      // 2. initialize storage views
      SetStorageViews(storageNum);
      
      // 3. update views
      UpdateViews();
    }

    public void SetStorageRestrict(int idx, StuffType type) {
      _storages[idx].SetRestrictType(type);
      
      UpdateViews();
    }

    public void UnlockStorage(int idx) {
      _storages[idx].UnlockRestrictType();
      
      UpdateViews();
    }
    
    public void UpdateViews() {
      for (int i = 0; i < _storageCount; i++) {
        _storageViews[i].UpdateDisplay(_storages[i]);
      }
    }
    
    private void SetStorageViews(int size) {
      
      // destroy over-count views
      for (int i = _storageViews.Count-1; i >= size; i++) {
        var last = _storageViews[i];
        _storageViews.RemoveAt(i);
        Destroy(last.gameObject);
      }
      
      // add under-count views
      for (int i = _storageViews.Count; i < size; i++) {
        var newView = Instantiate(viewPrefab, NextViewPos(i, size), Quaternion.identity);
        _storageViews.Add(newView);
        newView.transform.SetParent(transform);
      }
    }

    private Vector3 NextViewPos(int idx, int total) {
      float offset = (total - 1) / -2f;
      
      return transform.position + (idx + offset) * viewInterval * Vector3.right;
    }
    

    public void TryAdd(StuffLoad load) {
      var copy = load.Copy();

      for (int i = 0; i < _storages.Count; i++) {
        bool isChanged = _storages[i].TryAdd(copy);
        if (isChanged) {
          // effect 
          _storageViews[i].ShakeCountText();
        }
      }
      
      UpdateViews();
    }

    public void TryConsume(StuffLoad load) {
      var copy = load.Copy();
      
      for (int i = _storages.Count-1; i >= 0; i--) {
        bool isChanged = _storages[i].TryConsume(copy);
        if (isChanged) {
          // effect 
          _storageViews[i].ShakeCountText();
        }
      }
      
      UpdateViews();
    }

    
    public bool IsSufficient(StuffLoad load) {
      var copy = load.Copy();
      
      foreach (var storage in _storages) {
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
      
      foreach (var storage in _storages) {
        storage.TryContain(copy);
      }

      if (copy.count != load.count) {
        return true;
      } else {
        return false;
      }
    }
    
    public void Clear() {
      foreach (var storage in _storages) {
        storage.Clear();
      }
      UpdateViews();
    }
    


  }
}