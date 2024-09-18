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
    public event Action<MapInst, CircuitUpdateContext> onCircuitUpdate;
    public event Action<MapInst> onDelete;

    public bool CheckCircuitUpdate(CircuitUpdateContext ctx) {
      if (!updateSeq.Check(ctx.updateSeqNum)) {
        return false;
      }

      onCircuitUpdate?.Invoke(this, ctx);
      return true;
    }

    public void NotifyDelete() {
      this.onDelete?.Invoke(this);
      Instance.m_mapInsts.Remove(instId);
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
  
  public class Pipe_State {
    
    public Pipe_State[] connect_pipes = new Pipe_State[2];

    // machine params
    public Pipe_State inPipe, outPipe;
    public int inPipeId, outPipeId;
    
    public int connectedCnt {
      get {
        var cnt = 0;
        for (int i = 0; i < 2; i++) {
          cnt += connect_pipes[i] == null ? 0 : 1;
        }
        return cnt;
      }
    }
    public int nextConnectIdx {
      get {
        for (int i = 0; i < 2; i++) {
          if (connect_pipes[i] == null) {
            return i;
          }
        }
        return -1;
      }
    }

    public MapInst self;
    public int portFlag { get; private set; }

    public SeqNumChecker circuitSeq;

    public bool Contain(Pipe_State other) {
      if (self.model.modelId == "pipe")
      {
        foreach (var pipe in connect_pipes) {
          if (pipe == other) {
            return true;
          }
        }
        return false; 
      }
      else if (self.model.modelId == "machine")
      {
        if (inPipe == other || outPipe == other)
        {
          return true;
        }
        return false;
      }
      return false;
    }

    public void Remove(Pipe_State other) {

      if (self.model.modelId == "pipe")
      {
        for (int i = 0; i < connect_pipes.Length; i++) {
          if (connect_pipes[i] == other) {
            connect_pipes[i] = null;
          }
        }  
      }
      else if (self.model.modelId == "machine")
      {
        if (inPipe == other)
        {
          inPipe = null;
        }
        if (outPipe == other)
        {
          outPipe = null;
        }
      }
      
    }

    public void OnDelete(MapInst inst) {

      if (self.model.modelId == "pipe")
      {
        foreach (var pipe in connect_pipes) {
          if (pipe == null) {
            continue;
          }
          pipe.Remove(this);
          this.Remove(pipe);
        }
      }else if (self.model.modelId == "machine")
      {
        if (inPipe != null)
        {
          inPipe.Remove(this);
          this.Remove(inPipe);
        }
        if (outPipe != null)
        {
          outPipe.Remove(this);
          this.Remove(outPipe);
        }
      }
    }

    public void OnCircuitUpdate(MapInst inst, CircuitUpdateContext ctx) {

      if (self.model.modelId == "pipe")
      {
        portFlag = 0;
        foreach (var cell in self.rootCell.Near4Area()) {
          if (connectedCnt >= 2) {
            break;
          }
          if (cell == null) {
            continue;
          }
          if (cell.inst != null && cell.inst.model.modelId == "pipe") {
            var otherPipe = cell.inst.data.pipeState;
            if (otherPipe.Contain(this) || otherPipe.connectedCnt >= 2) {
              continue;
            }
            connect_pipes[nextConnectIdx] = otherPipe;
            otherPipe.connect_pipes[otherPipe.nextConnectIdx] = this;
          }
        }
        foreach (var pipe in connect_pipes) {
          if (pipe == null) {
            continue;
          }
          SetPortFlag(pipe.self.rootCell.pos - self.rootCell.pos);
        }
        self.rootCell.NotifyDraw();
      }
      else if (self.model.modelId == "machine")
      {
        int idx = 0;    // up = 0, right = 1, down = 2, left = 3
        foreach (var cell in self.rootCell.Near4Area()) {
          if (inPipe != null && outPipe != null) {
            break;
          }
          if (cell == null) {
            continue;
          }

          if (cell.inst != null && cell.inst.model.modelId == "pipe")
          {
            if (idx == inPipeId && inPipe == null)
            {
              var otherPipe = cell.inst.data.pipeState;
              if (otherPipe.Contain(this) || otherPipe.connectedCnt >= 2) {
                continue;
              }
              inPipe = otherPipe;
              otherPipe.connect_pipes[otherPipe.nextConnectIdx] = this;
            } 
            else if (idx == outPipeId && outPipe == null)
            {
              var otherPipe = cell.inst.data.pipeState;
              if (otherPipe.Contain(this) || otherPipe.connectedCnt >= 2) {
                continue;
              }
              outPipe = otherPipe;
              otherPipe.connect_pipes[otherPipe.nextConnectIdx] = this;
            }
          }

          idx++;
        }
      }
      
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
        inst.CheckCircuitUpdate(this);
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
          onAttach = InstAttach_Pipe,
        }
      },
      {
        "machine",
        new MapInstModel()
        {
          sprId = "machine",
          onAttach = InstAttach_Machine,
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

  public void NotifyUpdateCircuit() {
    circuitUpdateContext.NotifyUpdate(m_mapInsts.Values);
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

  public MapInst CreatMapInst(string modelId) {
    var model = mapInstModel[modelId];

    var inst = new MapInst {
      instId = nextInstId++,
      model = model
    };

    m_mapInsts.Add(inst.instId, inst);
    
    return inst;
  }

  private void PlantMachineOnCell(Cell cell)
  {
    if (cell.inst != null) {
      return;
    }

    var inst = CreatMapInst("machine");
    cell.inst = inst;
    inst.rootCell = cell;
    
    inst.model.onAttach?.Invoke(inst);
    cell.NotifyDraw();
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
      case "debug":
        break;
      case "pipe":
        PlantInstOnCell(cell, "pipe");
        break;
      case "machine":
        PlantMachineOnCell(cell);
        break;
      case "delete":
        DeleteInstOnCell(cell);
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
    inst.onCircuitUpdate += selfPipe.OnCircuitUpdate;
    inst.onDelete += selfPipe.OnDelete;
    circuitUpdateContext.NotifyUpdate(m_mapInsts.Values);
  }
  
  private void InstAttach_Machine(MapInst inst) {
    var selfPipe = new Pipe_State() {
      self = inst,
      inPipeId = 0,
      outPipeId = 2,
    };
    inst.data.pipeState = selfPipe;
    inst.onCircuitUpdate += selfPipe.OnCircuitUpdate;
    inst.onDelete += selfPipe.OnDelete;
    circuitUpdateContext.NotifyUpdate(m_mapInsts.Values);
  }
  
}