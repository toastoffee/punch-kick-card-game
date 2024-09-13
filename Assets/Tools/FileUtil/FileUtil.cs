using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class FileUtil {
#if UNITY_EDITOR
  public static string CreateAssetDatabaseTypeFilter<T>() where T : UnityEngine.Object {
    var typeName = typeof(T).ToString();
    if (typeName.StartsWith("UnityEngine.")) {
      typeName = typeName.Split('.').Last();
    }
    return string.Format("t:{0}", typeName);
  }

  public static List<string> FindAssetPathsOfType<T>(string folder, bool recursive = false) where T : UnityEngine.Object {
    folder = folder.TrimEnd('/');
    var arr = new string[] {
      folder
    };
    var guids = AssetDatabase.FindAssets(CreateAssetDatabaseTypeFilter<T>(), arr);
    var assetPaths = new List<string>();
    foreach (var guid in guids) {
      var path = AssetDatabase.GUIDToAssetPath(guid);
      TryParsePath(path, out var directory, out _);
      if (!recursive && directory != folder) {
        continue;
      }
      if (!string.IsNullOrEmpty(path)) {
        assetPaths.Add(path);
      }
    }
    return assetPaths;
  }

  public static bool TryParsePath(string path, out string diretory, out string fileName) {
    diretory = null;
    fileName = null;
    var indexOfSlash = path.LastIndexOf('/');
    if (indexOfSlash < 0) {
      return false;
    }
    diretory = path.Substring(0, indexOfSlash);
    fileName = path.Substring(indexOfSlash + 1);
    return true;
  }
  #endif
}
