using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLayout : MonoBehaviour {
  public class ViewCache {
    public object view;
    public GameObject gameObject;

    public T GetView<T>() where T : Component {
      if (view == null) {
        view = gameObject.GetComponent<T>();
      }
      if (view.IsNullOrUObjectNull()) {
        Debug.LogError($"can't get view [{typeof(T)}]");
      }
      if (!(view is T)) {
        Debug.LogError($"view is not [{typeof(T)}] but [{view.GetType()}]");
      }
      return view as T;
    }
  }

  public abstract class Adapter {
    public abstract int count { get; }
    public abstract void OnRender(int position, ViewCache viewCache);
  }

  public Transform container;
  public GameObject prefab;

  private Adapter adapter;
  private List<ViewCache> viewCaches = new List<ViewCache>();
  public void SetAdapter(Adapter adapter) {
    this.adapter = adapter;
  }

  public void NotifyUpdate() {
    AdjustView();
  }

  private void AdjustView() {
    var require = adapter.count;
    for (int i = 0; i < viewCaches.Count; i++) {
      SetCacheActive(viewCaches[i], i < require);
      if (i < require) {
        adapter.OnRender(i, viewCaches[i]);
      }
    }
    for (int i = viewCaches.Count; i < require; i++) {
      viewCaches.Add(new ViewCache());
      SetCacheActive(viewCaches[i], true);
      adapter.OnRender(i, viewCaches[i]);
    }
  }

  private void SetCacheActive(ViewCache cache, bool active) {
    if (cache.gameObject == null) {
      var inst = Instantiate(prefab, container);
      cache.gameObject = inst;
    }
    cache.gameObject.SetActive(active);
  }
}
