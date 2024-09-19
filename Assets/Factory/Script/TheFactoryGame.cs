using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static TheFactoryGame;

public class TheFactoryGame : MonoSingleton<TheFactoryGame> {
  public RectTransform cellContainer;
  public GameObject cellPrefab;
  public Vector2Int mapSize;
  public InstRot placingInstRot;
  public RectTransform pinBridgeLayerContainer;
  public PinBridgeView pinBridgeViewPrefab;

  public TMP_Text selectedToolText;
  public TMP_Text rotTxt;

  private Cell[,] m_cells;

  private int nextInstId = 0;
  private Dictionary<int, MapInst> m_mapInsts;
  private string selectedToolId;
  private Pin connectingSourcePin;

  public class Cell {
    public static Vector2Int[] near4 = new Vector2Int[] {
      Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left
    };
    public static Vector2Int[] near8 = new Vector2Int[] {
      Vector2Int.up, new Vector2Int(1,1),
      Vector2Int.right, new Vector2Int(1,-1),
      Vector2Int.down, new Vector2Int(-1,-1),
      Vector2Int.left, new Vector2Int(-1 ,1),
    };

    public GameObject gameObject;
    public Vector2Int pos;

    //一个cell可能被inst, pin, pinBridge占用
    public MapInst inst;
    public Vector2Int instOffset;

    public Pin pin;

    public PinBridge pinBridge;

    public CellView cellView;

    public int drawSeq;

    public bool isEmpty {
      get => inst == null && pin == null && pinBridge == null;
    }

    public int x {
      get => pos.x; set => pos.x = value;
    }
    public int y {
      get => pos.y; set => pos.y = value;
    }

    public Vector2 cellViewPos => cellView.transform.position;

    public bool debugShowPos;

    public void NotifyDraw() {
      drawSeq++;
    }

    public IEnumerable<Cell> Near4Area() {
      var map = TheFactoryGame.Instance.m_cells;
      for (int i = 0; i < near4.Length; i++) {
        yield return map.SafeGet(pos + near4[i]);
      }
    }
  }

  public class MapInstData {

  }

  public enum InstRot {
    UP, RIGHT, DOWN, LEFT
  }

  public class MapInst {
    public int instId;
    public InstRot rot;
    public Cell rootCell;
    public MapInstModel model;
    public MapInstData data = new MapInstData();

    public event Action<MapInst> onClick;
    public event Action<MapInst> onCircuitUpdate;
    public event Action<MapInst> onDelete;

    public void NotifyDelete() {
      this.onDelete?.Invoke(this);
      Instance.m_mapInsts.Remove(instId);
    }
  }

  /// <summary>
  /// 5 4 3
  /// 6 p 2
  /// 7 0 1
  /// </summary>
  public class Pin {
    public enum Polar {
      None = 0,
      Output = 1,
      Input = -1,
    }
    public struct CheckConnectResult {
      public bool success;
      public int dir;
      public List<Cell> overlayCells;
    }

    public MapInst parentInst;
    public Cell cell;
    public int pinDisableMask;
    public PinBridge[] pinBridges = new PinBridge[8];
    public Polar polar;

    public bool CanConnect(Pin other, out CheckConnectResult res) {
      res = new CheckConnectResult() {
        overlayCells = new List<Cell>(),
      };
      //在方向上
      var dir = CheckOn8Dir(cell.pos, other.cell.pos);
      if (dir < 0) {
        return false;
      }
      //方向没有被禁用
      if (CheckFlag(pinDisableMask, dir) || CheckFlag(other.pinDisableMask, (dir + 4) % 8)) {
        return false;
      }
      //并非已连接
      foreach (var pinBridge in pinBridges) {
        if (pinBridge != null && pinBridge.Contain(other)) {
          return false;
        }
      }
      //极性正确
      var sum = (int)polar + (int)other.polar;
      if (sum != 0 && Mathf.Abs(sum) != 1) {
        return false;
      }
      //引脚之间都是空位s
      var dirVec = Cell.near8[dir];
      Cell next = cell;
      do {
        next = Instance.m_cells.SafeGet(next.pos + dirVec);
        if (next == null) {
          return false;
        }
        if (next.pin == other) {
          break;
        }
        if (!next.isEmpty) {
          return false;
        }
        res.overlayCells.Add(next);
      } while (next != null);
      res.success = true;
      return true;
    }

    public void Connect(Pin other, CheckConnectResult result) {
      if (!result.success) {
        return;
      }
      var bridge = PinBridge.Create(this, other, result);
    }
  }

  public static int CheckOn8Dir(Vector2Int from, Vector2Int to) {
    var delta = (Vector2)(to - from);
    for (int i = 0; i < 8; i++) {
      var dir = Cell.near8[i];
      if (Vector2.SignedAngle(delta, dir) == 0) {
        return i;
      }
    }
    return -1;
  }

  public static bool CheckFlag(int flag, int bit) {
    return (flag & (1 << bit)) != 0;
  }

  public void CreatePinBridgeView(PinBridge bridge) {
    var view = Instantiate(pinBridgeViewPrefab, pinBridgeLayerContainer);
    view.pinBridge = bridge;
  }

  public class PinBridge {
    private enum InnerPolar {
      none, p01, p10
    }
    private Pin pin0, pin1;
    private InnerPolar innerPolar;

