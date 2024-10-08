using System;
using System.Collections.Generic;
using UnityEngine;


namespace ToffeeFactory {
  public class RepositoryStorageSet : MonoSingleton<RepositoryStorageSet> {
    [Serializable]
    public struct StuffConfig {
      public StuffType type;
      public int initLimit;
      public int initCnt;
    }
    [SerializeField]
    public List<StuffConfig> _stuffTypes = new List<StuffConfig>();

    [SerializeField]
    private int singleRepositorySpace;

    private List<SingleStorage> storages;

    [Header("所有注册的仓库")]
    private List<AdvancedRepository> _repos = new List<AdvancedRepository>();

    protected override void Awake() {
      base.Awake();

      storages = new List<SingleStorage>();

      foreach (var config in _stuffTypes) {
        var storage = new SingleStorage(0);
        storage.SetRestrictType(config.type);
        storages.Add(storage);
      }
      UpdateRepositoryLimit();
    }

    private void Start() {
      foreach (var config in _stuffTypes) {
        TryAdd(new StuffLoad(config.type, config.initCnt));
      }
    }

    public void Register(AdvancedRepository repo) {
      _repos.Add(repo);

      UpdateRepositoryLimit();
    }

    public void Unregister(AdvancedRepository repo) {
      _repos.Remove(repo);

      UpdateRepositoryLimit();
    }

    public void UpdateTypes() {
      UpdateRepositoryLimit();
    }

    private void UpdateRepositoryLimit() {
      foreach (var sto in storages) {

        int capacity = 0;
        foreach (var repo in _repos) {
          if (repo._stuffType == sto.type) {
            capacity += singleRepositorySpace;
          }
        }

        var config = _stuffTypes.Find(x => x.type == sto.type);

        sto._capacity = Mathf.Max(capacity, config.initLimit);

        // cut off
        sto._count = Mathf.Min(sto.count, sto._capacity);
      }
    }

    public void TryAdd(StuffLoad load) {
      var copy = load.Copy();

      for (int i = 0; i < storages.Count; i++) {
        bool isChanged = storages[i].TryAdd(copy);
      }
    }

    public void TryConsume(StuffLoad load) {
      var copy = load.Copy();

      for (int i = storages.Count - 1; i >= 0; i--) {
        bool isChanged = storages[i].TryConsume(copy);
      }
    }

    public void TryConsume(List<StuffLoad> loads) {
      foreach (var load in loads) {
        TryConsume(load);
      }
    }

    public bool IsSufficient(List<StuffLoad> loads) {
      foreach (var load in loads) {
        if (!IsSufficient(load)) {
          return false;
        }
      }
      return true;
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