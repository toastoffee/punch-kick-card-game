using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public static class ReflectionUtils {
  public static bool HasAttribute<T>(this Type type) {
    if (type == null) {
      return false;
    }
    return type.IsDefined(typeof(T), false);
  }

  public static IEnumerable<Type> AllTypes() {
    return Assembly.GetExecutingAssembly().GetTypes();
  }
}