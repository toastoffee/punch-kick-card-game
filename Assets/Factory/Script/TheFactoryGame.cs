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

  }

  public class MapInst {
    public int instId;
    public Cell rootCell;
    public MapInstModel model;
    public MapInstData data = new MapInstData();

    public event Action<MapInst> onCircuitUpdate;
    public event Action<MapInst> onDelete;

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
          onAttach = InstAttach_Pipe,
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

  private void PlantMachineOnCell(Cell cell, int inPipeId, int outPipeId) {
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
      case "machine-left-right":
        PlantMachineOnCell(cell, 3, 1);
        break;
      case "machine-up-down":
        PlantMachineOnCell(cell, 0, 2);
        break;
      case "machine-right-left":
        PlantMachineOnCell(cell, 1, 3);
        break;
      case "machine-down-up":
        PlantMachineOnCell(cell, 2, 0);
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

  }

}