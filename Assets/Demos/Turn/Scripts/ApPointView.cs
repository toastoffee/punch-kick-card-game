using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApPointView : MonoBehaviour {
  public GameObject apObj;
  public void SetAp(bool value) {
    apObj.SetActive(value);
  }
}
