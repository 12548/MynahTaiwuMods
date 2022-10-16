using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using HarmonyLib;
using UICommon.Character.Elements;

namespace MynahMoreInfo;

[HarmonyPatch]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class ResidentViewPatch
{
    public static IEnumerable<MethodBase> TargetMethods()
    {
        // if possible use nameof() or SymbolExtensions.GetMethodInfo() here
        yield return AccessTools.Method(typeof(ResidentView), "RenderCharInfo");
        yield return AccessTools.Method(typeof(ResidentView), "RenderShopCharInfo");
        yield return AccessTools.Method(typeof(ResidentView), "RenderResidentCharInfo");
        yield return AccessTools.Method(typeof(ResidentView), "RenderExpandCharInfo");
    }
    
    public static void Postfix(ResidentView __instance, int charId)
    {
        if (charId < 1) return;
        EnsureCharMouseTip(__instance, charId);
    }

    private static void EnsureCharMouseTip(ResidentView __instance, int charId)
    {
        if (ModEntry.MouseTipResidentView == 0) return;
        var transform = __instance.transform.Find("CharInfoHolder");
        if (transform == null) return;
        var mouseTipDisplayer = Util.EnsureMouseTipDisplayer(transform.gameObject);
        Util.EnableMouseTipCharacter(mouseTipDisplayer, charId, ModEntry.MouseTipResidentView);
    }
}
