using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Object = UnityEngine.Object;
using UObject = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
using UFileUtil = UnityEditor.FileUtil;
using Sirenix.OdinInspector;
#endif

[CreateAssetMenu(menuName = "SO/Auto Loader")]
public class AutoLoader : ScriptableObject {
  public const string FILE_NAME = "#AutoLoader";
  [HideInInspector]
  public bool isInited = false;
  [HideInInspector]
  public int normalCountLimit = 100;
  public List<ObjectGroup> objectGroups = new();

  [Serializable]
  public class ObjectGroup {
    public string objectType;
    public List<UObject> objects;
  }

  private void Clear() {
    if (objectGroups == null) {
      return;
    }
    objectGroups.Clear();
  }

  private T FindObjectByCondition<T>(Predicate<UObject> condition) where T : UObject {
    var typeStr = typeof(T).ToString();
    var group = objectGroups.Find(x => x.objectType == typeStr);
    if (group == null) {
      return null;
    }
    #if UNITY_EDITOR
    var cnt = group.objects.FindAll(condition).Count;
    if (cnt > 1) {
      Debug.LogError("你的载入行为匹配到了多个资源，请联系崔斯特轮子委员会");
    }
    #endif
    var ret = group.objects.Find(condition);
    return ret as T;
  }

  public T Load<T>(string id) where T : UObject {
    return FindObjectByCondition<T>(x => x.name == id);
  }
  
  /// <summary>
  /// 可以用"abc_123"匹配“abc”，也可以用"abc"匹配"abc_123"
  /// </summary>
  /// <param name="pattern"></param>
  /// <typeparam name="T"></typeparam>
  /// <returns></returns>
  public T LoadByPrefixMatch<T>(string pattern) where T : UObject {
    return FindObjectByCondition<T>(x => x.name.StartsWith(pattern) || pattern.StartsWith(x.name));
  }

  public bool TryLoad<T>(string id, out T res) where T : UObject {
    res = Load<T>(id);
    if (res == null) {
      Debug.LogError($"[AutoLoader] null : {id}");
    }
    return res != null;
  }

#if UNITY_EDITOR
  [Button("Reload")]
  public void EditorOnlyReload() {
    Clear();
    var selfPath = AssetDatabase.GetAssetPath(this);
    FileUtil.TryParsePath(selfPath, out var diretory, out _);
    var newName = "#" + diretory.Replace('/', '_');
    AssetDatabase.RenameAsset(selfPath, newName);
    name = newName;

    FileUtil.TryParsePath(selfPath, out var direcotry, out _);
    var allUObjectPath = FileUtil.FindAssetPathsOfType<UObject>(direcotry);
    if (allUObjectPath.SafeCount() > normalCountLimit) {
      Debug.Log("[AutoObjectCache] Hit liimt. check direcotry");
      return;
    }

    foreach (var path in allUObjectPath) {
      var obj = AssetDatabase.LoadAssetAtPath<UObject>(path);

      if (obj == this) {
        continue;
      }
      AddObject(obj);
    }

    isInited = true;
  }

  public static AutoLoader EditorOnlyCreateInstance(string path) {
    var newInst = ScriptableObject.CreateInstance(typeof(AutoLoader));
    AssetDatabase.CreateAsset(newInst, path + "/" + FILE_NAME);
    return AssetDatabase.LoadAssetAtPath<AutoLoader>(path + "/" + FILE_NAME);
  }

  public void EditorOnlyAddObject(List<UObject> objects) {
    foreach (var obj in objects) {
      if (obj == null) {
        continue;
      }
      AddObject(obj);
    }
  }

  public void EditoryOnlyRemoveObjects(List<UObject> objects) {
    foreach (var obj in objects) {
      RemoveObject(obj);
    }
  }

