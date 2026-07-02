using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace MynahMoreInfo.patches;

[HarmonyPatch]
public class TMPTextSpriteHelperPatch
{
    static IEnumerable<MethodBase> TargetMethods()
    {
        // 1. 获取目标类的类型
        var originalType = typeof(TMPTextSpriteHelper);
        
        // 2. 查找编译器为异步方法生成的私有状态机嵌套类型（通常命名类似 "<YourAsyncMethodName>d__0"）
        var stateMachineType = originalType.GetNestedTypes(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        var stateMachineT = stateMachineType.Where(t => t.Name.Contains("HandleSpriteReplace"));
        
        foreach (var s in stateMachineType)
        {
            // 3. 返回该状态机类里的 MoveNext 方法，对其进行打补丁
            var methodInfo = s.GetMethod("MoveNext", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (methodInfo == null) continue;
            yield return methodInfo;
        }

    }
    
    static Exception Finalizer()
    {
        return null; // suppresses all exceptions
    }
}