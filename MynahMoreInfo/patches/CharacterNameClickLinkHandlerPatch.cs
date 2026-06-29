using Game.Components.Character.LifeRecord;
using HarmonyLib;
using MynahMoreInfo.utils;

namespace MynahMoreInfo.patches;

[HarmonyPatch]
public class CharacterNameClickLinkHandlerPatch
{
    /**
     * 详细经历界面的人物链接 增加人物浮窗
     */
    [HarmonyPatch(
        typeof(NameButton),
        nameof(NameButton.Set))
    ]
    [HarmonyPostfix]
    static void SetPostfix(NameButton __instance, int charId)
    {
        if (!ModEntry.MTC_CharacterNameClickLink) return;
        var dp = __instance.GetComponent<TooltipInvoker>();
        if (dp == null) return;

        Util.EnableMouseTipCharacter(dp, charId, true);
    }
}