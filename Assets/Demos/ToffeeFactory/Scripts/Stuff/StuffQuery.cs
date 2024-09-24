namespace ToffeeFactory {
  public class StuffQuery {
    
    public static string GetRichText(StuffType type) {
      switch (type) {
        case StuffType.NONE:
          return "NONE";
        case StuffType.IronMine:
          return "<color=#FF171D>铁矿</color>";
        case StuffType.CoalMine:
          return "<color=#9A6C4E>煤炭矿</color>";
        case StuffType.CooperMine:
          return "<color=#00DB65>铜矿</color>";
        
        case StuffType.IronOre:
          return "<color=#FF1F47>铁矿石</color>";
        case StuffType.CoalOre:
          return "<color=#874743>煤炭</color>";
        case StuffType.CooperOre:
          return "<color=#27BC4F>铜矿石</color>";
        
        case StuffType.IronIngot:
          return "<color=#939393>铁锭</color>";
        case StuffType.CooperIngot:
          return "<color=#FFAC00>铜锭</color>";
        
        
        case StuffType.CooperWire:
          return "<color=#FFBD00>铜线</color>";
        case StuffType.IronPlate:
          return "<color=#B4B4B4>铁板</color>";
        
        case StuffType.CircuitBoard:
          return "<color=#004B0B>电路板</color>";
      }

      return "NONE";
    }
  }
}