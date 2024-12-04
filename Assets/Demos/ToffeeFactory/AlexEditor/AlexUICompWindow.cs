using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
// using Sirenix.OdinInspector.Editor;


// public class AlexUICompWindow : OdinEditorWindow {
//   [Serializable]
//   public struct FilterContext {
//     [Serializable]
//     public struct ResultItem {
//       [ReadOnly]
//       public string name;
//       public Transform transform;
//
//       [Button("Focus")]
//       public void Focus() {
//         Selection.activeTransform = transform;
//       }
//     }
//     public List<ResultItem> filterResults;
//     public List<Filter> filters;
//
//     public static FilterContext FilterOn(Transform transform, List<Filter> filters) {
//       var res = new FilterContext() {
//         filterResults = new List<ResultItem>(),
//         filters = filters,
//       };
//       IterFunc(transform, ref res);
//       res.filterResults.Sort((a, b) => a.name.CompareTo(b.name));
//       return res;
//     }
//
//     private static void IterFunc(Transform cur, ref FilterContext context) {
//       for (int j = 0; j < context.filters.Count; j++) {
//         var filter = context.filters[j];
//         if (filter.DoFilter(cur)) {
//           context.filterResults.Add(new ResultItem {
//             name = filter.Name,
//             transform = cur,
//           });
//         }
//       }
//
//       for (int i = 0, cnt = cur.childCount; i < cnt; i++) {
//         var next = cur.GetChild(i);
//         IterFunc(next, ref context);
//       }
//     }
//   }
//
//   public abstract class Filter {
//     public abstract string Name { get; }
//     public abstract bool DoFilter(Transform cur);
//   }
//
//   public class RichTextFilter : Filter {
//     public override string Name => "Rich Text";
//     public override bool DoFilter(Transform cur) {
//       var text = cur.GetComponent<Text>();
//       return text != null && text.supportRichText;
//     }
//   }
//
//   public class RaycastTargetFilter : Filter {
//     public override string Name => "Raycast Target";
//     public override bool DoFilter(Transform cur) {
//       var graphic = cur.GetComponent<Graphic>();
//       return graphic != null && graphic.raycastTarget;
//     }
//   }
//
//   [MenuItem("Alex UI/UI Comp Window")]
//   private static void OpenWindow() {
//     GetWindow<AlexUICompWindow>().Show();
//   }
//
//   private static List<Filter> filters = new List<Filter>(){
//     new RichTextFilter(),
//   };
//
//   public GameObject prefab;
//   [TableList]
//   public List<FilterContext.ResultItem> filterResults;
//
//   [Button("Check")]
//   public void BtnCheck() {
//     if(prefab == null) {
//       return;
//     }
//     var ctx = FilterContext.FilterOn(prefab.transform, filters);
//     filterResults = ctx.filterResults;
//   }
// }
