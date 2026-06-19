using System.Collections.Generic;
using Game.Views.Bottom;
using GameData.Utilities;
using HarmonyLib;
using UnityEngine;

namespace MynahMoreInfo;

[HarmonyPatch]
public class UIBottomPatch
{
    
    [HarmonyPatch(typeof(ViewBottom), nameof(ViewBottom.RefreshCharacterAvatar))]
    [HarmonyPostfix]
    public static void UpdateCombatTeammatePostfix(ViewBottom __instance)
    {
        Debug.Log($"Updating Combat Teammate");

        for (int i = 0; i < 3; i++)
        {
            List<int> combatTeamCharIds = SingletonObject.getInstance<CharacterMonitorModel>().GetTaiwuCombatTeamCharIds();
            var charId = combatTeamCharIds.GetOrDefault(i + 1, -1);
            {
                var component = __instance.transform.Find($"AnimationRoot/Teammate{i+1}/Bg");
                if(component == null) return;
                Util.EnableMouseTipCharacter(Util.EnsureMouseTipDisplayer(component.gameObject), charId);
            }
        }
        
        
        //
        //
        // // var component = __instance._groupChar.CGet<RectTransform>("CombatCharHolder").Find(index.ToString()).GetComponent<Refers>();
        // var component = __instance._groupChar.CGet<RectTransform>("CombatCharHolder").Find($"{index}/Avatar");
        // bool isExist = charId >= 0;
        // // var avatar = component.CGet<UICommon.Character.Avatar.Avatar>("Avatar");
        //
        // var mouseTipDisplayer = component.gameObject.GetOrAddComponent<TooltipInvoker>();
        // mouseTipDisplayer.enabled = isExist;
        //
        // if (isExist)
        // {
        //     Util.EnableMouseTipCharacter(mouseTipDisplayer, charId);
        // }
    }
}