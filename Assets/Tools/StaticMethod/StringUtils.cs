using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringUtils {
  public static string WrapColor(this string str, Color color) {
    str = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{str}</color>";
    return str;
  }
}