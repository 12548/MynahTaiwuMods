using FrameWork;
using HarmonyLib;
using UnityEngine;

namespace MynahMoreInfo;

[HarmonyPatch]
public class MouseTipDisplayerPatch
{
    /// <summary>
    /// 检查地块浮窗是否应该显示
    /// </summary>
    [HarmonyPrefix, HarmonyPatch(typeof(MouseTipDisplayer), "CanShowTips")]
    static bool CanShowTipsPrefix(MouseTipDisplayer __instance, ref bool __result, ArgumentBox ___RuntimeParam)
    {
        if (___RuntimeParam != null && ___RuntimeParam.Get("_mmi_require_alt", out bool requireAlt))
        {
            if (requireAlt && !(Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)))
            {
                __result = false;
                return false;
            }
        }
        
        if (ModEntry.MapBlockMouseTipStat == 2) return true;
        if (__instance.Type != TipType.MapBlock) return true;
        __result = false;
        if (ModEntry.MapBlockMouseTipStat == 0) return false;

        if (!(Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)))
        {
            return false;
        }
        
        __result = true;
        return true;
    }
}