//一个模板类，用来创建一个表，与直接使用Dictionary不同的是可以提供一个构造函数，配合StaticInst可以很方便的构造静态单例表
using System.Collections.Generic;
using UnityEngine;

#region StaticInst
/*
 * 一个模板，用来方便创建一个静态实例，并且不需要显式初始化和赋值（因为是struct），而可以懒启动
 * 原理：泛型类型MyClass<T>的每一个封闭类型（例如MyClass<float>，MyClass<int>）的静态成员是独立的
 * 因此StaticInst<THost, TInst>将会创建一个与THost关联的实例（例如StaticInst<int, MyClass>和StaticInst<float, MyClass>对应了两个不同的静态的MyClass实例）
 */
public struct StaticInst<THost, TInst> where TInst : new() {
  private static TInst _inst;
  public TInst inst {
    get {
      if (_inst == null) {
        _inst = new TInst();
      }
      return _inst;
    }
  }

  public static TInst staticGetInst {
    get {
      if (_inst == null) {
        _inst = new TInst();
      }
      return _inst;
    }
  }
}

public struct StaticInst<TInst> where TInst : new() {
  private static TInst _inst;
  public TInst getInst {
    get {
      if (_inst == null) {
        _inst = new TInst();
      }
      return _inst;
    }
  }

  public static TInst staticInst {
    get {
      if (_inst == null) {
        _inst = new TInst();
      }
      return _inst;
    }
  }

  public TInst inst {
    get {
      return staticInst;
    }
  }

  public static TInst staticSetInt {
    set {
      _inst = value;
    }
  }
}
#endregion

public struct StaticTable<TValue> {
  public TValue this[string key] {
    get => Read(key);
  }

  public static TValue Read(string key) {
    return StaticInst<CommonTemplateTable<string, TValue>>.staticInst[key];
  }
}

public class CommonTemplateTable<TKey, TValue> : TemplateTable<TKey, TValue> {
  protected override void OnCreateTable(Dictionary<TKey, TValue> dict) { }
}

public abstract class TemplateTable<TKey, TValue> {
  protected Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();
  public TValue this[TKey key] {
    get {
      if (dict.ContainsKey(key)) {
        return dict[key];
      }
      Debug.LogError($"[Table] 没找到key [{key}]");
      return default;
    }
  }

  public void ReflectOnlyAdd(object key, object val) {
    dict[(TKey)key] = (TValue)val;
  }

  protected TemplateTable() {
    OnCreateTable(dict);
  }

  protected abstract void OnCreateTable(Dictionary<TKey, TValue> dict);
}