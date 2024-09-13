using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class VectorUtils {
  public static Vector3 SetX(this Vector3 target, float x) {
    target = new Vector3(x, target.y, target.z);
    return target;
  }

  public static Vector3 SetZ(this Vector3 target, float z) {
    target = new Vector3(target.x, target.y, z);
    return target;
  }

  public static Vector3 SetY(this Vector3 target, float y) {
    target = new Vector3(target.x, y, target.z);
    return target;
  }

  public static Vector2 XY(this Vector3 vec) {
    return new Vector2(vec.x, vec.y);
  }

  public static Vector2 XZ(this Vector3 vec) {
    return new Vector2(vec.x, vec.z);
  }

  public static Vector2 YZ(this Vector3 vec) {
    return new Vector2(vec.y, vec.z);
  }

  public static Vector3 X0Y(this Vector2 vec) {
    return new Vector3(vec.x, 0, vec.y);
  }

  public static Vector3 SetXY(this Vector3 vec, Vector2 xy) {
    return new Vector3(xy.x, xy.y, vec.z);
  }

  public static Vector3 SetXZ(this Vector3 vec, Vector2 xz) {
    return new Vector3(xz.x, vec.y, xz.y);
  }

  public static Vector3 SetYZ(this Vector3 vec, Vector2 yz) {
    return new Vector3(vec.x, yz.x, yz.y);
  }

  public static Vector3Int SetX(this Vector3Int target, int x) {
    target = new Vector3Int(x, target.y, target.z);
    return target;
  }

  public static Vector3Int SetZ(this Vector3Int target, int z) {
    target = new Vector3Int(target.x, target.y, z);
    return target;
  }

  public static Vector3Int SetY(this Vector3Int target, int y) {
    target = new Vector3Int(target.x, y, target.z);
    return target;
  }

  public static Vector2Int XY(this Vector3Int vec) {
    return new Vector2Int(vec.x, vec.y);
  }

  public static Vector2Int XZ(this Vector3Int vec) {
    return new Vector2Int(vec.x, vec.z);
  }

  public static Vector2Int YZ(this Vector3Int vec) {
    return new Vector2Int(vec.y, vec.z);
  }

  public static Vector3Int SetXY(this Vector3Int vec, Vector2Int xy) {
    return new Vector3Int(xy.x, xy.y, vec.z);
  }

  public static Vector3Int SetXZ(this Vector3Int vec, Vector2Int xz) {
    return new Vector3Int(xz.x, vec.y, xz.y);
  }

  public static Vector3Int SetYZ(this Vector3Int vec, Vector2Int yz) {
    return new Vector3Int(vec.x, yz.x, yz.y);
  }


  public static float ProjectLength(this Vector3 vec, Vector3 on) {
    return Vector3.Project(vec, on).magnitude * Mathf.Sign(Vector3.Dot(vec, on));
  }

  public static float ProjectLength(this Vector2 vec, Vector3 on) {
    return Vector3.Project(vec, on).magnitude * Mathf.Sign(Vector3.Dot(vec, on));
  }

  public static Vector3 Rotate(this Vector3 source, Vector3 axis, float angle) {
    Quaternion q = Quaternion.AngleAxis(angle, axis);
    return q * source;
  }

}
