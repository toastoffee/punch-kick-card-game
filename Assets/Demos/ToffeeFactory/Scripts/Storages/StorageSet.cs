

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
      for (int i = _storageViews.Count-1; i < size; i++) {
        var newView = Instantiate(viewPrefab, NextViewPos(i), Quaternion.identity);
        _storageViews.Add(newView);
      }
    }

    private Vector3 NextViewPos(int idx) {
      return transform.position + idx * viewInterval * Vector3.right;
    }
    
    public void Clear() {
      foreach (var storage in _storages) {
        storage.Clear();
      }
    }
    


  }
}