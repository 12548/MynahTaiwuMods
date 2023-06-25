using GameData.Domains.Character.Display;
using HarmonyLib;
using UICommon.Character.Avatar;

namespace MynahMoreInfo;

[HarmonyPatch]
public static class AvatarPatch
{
    // [HarmonyPatch(typeof(Avatar), "Refresh", typeof(CharacterDisplayData))]
    // static void RefreshPostfix(Avatar __instance, CharacterDisplayData displayData)
    // {
    //     var mouseTipDisplayer = Util.EnsureMouseTipDisplayer(__instance.gameObject);
    //     Util.EnableMouseTipCharacter(mouseTipDisplayer, displayData.CharacterId);
    // }
}