using UnityEngine;

namespace Apipi {
  public class Floating : MonoBehaviour {
    public float frqX;
    public float frqY;
    public float frqR;
    public float str;
    public float strR;

    public void Update() {
      var time = Time.realtimeSinceStartup;
      var seed = GetInstanceID().GetHashCode() / 123.45f;
      var x = str * Mathf.Sin(frqX * time + seed);
      var y = str * Mathf.Sin(frqY * frqX * time + seed * frqY);
      transform.localPosition = new Vector3(x, y, 0);

      var r = strR * Mathf.Sin(frqR * time);
      transform.localRotation = Quaternion.Euler(0, 0, r);
    }
  }
}
