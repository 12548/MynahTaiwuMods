using GameData.Domains.Character.Display;
using GameData.Domains.TaiwuEvent.DisplayEvent;
using HarmonyLib;

namespace MynahMoreInfo;

[HarmonyPatch]
public class UI_EventWindowPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(UI_EventWindow), "UpdateMainCharacter")]
    static void UpdateMainCharacterPostfix(UI_EventWindow __instance)
    {
        if (!ModEntry.ShowEventUICharacterMouseTip) return;
        var data = (TaiwuEventDisplayData)Traverse.Create(__instance).Property("Data").GetValue();
        CharacterDisplayData mainCharacter = data.MainCharacter;
        Refers refers = __instance.CGet<Refers>("MainCharacter");
        var transform = refers.transform.Find("MoveRoot/AvatarArea/ShowMainCharacterMenu");
        if (transform == null) return;
        var mouseTipObj = transform.gameObject;

        if (mainCharacter == null || !HasCharacter(__instance, true))
        {
            if (refers != null)
            {
                var mouseTipDisplayer = Util.EnsureMouseTipDisplayer(mouseTipObj);
                mouseTipDisplayer.enabled = false;
            }
        }
        else
        {
            var mouseTipDisplayer = Util.EnsureMouseTipDisplayer(mouseTipObj);
            Util.EnableMouseTipCharacter(mouseTipDisplayer, mainCharacter.CharacterId,
                ModEntry.ReplaceAllCharacterTipToDetail ? 2 : 1);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(UI_EventWindow), "UpdateTargetCharacter")]
    static void UpdateTargetCharacterPostfix(UI_EventWindow __instance)
    {
        if (!ModEntry.ShowEventUICharacterMouseTip) return;
        var data = (TaiwuEventDisplayData)Traverse.Create(__instance).Property("Data").GetValue();
        CharacterDisplayData targetCharacter = data.TargetCharacter;
        Refers refers = __instance.CGet<Refers>("TargetCharacter");
        var transform = refers.transform.Find("CanvasChanger/AvatarArea/ShowTargetCharacterMenu");
        if (transform == null) return;
        var mouseTipObj = transform.gameObject;

        if (targetCharacter == null || !HasCharacter(__instance, false))
        {
            if (refers != null)
            {
                var mouseTipDisplayer = Util.EnsureMouseTipDisplayer(mouseTipObj);
                mouseTipDisplayer.enabled = false;
            }
        }
        else
        {
            var mouseTipDisplayer = Util.EnsureMouseTipDisplayer(mouseTipObj);
            Util.EnableMouseTipCharacter(mouseTipDisplayer, targetCharacter.CharacterId,
                ModEntry.ReplaceAllCharacterTipToDetail ? 2 : 1);
        }
    }

    // [HarmonyReversePatch]
    // [HarmonyPatch(typeof(UI_EventWindow), "HasCharacter")]
    static bool HasCharacter(UI_EventWindow instance, bool left)
    {
        return (bool)AccessTools.Method(typeof(UI_EventWindow), "HasCharacter")
            .Invoke(instance, new object[] { left });
        // throw new Exception("stub!");
    }
}