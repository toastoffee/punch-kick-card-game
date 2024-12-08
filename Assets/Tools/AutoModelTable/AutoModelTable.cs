using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using Sirenix.Utilities;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class AutoModelTableAttribute : Attribute {
  public Type modelType;

  public AutoModelTableAttribute(Type modelType) {
    this.modelType = modelType;
  }
}

public static class AutoModelTableImpl {
  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
  private static void OnInit() {
    var types = ReflectionUtils.AllTypes();

    var modelTableClasses =
      from type in types
      where type.IsClass && type.IsStatic() && type.HasAttribute<AutoModelTableAttribute>()
      select type;

    foreach (var type in modelTableClasses) {
      CollectModel(type);
    }
  }

  private static void CollectModel(Type staticDefineType) {
    var attr = staticDefineType.GetAttribute<AutoModelTableAttribute>();
    var modelType = attr.modelType;
    if (modelType.GetField("modelId") == null) {
      Debug.LogError($"[AutoModelTable] Model类型[{modelType}] 需要定义字段modelId才可被收集");
      return;
    }
    var tableType = BuildTableType(modelType);

    var staticInstType = typeof(StaticInst<>);
    var tableStaticInstType = staticInstType.MakeGenericType(tableType);
    var staticInst = Activator.CreateInstance(tableStaticInstType);

    var staticGetInstMethod = tableStaticInstType.GetProperty(nameof(StaticInst<int>.staticInst));
    var tableAddMethod = tableType.GetMethod(nameof(TemplateTable<int, int>.ReflectOnlyAdd));

    var tableInst = staticGetInstMethod.GetValue(staticInst);
    CollectModelFromType(staticDefineType, modelType, tableInst, tableAddMethod);
  }

  private static Type BuildTableType(Type modelType) {
    var templateTable = typeof(CommonTemplateTable<,>);
    var tableType = templateTable.MakeGenericType(typeof(string), modelType);
    return tableType;
  }

  private static void CollectModelFromType(Type defineType, Type modelType, object tableInst, MethodInfo addMethod) {
    var defines = from prop in defineType.GetProperties()
                  where prop.IsStatic() && prop.GetMethod != null && prop.PropertyType == modelType
                  select prop;
    var modelIdField = modelType.GetField("modelId");

    foreach (var prop in defines) {
      var modelObj = prop.GetMethod.Invoke(null, new object[] { });
      var modelId = modelIdField.GetValue(modelObj);
      addMethod.Invoke(tableInst, new object[] { modelId, modelObj } );
    }
  }
}