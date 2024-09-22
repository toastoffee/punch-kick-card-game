using UnityEngine;

namespace ToffeeFactory {
  public abstract class AdvancedMachine : MonoBehaviour {
    
    public static float pipeInterval = 0.5f;

    public abstract bool ReceiveStuffLoad(StuffLoad load);
  }
}