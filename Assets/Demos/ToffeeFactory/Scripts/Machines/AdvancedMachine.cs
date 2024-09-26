using UnityEngine;
using System.Collections.Generic;

namespace ToffeeFactory {
  public abstract class AdvancedMachine : MonoBehaviour {
    
    public static float pipeInterval = 0.5f;

    public abstract bool ReceiveStuffLoad(StuffLoad load);

    public abstract IEnumerable<Port> GetAllPorts();
  }
}