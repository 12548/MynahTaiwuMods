using System;
using System.Diagnostics;
using HarmonyLib;
using TaiwuModdingLib.Core.Plugin;

namespace MynahBaseModBackend;

public class TaiwuRemakeHarmonyPlugin : TaiwuRemakePlugin
{
    Harmony harmony;
    
    public override void Initialize()
    {
        harmony = new Harmony(PluginName);
        var assembly = new StackTrace().GetFrame(1).GetMethod().ReflectedType.Assembly;
        harmony.PatchAll(assembly);
    }

    public override void Dispose()
    {
        harmony.UnpatchSelf();
    }
}