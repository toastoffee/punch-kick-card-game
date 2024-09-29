using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnityUtils {
  public static bool IsNullOrUObjectNull(this object obj) {
    if (obj == null) {
      return true;
    }

    if (obj is UnityEngine.Object uobj) {
      return uobj == null;
    }

    if (obj is UnityEngine.MonoBehaviour mono) {
      return mono == null;
    }
    return false;
  }

  public static IEnumerable<T> SafeUObjects<T>(this T[] objArr) where T : class {
    for (int i = 0, count = objArr.SafeCount(); i < count; i++) {
      if (objArr[i].IsNullOrUObjectNull()) {
        continue;
      }
      yield return objArr[i] as T;
    }
  }

  public static void RemoveNullUObjects<T>(this IList<T> list) {
    if (list == null) {
      return;
    }
    for (int i = 0; i < list.Count; i++) {
      if (list[i].IsNullOrUObjectNull()) {
        list.RemoveAt(i);
        i--;
      }
    }
  }

  public static bool TryFindParentWithTag(this GameObject go, string tag, out GameObject res) {
    res = FindParentWithTag(go, tag);
    return res != null;
  }

  public static GameObject FindParentWithTag(this GameObject go, string tag) {
    if (go == null) {
      return null;
    }

    var parent = go.transform;
    while (parent != null) {
      if (parent.tag == tag) {
        return parent.gameObject;
      }
      parent = parent.parent;
    }

    return null;
  }

  public static bool TryFindParentWithTag(this Component component, string tag, out GameObject res) {
    res = FindParentWithTag(component, tag);
    return res != null;
  }

  public static GameObject FindParentWithTag(this Component component, string tag) {
    if (component == null) {
      return null;
    }
    return component.gameObject.FindParentWithTag(tag);
  }

  public static void SetLocalScaleX(this Transform transform, float x) {
    var cur = transform.localScale;
    cur.x = x;
    transform.localScale = cur;
  }

  public static void SetLocalScaleY(this Transform transform, float y) {
    var cur = transform.localScale;
    cur.y = y;
    transform.localScale = cur;
  }

  public static void SetLocalScaleZ(this Transform transform, float z) {
    var cur = transform.localScale;
    cur.z = z;
    transform.localScale = cur;
  }

  public static Vector2 RandomWithin(this BoxCollider2D box) {
    var position = (Vector2)box.transform.position;
    var xRange = new Vector2(position.x + box.offset.x - box.size.x / 2, position.x + box.offset.x + box.size.x / 2);
    var yRange = new Vector2(position.y + box.offset.y - box.size.y / 2, position.y + box.offset.y + box.size.y / 2);

    var rand = new Vector2(Random.Range(xRange.x, xRange.y), Random.Range(yRange.x, yRange.y));
    return rand;
  }

  public static void ClampedForce(this Rigidbody2D rb, Vector2 dir, float maxSpeed, float force) {
    dir = dir.normalized;
    maxSpeed = Mathf.Max(maxSpeed, 0);
    var cur = rb.velocity;
    float dot = Vector2.Dot(cur, dir.normalized);

    if (dot < maxSpeed) {
      rb.AddForce(force * dir);
    }
  }

  public static Vector3 rbForward(this Rigidbody rb) {
    return rb.rotation * Vector3.forward;
  }

  public static Vector3 rbUp(this Rigidbody rb) {
    return rb.rotation * Vector3.up;
  }

  public static List<Transform> FindChildren(this Transform transform, string name) {
    var res = new List<Transform>();
    foreach (Transform child in transform) {
      if (child.name.ToLower() == name.ToLower()) {
        res.Add(child);
      }
    }
    return res;
  }

  public static Quaternion ToQuaternion(this Vector3 vec) {
    return Quaternion.Euler(vec.x, vec.y, vec.z);
  }

  public static Quaternion ToQuaternion(this Unity.Mathematics.float3 vec) {
    return Quaternion.Euler(vec.x, vec.y, vec.z);
  }

  public static T NotNull<T>(this T obj) {
#if !UNITY_EDITOR
    return obj;
#else
    if (obj.IsNullOrUObjectNull()) {
      Debug.LogError($"[NotNull] 类型为【{typeof(T)}】的对象为空");
    }
    return obj;
#endif
  }

  public static Vector2 ClampByRect(this Vector2 vec, Rect rect) {
    vec.x = Mathf.Clamp(vec.x, rect.xMin, rect.xMax);
    vec.y = Mathf.Clamp(vec.y, rect.yMin, rect.yMax);
    return vec;
  }

  public static void DestroyAllChildren(this Transform transform) {
    List<Transform> children = new();
    for (int i = 0; i < transform.childCount; i++) {
      children.Add(transform.GetChild(i));
    }

    foreach (var child in children) {
      GameObject.Destroy(child.gameObject);
    }
  }

  public static string LogObjectScenePath(this GameObject obj) {
    var sb = new System.Text.StringBuilder();
    while (obj != null) {
      sb.Append(obj.name);
      sb.Append(" ->");
      obj = obj.transform.parent == null ? null : obj.transform.parent.gameObject;
    }
    if (sb.Length > 0) {
      return sb.ToString();
    }
    return string.Empty;
  }
}
public struct ComponentFinder<T> where T : Component {
  private T m_cache;
  public T Get(Component comp) {
    if (m_cache == null) {
      while (comp != null && !comp.TryGetComponent(out m_cache)) {
        comp = comp.transform.parent;
      }
    }
    return m_cache;
  }
}

public struct ResourcesLoadCache<T> where T : Object {
  private string path;
  private T res;

  public T Res {
    get {
      if (res == null) {
        res = Resources.Load<T>(path);
      }
      return res;
    }
  }

  public ResourcesLoadCache(string path) {
    this.path = path;
    res = null;
  }

}