using System;
using System.Diagnostics;
using HarmonyLib;
using HarmonyLib.Tools;
using TaiwuModdingLib.Core.Plugin;
using Debug = UnityEngine.Debug;

namespace MynahBaseModFrontend;

public class TaiwuRemakeHarmonyPlugin : TaiwuRemakePlugin
{
    Harmony harmony;
    
    public override void Initialize()
    {
        harmony = new Harmony(PluginName);
        var assembly = new StackTrace().GetFrame(1).GetMethod().ReflectedType.Assembly;
        // Debug.Log("[MynahBaseModFrontend] Patching assembly: " + assembly.FullName);
        harmony.PatchAll(assembly);
        // Debug.Log($"[MynahBaseModFrontend] Patching Harmony {PluginName}");
        // var originalMethods = Harmony.GetAllPatchedMethods();
        // foreach (var method in originalMethods)
        // {
        //     Debug.Log($"[{PluginName}] Patched: {method.FullDescription()}");
        // }
    }

    public override void Dispose()
    {
        harmony.UnpatchSelf();
    }
}