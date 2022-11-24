using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FrameWork;
using GameData.Domains.Character.Display;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace MynahMoreInfo;

[HarmonyPatch]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class UI_MapBlockCharListPatch
{
    [HarmonyPatch(typeof(UI_MapBlockCharList), "OnRenderChar")]
    [HarmonyPostfix]
    static void OnRenderCharPostfix(
        int index,
        Refers charRefers,
        bool ____canSeeDetail,
        CToggleGroup ____togGroup,
        List<int> ____infectedList,
        List<int> ____normalCharList,
        List<int> ____graveList,
        Dictionary<int, GraveDisplayData> ____graveDataDict,
        Dictionary<int, CharacterDisplayData> ____charDataDict,
        UI_MapBlockCharList __instance)
    {
        if (ModEntry.MouseTipMapBlockCharList == 0) return;
        var trigger = ____canSeeDetail;
        var key = ____togGroup.GetActive().Key;
        var showingRandomEnemy = key == 1 && index >= ____infectedList.Count;
        if (showingRandomEnemy) trigger = false;
        if (key > 1 && key != 3) trigger = false;

        var cbutton = charRefers.CGet<CButton>("Button");
        var obj = cbutton.gameObject;
        var mouseTipDisplayer = Util.EnsureMouseTipDisplayer(obj);

        var charList = (key switch
        {
            0 => ____normalCharList,
            1 => ____infectedList,
            3 => ____graveList,
            _ => new List<int>()
        });

        if (!trigger)
        {
            // Debug.Log($"{key} {index} not trigger!");
            mouseTipDisplayer.enabled = false;
            return;
        }

        if (!charList.CheckIndex(index))
        {
            Debug.Log($"{key} {index} CheckIndex fail!");
            return;
        }

        if (key != 3 && !____charDataDict.ContainsKey(charList[index]))
        {
            Debug.Log($"{key} {index}ContainsKey {charList[index]} fail!");
            return;
        }

        var charIndex = charList[index];
        var charDisplayData = key == 3 ? null : ____charDataDict[charIndex];
        var characterId = charDisplayData?.CharacterId ?? ____graveDataDict[charIndex].Id;

        // Debug.Log($"charId: {characterId}, disp: {(charDisplayData?.FullName ?? ____graveDataDict[charIndex].NameData.FullName).GetName(charDisplayData?.Gender ?? ____graveDataList[charIndex].NameData.Gender, new Dictionary<int, string>())}");

        switch (ModEntry.MouseTipMapBlockCharList)
        {
            case 2:
                GetAlternateCharTipStr(mouseTipDisplayer, charDisplayData, characterId);
                break;
            case 1:
                Util.EnableMouseTipCharacter(mouseTipDisplayer, characterId);
                break;
        }
    }

    static void GetAlternateCharTipStr(MouseTipDisplayer displayer, [CanBeNull] CharacterDisplayData displayData, int charId)
    {
        displayer.Type = TipType.SimpleWide;
        displayer.RuntimeParam = new ArgumentBox();
        displayer.RuntimeParam.Set("arg0", "人物浮窗加载中");
        displayer.RuntimeParam.Set("arg1", "人物浮窗加载中");
        displayer.RuntimeParam.Set("_mmi_charId", charId);
        if (displayData != null)
        {
            displayer.RuntimeParam.SetObject("_mmi_mtc_charDisplayData", displayData);
        }

        displayer.enabled = true;
    }
}