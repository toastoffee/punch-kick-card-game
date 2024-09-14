using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheFactoryGame : MonoSingleton<TheFactoryGame> {
  public RectTransform cellContainer;
  public GameObject cellPrefab;
  public Vector2Int mapSize;

  private Cell[,] m_cells;
  private int nextInstId = 0;
  private Dictionary<int, MapInst> m_mapInsts;

  public class Cell {
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
      yield return map.SafeGet(pos + Vector2Int.up);
      yield return map.SafeGet(pos + Vector2Int.right);
      yield return map.SafeGet(pos + Vector2Int.down);
      yield return map.SafeGet(pos + Vector2Int.left);
    }
  }

  public class MapInst {
    public int instId;
    public Cell rootCell;
    public MapInstModel model;
    public Dictionary<string, object> data = new Dictionary<string, object>();
  }

  public delegate void MapInstOnAttach(MapInst mapInst);

  public class MapInstModel {
    public string modelId;
    public string sprId;
    public MapInstOnAttach onAttach;
  }

  public class Pipe_State {
    public MapInst self;
    public MapInst[] ports = new MapInst[2];
    public int portFlag;
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
          if (ports[i] != null) {
            return i;
          }
        }
        return -1;
      }
    }

    public void ConnectTo(Pipe_State other) {
      if (other == null) {
        return;
      }
      if (connectedCount >= 2 || other.connectedCount >= 2) {
        return;
      }
      ports[availPortIdx] = other.self;
      other.ports[availPortIdx] = self;

      UpdateConnect();
      other.UpdateConnect();
    }

    private void UpdateConnect() {

    }
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
        "pipe",
        new MapInstModel {
          sprId = "pipe",
          onAttach = InstAttach_Pipe
        }
      }
    };

    foreach (var pair in mapInstModel) {
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

  private void PlantInstOnCell(Cell cell) {
    if (cell.inst != null) {
      return;
    }

    var inst = CreatMapInst("pipe");
    cell.inst = inst;
    inst.rootCell = cell;
    inst.model.onAttach?.Invoke(inst);
    cell.NotifyDraw();
  }

  public void OnCellClick(Cell cell) {
    PlantInstOnCell(cell);
  }

  private void InstAttach_Pipe(MapInst inst) {
    inst.data["pipe"] = new Pipe_State() {
      self = inst
    };

    foreach (var cell in inst.rootCell.Near4Area()) {
      if (cell == null) {
        return;
      }
      if (cell.inst != null && cell.inst.model.modelId == "pipe") {

      }
    }
  }
}