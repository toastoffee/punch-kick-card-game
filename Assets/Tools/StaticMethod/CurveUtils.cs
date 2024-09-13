using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CurveUtils {
  public static void ParabolicMove(Transform transform, float maxDelta, float arcHeight, Vector3 startPosition, Vector3 target) {
    // Compute the next position, with arc added in
    Vector2 xz0 = startPosition.XZ();
    Vector2 xz1 = target.XZ();
    Vector2 xzCur = transform.position.XZ();
    float dist = (xz1 - xz0).magnitude;
    float currentDist = (xzCur - xz0).magnitude;
    float leftDist = (xzCur - xz1).magnitude;
    float nextDist = Mathf.MoveTowards(currentDist, dist, maxDelta);
    Vector2 nextXZ = xz0 + (xz1 - xz0).normalized * nextDist;
    float baseY = Mathf.Lerp(startPosition.y, target.y, currentDist / dist);
    float arc = arcHeight * currentDist * leftDist / (0.25f * dist * dist);
    var nextPos = new Vector3(nextXZ.x, baseY + arc, nextXZ.y);
    transform.LookAt(nextPos);
    transform.position = nextPos;
  }

  public static float ParabolicLerp(float n) {
    n = Mathf.Clamp01(n);
    return n * (1 - n) / 0.25f;
  }

  public static float TLerp(this float fval, float dt) {
    fval = 1 - Mathf.Clamp01(fval);
    fval = Mathf.Pow(fval, dt);
    return 1 - fval;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="t">在t秒后达到95%</param>
  /// <param name="dt"></param>
  /// <returns>用以作为lerp的t</returns>
  public static float TLerp95(this float t, float dt) {
    if (t <= 0) {
      return 0;
    }
    var k = Mathf.Pow(0.05f, 1 / t);
    k = Mathf.Pow(k, dt);
    return 1 - k;
  }

  public struct ADSR {
    public float a;
    public float d;
    public float s_val;
    public float s_t;
    public float r;

    public float GetValue(float t) {
      t = Mathf.Clamp01(t);
      var total = a + d + s_t + r;
      t = Mathf.Lerp(0, total, t);
      
      if (t > 0 && t <= a) {
        return Mathf.Lerp(0, 1, Mathf.InverseLerp(0, a, t));
      } else if (t > a && t <= a + d) {
        return Mathf.Lerp(1, s_val, Mathf.InverseLerp(a, a + d, t));
      } else if (t > a + d && t <= a + d + s_t) {
        return s_val;
      } else if (t > a + d + s_t && t <= a + d + s_t + r) {
        return Mathf.Lerp(s_val, 0, Mathf.InverseLerp(a + d + s_t, a + d + s_t + r, t));
      } else {
        return 0;
      }
    }
  }
}