  private void AddObject(UObject obj) {
    var type = obj.GetType();
    if (type == typeof(AutoLoader)) {
      return;
    }
    if (type == typeof(UnityEditor.DefaultAsset)) {
      return;
    }

    #region Special Rules
    if (obj is Texture2D tex2d) {
      var path = AssetDatabase.GetAssetPath(obj);
      var texImpoter = TextureImporter.GetAtPath(path) as TextureImporter;

      //Handle Sprite and Multi-Sprite
      if (texImpoter.textureType == TextureImporterType.Sprite) {
        //Override type to sprite
        type = typeof(Sprite);
        //try load multi-sprite
        if (texImpoter.spriteImportMode == SpriteImportMode.Multiple) {
          var subSpritesObj = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
          foreach (var subObj in subSpritesObj) {
            _DoAddObject(subObj, type);
          }
          return; //skip load self, if sprite is multi
        } else {
          obj = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
      }
    }
    #endregion

    _DoAddObject(obj, type);
  }

  private void _DoAddObject(Object obj, Type type) {
    var group = objectGroups.Find(x => x.objectType == type.ToString());
    if (group == null) {
      group = new ObjectGroup() {
        objectType = type.ToString(),
        objects = new List<UObject>(),
      };
      objectGroups.Add(group);
    }
    if (group.objects == null) {
      group.objects = new();
    }
    if (group.objects.Contains(obj)) {
      return;
    }
    group.objects.Add(obj);
  }

  private void RemoveObject(UObject obj) {
    if ((object)obj == null) {
      return;
    }
    var type = obj.GetType();
    var group = objectGroups.Find(x => x.objectType == type.ToString());
    if (group == null || group.objects.SafeCount() == 0) {
      return;
    }
    group.objects.Remove(obj);
  }
#endif
}

#if UNITY_EDITOR
class AutoObjectCachePostprocessor : AssetPostprocessor {
  private const bool TOGGLE = true;

  private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload) {
    if (!TOGGLE) {
      return;
    }

    var addedPath = new List<string>(importedAssets);
    addedPath.AddRange(deletedAssets);

    var removedPath = new List<string>(movedAssets);
    removedPath.AddRange(movedFromAssetPaths);

    Dictionary<string, List<UObject>> addedDict = new();
    foreach (var path in addedPath) {
      if (!FileUtil.TryParsePath(path, out var directory, out var fileName)) {
        return;
      }
      if (!addedDict.TryGetValue(directory, out _)) {
        addedDict.Add(directory, new List<UObject>());
      }
      addedDict[directory].Add(AssetDatabase.LoadAssetAtPath<UObject>(path));
    }

    Dictionary<string, List<UObject>> removedDict = new();
    foreach (var path in removedPath) {
      if (!FileUtil.TryParsePath(path, out var directory, out var fileName)) {
        return;
      }
      if (!removedDict.TryGetValue(directory, out _)) {
        removedDict.Add(directory, new List<UObject>());
      }
      removedDict[directory].Add(AssetDatabase.LoadAssetAtPath<UObject>(path));
    }

    foreach (var pair in addedDict) {
      if (!TryFindCache(pair.Key, out var cache)) {
        continue;
      }

      if (!cache.isInited) {
        cache.EditorOnlyReload();
      } else {
        cache.EditorOnlyAddObject(pair.Value);
      }
    }

    foreach (var pair in removedDict) {
      if (!TryFindCache(pair.Key, out var cache)) {
        continue;
      }

      if (!cache.isInited) {
        cache.EditorOnlyReload();
      } else {
        cache.EditoryOnlyRemoveObjects(pair.Value);
      }
    }
  }

  private static bool TryFindCache(string directory, out AutoLoader cache) {
    cache = null;
    var foundPaths = FileUtil.FindAssetPathsOfType<AutoLoader>(directory);
    if (foundPaths.SafeCount() == 0) {
      return false;
    }
    if (foundPaths.SafeCount() > 1) {
      Debug.LogError($"[AutoObjectCache] Multi Cache found in : {directory}");
      return false;
    }

    cache = AssetDatabase.LoadAssetAtPath<AutoLoader>(foundPaths[0]);
    if (cache == null) {
      Debug.LogError($"[AutoObjectCache] Invalid Cast : {foundPaths[0]}");
      return false;
    }
    return true;
  }
}
#endif