using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

/// <summary>
/// 可以仅在下一帧为True的Flag（不用重置）
/// ---Frame 1---
/// flag.IsTrue(); ->false
/// flag.SetTrue();
/// flag.IsTrue(); ->false
/// ---Frame 2---
/// flag.IsTrue(); ->true
/// ---Frame 3---
/// flag.IsTrue(); ->false
/// 
/// </summary>
public struct FrameFlag {
  private bool value;
  private int validFrame;
  private int altValidFrame;

  public void SetTrue() {
    value = true;
    if (value) {
      if (validFrame == Time.frameCount) {
        altValidFrame = Time.frameCount;//cache valid frame When SetTrue() in validFrame
      }
      validFrame = Time.frameCount + 1;
    }
  }

  public bool isTrue => IsTrue();

  public bool IsTrue() {
    if (value && (validFrame == Time.frameCount || altValidFrame == Time.frameCount)) {
      return true;
    }
    return false;
  }
}

/// <summary>
///  等价于类似的逻辑,可以用来实现计时器：
///  if(condition){
///    cacheTime = Time.time;
///  }
///  
///  if(Time.time - cacheTime < XXX){
///   return true;
///  }
/// </summary>
public struct TimeFlag {
  private bool value;
  private float setTime;

  public void SetTrue() {
    value = true;
    setTime = Time.time;
  }
  public bool LessThan(float duration) {
    return value && ((Time.time - setTime) <= duration);
  }
}