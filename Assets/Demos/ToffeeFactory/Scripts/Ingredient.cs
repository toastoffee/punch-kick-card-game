using System;

namespace ToffeeFactory {
  
  [Serializable]
  public class Ingredient {
    public string name;
    public int count;

    private static Ingredient Empty = new Ingredient() {
      name = "empty",
      count = 0,
    };
  }

  [Serializable]
  public class Storage {
    public string name;
    public int max;
    private int current;

    public int count { get => current; set => current = value; }

    public bool isFull => current == max;

    public bool isSufficient(Ingredient load) {
      if (load.name == name && count >= load.count) {
        return true;
      } else {
        return false;
      }
    }

    public bool TryConsume(Ingredient load) {
      if (isSufficient(load)) {
        count -= load.count;
        return true;
      } else {
        return false;
      }
    }

    public bool TryAdd(Ingredient load) {
      if (load.name != name || isFull) {
        return false;
      }
      
      if (load.name == name) {
        count += load.count;
        count = count > max ? max : count;
      }
      return true;
    }
    
  }
}