using System;
using UnityEngine;

public struct ScopeLocal<T> : IDisposable {
  private static bool m_inScope;
  private static T m_instance;

  public static ScopeLocal<T> Create(T value) {
    var ret = new ScopeLocal<T>();
    m_inScope = true;
    m_instance = value;
    return ret;
  }

  public T ReadValue() {
    if (!m_inScope) {
      Debug.LogError("not in scope");
      return default;
    }
    return m_instance;
  }

  public void Dispose() {
    m_inScope = false;
    m_instance = default;
  }
}
