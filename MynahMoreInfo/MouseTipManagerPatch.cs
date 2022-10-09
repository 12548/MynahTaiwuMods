using System.Collections.Generic;
using System.Reflection;
using FrameWork;
using HarmonyLib;

namespace MynahMoreInfo;

[HarmonyPatch]
public class MouseTipManagerPatch
{
    /// <summary>
    /// 重写一遍，去除官方延迟
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch(typeof(MouseTipManager), "ShowTips")]
    public static bool ShowTips(
        MouseTipManager __instance,
        ref UIElement ____showingTips,
        Dictionary<TipType, UIElement> ____uiElementDict,
        TipType type,
        ArgumentBox argsBox,
        bool needRefresh = false,
        bool showOnLeft = false,
        bool showOnTop = false)
    {
        if (!ModEntry.DelayFix) return true;
        __instance.HideTips();
        var tipToShow = ____uiElementDict[type];
        ____showingTips = tipToShow;
        var flag = !tipToShow.IsWaitShowing;
        argsBox.Set("NeedRefresh", needRefresh);
        tipToShow.SetOnInitArgs(argsBox);
        tipToShow.OnListenerIdReady += () =>
        {
            if (__instance.GetType().GetField("_showingTips", (BindingFlags)(-1))!.GetValue(__instance) == tipToShow)
            {
                var uiBase = (MouseTipBase)tipToShow.UiBase;
                uiBase.ShowOnLeft = showOnLeft;
                uiBase.ShowOnTop = showOnTop;
            }
            else
                tipToShow.Hide();
        };
        if (!flag)
            return false;
        tipToShow.Show();
        return false;
    }
}