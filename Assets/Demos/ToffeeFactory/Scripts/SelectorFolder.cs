using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace ToffeeFactory {
  public class SelectorFolder : MonoBehaviour {

    private List<Transform> units = new List<Transform>();

    [SerializeField]
    private float unitInterval;

    private int unitCount => units.Count;

    [SerializeField]
    private float duration;

    [SerializeField]
    private float yOffset;


    public void AddUnit(Transform unit) {
      units.Add(unit);
      unit.parent = transform;
    }
    
    public void Fold() {
      foreach (var unit in units) {
        unit.DOScale(Vector3.zero, duration);
        unit.DOMove(transform.position, duration);
      }
    }

    public void Unfold() {
      for (int i = 0; i < units.Count; i++) {
        float offset = (unitCount - 1) / -2f;

        var targetPos = transform.position 
                        + (i + offset) * unitInterval * Vector3.right 
                        + yOffset * Vector3.up;

        units[i].DOScale(Vector3.one, duration);
        units[i].DOMove(targetPos, duration);
      }
    }
    
  }
}