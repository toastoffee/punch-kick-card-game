using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DampValue {
  private float m_lastActiveTime;
  private float m_fallTime;

  public DampValue(float fallTime) {
    m_fallTime = fallTime.NotLessThanZero();
    m_lastActiveTime = 0;
  }

  public void OnActive() {
    m_lastActiveTime = Time.time;
  }

  public float ReadValue() {
    var delta = Time.time - m_lastActiveTime;
    return Mathf.Lerp(1, 0, delta / m_fallTime);
  }
}