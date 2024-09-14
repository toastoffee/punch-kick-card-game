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

  public TMP_Text selectedToolText;

  private Cell[,] m_cells;
  private int nextInstId = 0;
  private Dictionary<int, MapInst> m_mapInsts;
  private string selectedToolId;

  public class Cell {
    public static Vector2Int[] near4 = new Vector2Int[] {
      Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left
    };

    public GameObject gameObject;
    public Vector2Int pos;

    public MapInst inst;
    public Vector2Int instOffset;

    public CellPort[] ports = new CellPort[4];

    public int drawSeq;

    public int x {
      get => pos.x; set => pos.x = value;
    }
    public int y {
      get => pos.y; set => pos.y = value;
    }

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
    public Pipe_State pipeState;
  }
  public class MapInst {
    public int instId;
    public Cell rootCell;
    public MapInstModel model;
    public MapInstData data = new MapInstData();

    public SeqNumChecker updateSeq;
    public event Action<CircuitUpdateContext> onCircuitUpdate;


    public bool CheckCircuitUpdate(CircuitUpdateContext ctx) {
      if (!updateSeq.Check(ctx.updateSeqNum)) {
        return false;
      }

      onCircuitUpdate?.Invoke(ctx);
      return true;
    }

    public IEnumerable<MapInst> FindNeightbors() {
      foreach (var port in rootCell.ports) {
        if (port == null) {
          continue;
        }
        var neightbor = port.GetOther(rootCell);
        if (neightbor != null && neightbor.inst != null) {
          yield return neightbor.inst;
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
  }

  public class CellPort {
    public Cell[] cells = new Cell[2];

    public Cell GetOther(Cell cell) {
      for (int i = 0; i < cells.Length; i++) {
        if (cells[i] != null && cells[i] != cell) {
          return cells[i];
        }
      }
      return null;
    }

    public void Set(Cell cell0, Cell cell1) {
      cells[0] = cell0;
      cells[1] = cell1;
    }
  }

  public class CircuitPort {
    private static List<CircuitPort> allPorts = new List<CircuitPort>();
    public MapInst selfInst;
    public MapInst otherInst;
    public Vector2Int offset;

    public static CircuitPort Create(MapInst other, Vector2Int selfPos, Vector2Int otherPos) {
      var res = new CircuitPort {
        selfInst = other,
        offset = otherPos - selfPos,
        otherInst = other
      };
      allPorts.Add(res);
      return res;
    }
  }

  public class Pipe_State {
    private CircuitPort[] ports = new CircuitPort[2];

    public MapInst self;
    public int portFlag { get; private set; }
    public int connectedCount {
      get {
        var cnt = 0;
        for (int i = 0; i < 2; i++) {
          if (ports[i] != null) {
            cnt++;
          }
        }
        return cnt;
      }
    }
    public int availPortIdx {
      get {
        for (int i = 0; i < 2; i++) {
          if (ports[i] == null) {
            return i;
          }
        }
        return -1;
      }
    }

    public void ConnectTo(Pipe_State other, Vector2Int selfPos, Vector2Int otherPos) {
      if (other == null) {
        return;
      }
      if (connectedCount >= 2 || other.connectedCount >= 2) {
        return;
      }
      ports[availPortIdx] = CircuitPort.Create(other.self, selfPos, otherPos);
      other.ports[other.availPortIdx] = CircuitPort.Create(self, otherPos, selfPos);

      UpdateConnect();
      other.UpdateConnect();
    }

    private void UpdateConnect() {
      portFlag = 0;
      for (int i = 0; i < 2; i++) {
        var port = ports[i];
        if (port == null || port.selfInst == null) {
          continue;
        }
        SetPortFlag(port.offset);
      }
      self.rootCell.NotifyDraw();
    }

    private void SetPortFlag(Vector2Int offset) {
      var bit = -1;
      if (offset == Vector2.up) {
        bit = 0;
      } else if (offset == Vector2.right) {
        bit = 1;
      } else if (offset == Vector2.down) {
        bit = 2;
      } else if (offset == Vector2.left) {
        bit = 3;
      }
      if (bit > -1) {
        portFlag |= 1 << bit;
      }
    }
  }

  public class CircuitUpdateContext {
    public int updateSeqNum;
    private static Queue<MapInst> queueBuffer = new Queue<MapInst>();

    public void NotifyUpdate(IEnumerable<MapInst> allInst) {
      updateSeqNum++;
      foreach (var inst in allInst) {
        if (inst.CheckCircuitUpdate(this)) {
          queueBuffer.Enqueue(inst);
          while (queueBuffer.Count > 0) {
            var next = queueBuffer.Dequeue();
            foreach (var neightbor in next.FindNeightbors()) {
              if (neightbor.CheckCircuitUpdate(this)) {
                queueBuffer.Enqueue(neightbor);
              }
            }
          }
        }
      }
    }
  }
  private CircuitUpdateContext circuitUpdateContext = new CircuitUpdateContext();

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
        "pipe",
        new MapInstModel {
          sprId = "pipe",
          onAttach = InstAttach_Pipe
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
    InitCellPorts();
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
        };

        cellView.SetCell(cell);

        m_cells[x, y] = cell;
      }
    }
  }

  private void InitCellPorts() {
    foreach (var cell in m_cells) {
      for (int i = 0; i < 4; i++) {
        if (cell.ports[i] != null) {
          continue;
        }
        var port = new CellPort();
        var offset = Cell.near4[i];
        var other = m_cells.SafeGet(cell.pos + offset);
        if (other == null) {
          continue;
        }
        port.Set(cell, other);
        cell.ports[i] = port;
        other.ports[(i + 2) % 4] = port;
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

  private void PlantInstOnCell(Cell cell, string instModelId) {
    if (cell.inst != null) {
      return;
    }

    var inst = CreatMapInst(instModelId);
    cell.inst = inst;
    inst.rootCell = cell;
    inst.model.onAttach?.Invoke(inst);
    cell.NotifyDraw();
  }

  public void OnCellClick(Cell cell) {
    switch (selectedToolId) {
      case "debug":
        break;
      case "pipe":
        PlantInstOnCell(cell, "pipe");
        break;
      default:
        break;
    }
  }

  public void OnToolClick(string toolId) {
    selectedToolId = toolId;
    selectedToolText.text = toolId;
  }

  private void InstAttach_Pipe(MapInst inst) {
    var selfPipe = new Pipe_State() {
      self = inst
    };
    inst.data.pipeState = selfPipe;

    foreach (var cell in inst.rootCell.Near4Area()) {
      if (cell == null) {
        return;
      }
      if (cell.inst != null && cell.inst.model.modelId == "pipe") {
        var otherPipe = cell.inst.data.pipeState;
        selfPipe.ConnectTo(otherPipe, inst.rootCell.pos, cell.pos);
      }
    }
    circuitUpdateContext.NotifyUpdate(m_mapInsts.Values);
  }
}