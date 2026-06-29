using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace MynahMoreInfo.patches;

[HarmonyPatch]
public class TMPTextSpriteHelperPatch
{
    static MethodBase TargetMethod()
    {
        // 1. 获取目标类的类型
        var originalType = typeof(TMPTextSpriteHelper);
        
        // 2. 查找编译器为异步方法生成的私有状态机嵌套类型（通常命名类似 "<YourAsyncMethodName>d__0"）
        var stateMachineType = originalType.GetNestedTypes(BindingFlags.Instance)
            .FirstOrDefault(t => t.Name.Contains("HandleSpriteReplace"));

        // 3. 返回该状态机类里的 MoveNext 方法，对其进行打补丁
        return stateMachineType.GetMethod("MoveNext", BindingFlags.Instance);
    }
    
    static Exception Finalizer()
    {
        return null; // suppresses all exceptions
    }
}