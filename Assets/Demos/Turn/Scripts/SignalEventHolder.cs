using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnGame {
  public class SignalEventHolder : MonoBehaviour {
    public string signal;
    public void Fire() {
      var sigEvent = new Signal(signal);
      TurnGame.Instance.PushEvent(sigEvent);
    }
  }
}