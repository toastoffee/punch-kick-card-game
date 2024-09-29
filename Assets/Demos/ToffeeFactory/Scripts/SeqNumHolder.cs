using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToffeeFactory {
  public class SeqNumHolder : MonoBehaviour {
    private Dictionary<string, int> seqNums = new Dictionary<string, int>();

    public void NotifyUpdate(string id) {
      EnsureSeqNum(id);
      seqNums[id]++;
    }

    public int Read(string id) {
      EnsureSeqNum(id);
      return seqNums[id];
    }

    private void EnsureSeqNum(string id) {
      if (!seqNums.ContainsKey(id)) {
        seqNums[id] = 0;
      }
    }
  }
}