using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;

public static class CSharpUtils {
  public static T SafeGet<T>(this T[,] arr, Vector2Int pos, T defaultVal = default(T)) {
    if (arr == null) {
      return defaultVal;
    }
    var size0 = arr.GetLength(0);
    var size1 = arr.GetLength(1);
    if (pos.x > -1 && pos.y > -1 &&
        pos.x < size0 && pos.y < size1) {
      return arr[pos.x, pos.y];
    }
    return defaultVal;
  }

  public static T SafeGet<T>(this T[,] arr, Vector2Int pos, Func<T> defaultValFunc) {
    if (arr == null) {
      return defaultValFunc.Invoke();
    }
    var size0 = arr.GetLength(0);
    var size1 = arr.GetLength(1);
    if (pos.x > -1 && pos.y > -1 &&
        pos.x < size0 && pos.y < size1) {
      return arr[pos.x, pos.y];
    }
    return defaultValFunc.Invoke();
  }

  public static bool SafeContainsIndex<T>(this T[,] arr, Vector2Int pos) {
    if (arr == null) {
      return false;
    }
    var size0 = arr.GetLength(0);
    var size1 = arr.GetLength(1);
    if (pos.x > -1 && pos.y > -1 &&
        pos.x < size0 && pos.y < size1) {
      return true;
    }
    return false;
  }

  public static Vector2Int SafeSize<T>(this T[,] arr) {
    if (arr == null) {
      return Vector2Int.zero;
    }
    return new Vector2Int(arr.GetLength(0), arr.GetLength(1));
  }

  public static TVal SafeGet<TKey, TVal>(this Dictionary<TKey, TVal> dict, TKey key, TVal defaultVal = default(TVal)) {
    if (dict == null || !dict.ContainsKey(key)) {
      return defaultVal;
    }
    return dict[key];
  }

  public static int SafeCount<T>(this ICollection<T> collection) {
    if (collection == null) {
      return 0;
    }
    return collection.Count;
  }

  public static T[,] CreateResizeCopy<T>(this T[,] arr, Vector2Int size) {
    Debug.Assert(size.x > -1);
    Debug.Assert(size.y > -1);
    if (arr == null) {
      return new T[size.x, size.y];
    }
    var newArr = new T[size.x, size.y];
    var iterSize = new Vector2Int(Mathf.Min(size.x, arr.GetLength(0)), Mathf.Min(size.y, arr.GetLength(1)));
    for (int x = 0; x < iterSize.x; x++) {
      for (int y = 0; y < iterSize.y; y++) {
        newArr[x, y] = arr[x, y];
      }
    }
    return newArr;
  }

  public static List<T> Copy<T>(this List<T> src) {
    return new List<T>(src);
  }

  public static float ClampAbs(this float val, float abs) {
    if (val == 0) {
      return 0;
    }
    abs = Mathf.Abs(abs);
    return Mathf.Min(abs, Mathf.Abs(val)) * Mathf.Sign(val);
  }

  public static float MinAbs(this float val, float abs) {
    abs = Mathf.Abs(abs);
    return Mathf.Max(abs, Mathf.Abs(val)) * Mathf.Sign(val);
  }

  public static float Abs(this float val) {
    return Mathf.Abs(val);
  }

  public static float Abs(this int val) {
    return Mathf.Abs(val);
  }

  public static float SinTo01(this float val) {
    return 0.5f * (val + 1);
  }

  public static float NotLessThanZero(this float val, float min = 0.0000001f) {
    return Mathf.Max(min, val);
  }

  public static void Shuffle<T>(this IList<T> list) {
    int n = list.Count;
    var rng = new System.Random();

    while (n > 1) {
      n--;
      int k = rng.Next(n + 1);
      T value = list[k];
      list[k] = list[n];
      list[n] = value;
    }
  }

  public static T Find<T>(this T[] arr, Predicate<T> cond) {
    if (cond == null) {
      return default;
    }
    for (int i = 0; i < arr.SafeCount(); i++) {
      if (cond(arr[i])) {
        return arr[i];
      }
    }
    return default;
  }

  public class CollectionAdder<T> {
    private ICollection<T> target;

    public CollectionAdder(ICollection<T> collection) {
      target = collection;
    }

    public static CollectionAdder<T> operator +(CollectionAdder<T> adder, T element) {
      adder.target.Add(element);
      return adder;
    }
  }

  public static CollectionAdder<T> Adder<T>(this ICollection<T> collection) {
    return new CollectionAdder<T>(collection);
  }

  public static T SafeInvoke<T>(this Func<T> func) {
    if (func == null) {
      return default(T);
    }
    return func();
  }

  public static bool isNullOrEmpty(this string str) {
    return string.IsNullOrEmpty(str);
  }

  public static T As<T>(this object obj) {
    return (T)obj;
  }

  public static void RemoveAllValues<K, V>(this IDictionary<K, V> dict, Predicate<V> condition) {
    var toRemove = new List<K>();
    foreach (var pair in dict) {
      if (condition(pair.Value)) {
        toRemove.Add(pair.Key);
      }
    }
    foreach (var key in toRemove) {
      dict.Remove(key);
    }
  }

  public static void SwapByIndex<T>(this IList<T> list, int indexA, int indexB) {
    var valA = list[indexA];
    var valB = list[indexB];
    list[indexA] = valB;
    list[indexB] = valA;
  }

  public static V GetValueNotNull<K, V>(this IDictionary<K, V> dict, K key, Action<V> constructor = null) where V : new() {
    if (!dict.ContainsKey(key)) {
      dict[key] = new V();
      constructor?.Invoke(dict[key]);
    }
    return dict[key];
  }

  public static IEnumerable<T> Union<T>(this IEnumerable<T> lhs, IEnumerable<T> rhs) {
    foreach (var item in lhs) {
      yield return item;
    }
    foreach (var item in rhs) {
      yield return item;
    }
  }
}
public struct SeqNumChecker {
  private int m_cachedNum;
  public SeqNumChecker(int init) {
    m_cachedNum = init;
  }

  public bool ConsumeUpdate(int seqNum) {
    var flag = m_cachedNum != seqNum;
    m_cachedNum = seqNum;
    return flag;
  }

  public static bool operator ==(SeqNumChecker a, SeqNumChecker b) => a.m_cachedNum == b.m_cachedNum;
  public static bool operator !=(SeqNumChecker a, SeqNumChecker b) => a.m_cachedNum != b.m_cachedNum;
}