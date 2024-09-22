using UnityEngine;

namespace ToffeeFactory {
  [SerializeField]
  public class StuffLoad {
    public StuffType type;
    public int count;

    private static StuffLoad Nothing() {
      return new StuffLoad() {
        type = StuffType.NONE,
        count = 0,
      };
    }
    
  }
}