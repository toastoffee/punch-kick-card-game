using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToffeeFactory {
  public class Devider : AdvancedMachine {
    public List<Port> inPorts;
    public List<Port> outPorts;

    private int receiveIdx;
    private int outPortCount;

    private void Start() {
      outPortCount = outPorts.Count;
      // set ports belonging
      foreach (var port in inPorts) {
        port.machineBelong = this;
      }
      foreach (var port in outPorts) {
        port.machineBelong = this;
      }
    }


    public override IEnumerable<Port> GetAllPorts() {
      foreach (var port in outPorts) {
        yield return port;
      }
      foreach (var port in inPorts) {
        yield return port;
      }
    }

    public override bool ReceiveStuffLoad(StuffLoad load) {
      var flag = false;
      for (int i = 0; i < 3; i++) {
        if (flag) {
          break;
        }
        var biasIdx = (receiveIdx + i) % outPortCount;
        var port = outPorts[biasIdx];
        if (port.isConnected && port.connectedPort.machineBelong.ReceiveStuffLoad(load)) {
          flag = true;
          receiveIdx++;
          receiveIdx %= outPortCount;
        }
      }
      return flag;
    }
  }
}
