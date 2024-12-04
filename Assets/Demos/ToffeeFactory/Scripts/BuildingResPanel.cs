using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
// using UnityEditor.SceneManagement;

namespace ToffeeFactory {
  public class BuildingResPanel : MonoBehaviour {
    public class Item {
      public GameObject obj;
      public TMP_Text text;
    }

    public GameObject itemPrefab;
    public Transform container;

    private List<Item> items = new List<Item>();

    private void Update() {
      var storages = RepositoryStorageSet.Instance.GetStorages();
      var cnt = storages.Count;
      if (cnt != items.Count) {
        foreach (var item in items) {
          Destroy(item.obj);
        }

        for (int i = 0; i < cnt; i++) {
          var itemObj = Instantiate(itemPrefab, container);
          var item = new Item {
            obj = itemObj,
            text = itemObj.GetComponentInChildren<TMP_Text>(),
          };
          items.Add(item);
        }
      }
      for (int i = 0; i < cnt; i++) {
        var sto = storages[i];
        var item = items[i];

        item.text.text = $"{StuffQuery.GetRichText(sto.type)} {sto.count}/{sto.capacity}";
      }
    }
  }
}
