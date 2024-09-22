using System;

namespace ToffeeFactory {
  public class SingleStorage {

    private bool _typeRestrict = false;   
    private string _type;
    private int _capacity;
    private int _count;

    public bool typeRestrict => _typeRestrict;
    public string type => _type;
    public int capacity => _capacity;
    public int count => _count;
    public bool isFull => _count == _capacity;
    public bool isEmpty => _count == 0;
    public int leftCapacity => _capacity - _count;
      
    public SingleStorage(int max) {
      _typeRestrict = false;
      _type = "";
      _capacity = max;
      _count = 0;
    }

    public void SetRestrictType(string restrictType) {
      _typeRestrict = true;
      _type = restrictType;
      _count = 0;
    }

    public void TryAdd(ref Ingredient load) {
      // if type is restricted => must be the same type
      if (_typeRestrict) {
        if (_type == load.name && !isFull) {
          _type = load.name;

          // add amount
          int addAmount = Math.Min(leftCapacity, load.count);
          _count += addAmount;

          // modify load
          load.count -= addAmount;
        }
      } else {  // else => type same OR empty
        if (isEmpty || (_type == load.name && !isFull)) {
          _type = load.name;

          // add amount
          int addAmount = Math.Min(leftCapacity, load.count);
          _count += addAmount;

          // modify load
          load.count -= addAmount;
        }  
      }
    }

    public void TryConsume(ref Ingredient load) {
      if (_type == load.name && !isEmpty) {
          
        // consume amount
        int consumeAmount = Math.Min(_count, load.count);
        _count -= consumeAmount;
          
        // modify load
        load.count -= consumeAmount;
      }
    }

    public void Clear() {
      _count = 0;
    }
  }
}