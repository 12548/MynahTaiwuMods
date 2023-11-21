using GameData.Domains.CombatSkill;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace MynahMoreInfo;

[HarmonyPatch]
public class EventWindowCharacterPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(EventWindowCharacter), "Refresh")]
    public static void Postfix(EventWindowCharacter __instance)
    {
        Debug.Log("EventWindowCharacterPatch1");
        if (!ModEntry.ShowEventUICharacterMouseTip) return;
        Debug.Log("EventWindowCharacterPatch2");
        if (__instance != null)
        {
            Debug.Log("EventWindowCharacterPatch3");
            var mou = Util.EnsureMouseTipDisplayer(__instance.transform.Find("CanvasChanger/AvatarArea/ShowCharacterMenu").gameObject);
            mou.enabled = __instance.GetHasCharacter();
            if (mou.enabled)
            {
                Util.EnableMouseTipCharacter(mou,
                    __instance.IsLeftCharacter
                        ? __instance.Data.MainCharacter.CharacterId
                        : __instance.Data.TargetCharacter.CharacterId,
                    ModEntry.ReplaceAllCharacterTipToDetail ? 2 : 1);
                Debug.Log("EventWindowCharacterPatch4");
            }

            Debug.Log("EventWindowCharacterPatch5");
        }

        
    }
}