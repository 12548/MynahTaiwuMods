using HarmonyLib;
using UnityEngine;

namespace MynahMoreInfo;

[HarmonyPatch]
public class UIBottomPatch
{
    
    [HarmonyPatch(typeof(UI_Bottom), "UpdateCombatTeammate", typeof(int), typeof(int))]
    [HarmonyPostfix]
    public static void UpdateCombatTeammatePostfix(int index, int charId, UI_Bottom __instance)
    {
        Debug.Log($"Updating Combat Teammate {charId}");
        // var component = __instance._groupChar.CGet<RectTransform>("CombatCharHolder").Find(index.ToString()).GetComponent<Refers>();
        var component = __instance._groupChar.CGet<RectTransform>("CombatCharHolder").Find($"{index}/Avatar");
        bool isExist = charId >= 0;
        // var avatar = component.CGet<UICommon.Character.Avatar.Avatar>("Avatar");

        var mouseTipDisplayer = component.gameObject.GetOrAddComponent<MouseTipDisplayer>();
        mouseTipDisplayer.enabled = isExist;
        
        if (isExist)
        {
            Util.EnableMouseTipCharacter(mouseTipDisplayer, charId, 2);
        }
    }
}