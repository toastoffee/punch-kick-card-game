using System;

namespace ToffeeFactory {
  
  [Serializable]
  public class SingleStorage {

    public bool _typeRestrict = false;   
    public StuffType _type;
    public int _capacity;
    public int _count;

    public bool typeRestrict => _typeRestrict;
    public StuffType type => _type;
    public int capacity {
      get => _capacity;
      set {
        _capacity = value;
        if (_count > _capacity) {
          _count = _capacity;
        }
      }
    }
    public int count => _count;
    public bool isFull => _count == _capacity;
    public bool isEmpty => _count == 0;
    public int leftCapacity => _capacity - _count;
      
    public SingleStorage(int max) {
      _typeRestrict = false;
      _type = StuffType.NONE;
      _capacity = max;
      _count = 0;
    }

    public void SetRestrictType(StuffType restrictType) {
      _typeRestrict = true;
      _type = restrictType;
      _count = 0;
    }

    public void UnlockRestrictType() {
      _typeRestrict = false;
    }

    // return true if added
    public bool TryAdd(StuffLoad load) {
      if (load.count == 0) {
        return false;
      }

      int formerCount = load.count;
      // if type is restricted => must be the same type
      if (_typeRestrict) {
        if (_type == load.type && !isFull) {
          _type = load.type;

          // add amount
          int addAmount = Math.Min(leftCapacity, load.count);
          _count += addAmount;

          // modify load
          load.count -= addAmount;
        }
      } else {  // else => type same OR empty
        if (isEmpty || (_type == load.type && !isFull)) {
          _type = load.type;

          // add amount
          int addAmount = Math.Min(leftCapacity, load.count);
          _count += addAmount;

          // modify load
          load.count -= addAmount;
        }  
      }

      return formerCount != load.count;
    }

    // return true if consumed
    public bool TryConsume(StuffLoad load) {
      if (load.count == 0) {
        return false;
      }
      
      int formerCount = load.count;
      if (_type == load.type && !isEmpty) {
          
        // consume amount
        int consumeAmount = Math.Min(_count, load.count);
        _count -= consumeAmount;
          
        // modify load
        load.count -= consumeAmount;
      }
      return formerCount != load.count;
    }

    public void TryProvide(StuffLoad load) {
      if (load.count == 0) {
        return;
      }
      
      if (_type == load.type && !isEmpty) {
        // provide amount
        int provideAmount = Math.Min(_count, load.count);
        
        // modify load
        load.count -= provideAmount;
      }
    }

    public void TryContain(StuffLoad load) {
      if (load.count == 0) {
        return;
      }
      
      // if type is restricted => must be the same type
      if (_typeRestrict) {
        if (_type == load.type && !isFull) {

          // modify load
          int containAmount = Math.Min(leftCapacity, load.count);
          load.count -= containAmount;
        }
      } else { // else => type same OR empty
        if (isEmpty || (_type == load.type && !isFull)) {

          // modify load
          int containAmount = Math.Min(leftCapacity, load.count);
          load.count -= containAmount;
        }  
      }
    }

    public void Clear() {
      _count = 0;
    }
  }
}