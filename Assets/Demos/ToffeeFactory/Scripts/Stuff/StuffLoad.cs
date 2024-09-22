using System;
using UnityEngine;

namespace ToffeeFactory {
  [Serializable]
  public class StuffLoad {
    public StuffType type;
    public int count;

    public StuffLoad(StuffType _type, int _count) {
      type = _type;
      count = _count;
    }
    
    public static StuffLoad Nothing() {
      return new StuffLoad(StuffType.NONE, 0);
    }

    public StuffLoad Copy() {
      return new StuffLoad(type, count);
    }
    
  }
}