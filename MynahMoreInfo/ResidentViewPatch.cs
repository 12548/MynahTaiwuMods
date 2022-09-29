using HarmonyLib;
using UICommon.Character.Elements;
using UnityEngine.UI;

namespace MynahMoreInfo;

[HarmonyPatch]
public class ResidentViewPatch
{
    [HarmonyPatch(typeof(ResidentView), "RenderCharInfoNew")]
    [HarmonyPostfix]
    static void RenderCharInfoNewPostfix(ResidentView __instance, int charId)
    {
        if (charId < 1) return;
        EnsureCharMouseTip(__instance, charId);
    }

    [HarmonyPatch(typeof(ResidentView), "RenderExpandCharInfo")]
    [HarmonyPostfix]
    static void RenderExpandCharInfo(ResidentView __instance, int charId)
    {
        if (charId < 1) return;
        EnsureCharMouseTip(__instance, charId);
    }

    private static void EnsureCharMouseTip(ResidentView __instance, int charId)
    {
        if (!MynahMoreInfo.ModEntry.ShowResidentCharacterMouseTip) return;
        var transform = __instance.transform.Find("CharInfoHolder");
        if (transform == null) return;
        var mouseTipDisplayer = Util.EnsureMouseTipDisplayer(transform.gameObject);
        Util.EnableMouseTipCharacter(mouseTipDisplayer, charId);
    }
}