    public bool isNonPolar => innerPolar != InnerPolar.none;
    public Pin fromPin {
      get {
        if (innerPolar != InnerPolar.p10) {
          return pin0;
        }
        return pin1;
      }
    }
    public Pin toPin {
      get {
        if (innerPolar == InnerPolar.p10) {
          return pin0;
        }
        return pin1;
      }
    }

    private PinBridge() { }
    public static PinBridge Create(Pin pin0, Pin pin1, Pin.CheckConnectResult checkRes) {
      if (!checkRes.success) {
        throw new Exception("Check Connectable first");
      }
      var ret = new PinBridge() {
        pin0 = pin0,
        pin1 = pin1,
      };
      ret.Refresh();
      foreach (var cell in checkRes.overlayCells) {
        cell.pinBridge = ret;
      }
      Instance.CreatePinBridgeView(ret);
      return ret;
    }

    public bool Contain(Pin pin) {
      return pin0 == pin || pin1 == pin;
    }

    public void Refresh() {
      if (pin0.polar == Pin.Polar.None && pin1.polar == Pin.Polar.None) {
        innerPolar = InnerPolar.none;
      } else {
        if (pin0.polar == Pin.Polar.Output) {
          innerPolar = InnerPolar.p01;
        } else {
          innerPolar = InnerPolar.p10;
        }
      }
    }
  }

  public delegate void MapInstOnAttach(MapInst mapInst);

  public interface IModel {
    public string modelId { get; set; }
  }

  public class MapInstModel : IModel {
    public string modelId { get; set; }
    public string sprId;
    public MapInstOnAttach onAttach;
    public Action<MapInst, Cell> onClick;
  }

  public Dictionary<string, MapInstModel> mapInstModel;

  private void InitModel() {
    mapInstModel = new Dictionary<string, MapInstModel>() {
      {
        "machine_1",
        new MapInstModel {
          sprId = "machine_1"
        }
      },
      {
        "pin",
        new MapInstModel()
        {
          sprId = "pin",
          onClick = InstClick_Pin,
          onAttach = InstAttach_Pin,
        }
      }
    };
    SetModelId(mapInstModel);
  }

  private void SetModelId<T>(Dictionary<string, T> modelTable) where T : IModel {
    foreach (var pair in modelTable) {
      pair.Value.modelId = pair.Key;
    }
  }

  protected override void Awake() {
    base.Awake();
    InitModel();
  }

  public void Start() {
    InitMap();
  }

  private void Update() {
    if (Input.GetKeyDown(KeyCode.R)) {
      placingInstRot += 1;
      placingInstRot = (InstRot)((int)placingInstRot % 4);
    }
    rotTxt.text = placingInstRot.ToString();

    if (Input.GetMouseButtonDown(1)) {
      selectedToolId = string.Empty;
    }
    selectedToolText.text = selectedToolId;
  }

  public void NotifyUpdateCircuit() {
  }

  private void InitMap() {
    m_mapInsts = new Dictionary<int, MapInst>();
    m_cells = new Cell[mapSize.x, mapSize.y];
    var cnt = mapSize.x * mapSize.y;
    for (int y = 0; y < mapSize.y; y++) {
      for (int x = 0; x < mapSize.x; x++) {
        var cellObj = Instantiate(cellPrefab, cellContainer);
        var cellView = cellObj.GetComponent<CellView>();

        var cell = new Cell {
          gameObject = cellObj,
          pos = new Vector2Int(x, y),
          cellView = cellView,
        };

        cellView.SetCell(cell);

        m_cells[x, y] = cell;
      }
    }
  }

  public MapInst CreatMapInst(string modelId) {
    var model = mapInstModel[modelId];

    var inst = new MapInst {
      instId = nextInstId++,
      model = model
    };

    m_mapInsts.Add(inst.instId, inst);

    return inst;
  }

  private void PlantInstOnCell(Cell cell, string instModelId, InstRot rot) {
    if (cell.inst != null) {
      return;
    }

    var inst = CreatMapInst(instModelId);
    inst.rot = rot;
    cell.inst = inst;
    inst.rootCell = cell;
    inst.model.onAttach?.Invoke(inst);
    cell.NotifyDraw();
  }

  private void DeleteInstOnCell(Cell cell) {
    if (cell.inst == null) {
      return;
    }
    cell.inst.NotifyDelete();
    cell.inst = null;
    NotifyUpdateCircuit();
    cell.NotifyDraw();
  }

  public void OnCellClick(Cell cell) {
    switch (selectedToolId) {
      case "pin":
        PlantInstOnCell(cell, "pin", placingInstRot);
        break;
      case "machine_1":
        PlantInstOnCell(cell, "machine_1", placingInstRot);
        break;
      case "delete":
        DeleteInstOnCell(cell);
        break;
      case "connecting":
        selectedToolId = string.Empty;
        if (connectingSourcePin.CanConnect(cell.pin, out var res)) {
          connectingSourcePin.Connect(cell.pin, res);
        }
        break;
      case "": //string.empty
        if (cell.inst == null) {
          return;
        }
        cell.inst.model.onClick?.Invoke(cell.inst, cell);
        break;
      default:
        break;
    }
  }

  public void OnToolClick(string toolId) {
    selectedToolId = toolId;
  }

  private void InstAttach_Pin(MapInst inst) {
    var pin = new Pin();
    pin.cell = inst.rootCell;
    inst.rootCell.pin = pin;
  }

  private void InstClick_Pin(MapInst inst, Cell cell) {
    connectingSourcePin = cell.pin;
    selectedToolId = "connecting";
  }
}