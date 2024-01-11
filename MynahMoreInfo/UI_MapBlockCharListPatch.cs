using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
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
    [HarmonyPatch(typeof(MapBlockCharNormal), "Init")]
    [HarmonyPostfix]
    static void OnRenderCharPostfix(
        bool canInteract,
        CharacterDisplayData characterDisplayData,
        MapBlockCharNormal __instance)
    {
        if (ModEntry.MouseTipMapBlockCharList == 0) return;
        var trigger = canInteract;

        var cbutton = __instance.transform.Find("Button");
        var obj = cbutton.gameObject;
        var mouseTipDisplayer = Util.EnsureMouseTipDisplayer(obj);

        if (!trigger)
        {
            // Debug.Log($"{key} {index} not trigger!");
            mouseTipDisplayer.enabled = false;
            return;
        }

        try
        {
            var characterId = characterDisplayData.CharacterId;

            // Debug.Log($"charId: {characterId}, disp: {(charDisplayData?.FullName ?? ____graveDataDict[charIndex].NameData.FullName).GetName(charDisplayData?.Gender ?? ____graveDataList[charIndex].NameData.Gender, new Dictionary<int, string>())}");

            switch (ModEntry.MouseTipMapBlockCharList)
            {
                case 2:
                    GetAlternateCharTipStr(mouseTipDisplayer, characterDisplayData, characterId);
                    break;
                case 1:
                    Util.EnableMouseTipCharacter(mouseTipDisplayer, characterId);
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
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