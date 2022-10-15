using System.Collections.Generic;
using FrameWork;
using GameData.Domains.Character.Display;
using HarmonyLib;

namespace MynahMoreInfo;

[HarmonyPatch]
public class UI_MapBlockCharListPatch
{
    [HarmonyPatch(typeof(UI_MapBlockCharList), "OnRenderChar")]
    [HarmonyPostfix]
    static void OnRenderCharPostfix(int index, Refers charRefers, bool ____canSeeDetail, CToggleGroup ____togGroup,
        List<int> ____infectedList, List<int> ____normalCharList,
        Dictionary<int, CharacterDisplayData> ____charDataDict, UI_MapBlockCharList __instance)
    {
        if (ModEntry.MouseTipMapBlockCharList == 0) return;
        var trigger = ____canSeeDetail;
        var key = ____togGroup.GetActive().Key;
        var showingRandomEnemy = key == 1 && index >= ____infectedList.Count;
        if (showingRandomEnemy) trigger = false;
        if (key > 1) trigger = false;

        var cbutton = charRefers.CGet<CButton>("Button");
        var obj = cbutton.gameObject;
        var mouseTipDisplayer = Util.EnsureMouseTipDisplayer(obj);

        var charList = ((key == 0) ? ____normalCharList : ____infectedList);

        if (trigger && charList.CheckIndex(index) && ____charDataDict.ContainsKey(charList[index]))
        {
            var charIndex = charList[index];
            var charDisplayData = ____charDataDict[charIndex];
            var characterId = charDisplayData.CharacterId;

            switch (ModEntry.MouseTipMapBlockCharList)
            {
                case 2:
                    GetAlternateCharTipStr(mouseTipDisplayer, charDisplayData);
                    break;
                case 1:
                    Util.EnableMouseTipCharacter(mouseTipDisplayer, characterId);
                    break;
            }
        }
        else
        {
            mouseTipDisplayer.enabled = false;
        }
    }

    static void GetAlternateCharTipStr(MouseTipDisplayer displayer, CharacterDisplayData displayData)
    {
        displayer.Type = TipType.SimpleWide;
        displayer.RuntimeParam = new ArgumentBox();
        displayer.RuntimeParam.Set("arg0", "人物浮窗加载中");
        displayer.RuntimeParam.Set("arg1", "人物浮窗加载中");
        displayer.RuntimeParam.SetObject("_mmi_mtc_charDisplayData", displayData);
        displayer.enabled = true;
    }

}