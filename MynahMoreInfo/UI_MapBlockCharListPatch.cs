using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using FrameWork;
using GameData.Domains.Character.Display;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace MynahMoreInfo;

[HarmonyPatch]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
public class UI_MapBlockCharListPatch
{

    [HarmonyPatch(typeof(MapBlockCharNormal), "Refresh")]
    [HarmonyPostfix]
    static void MapBlockCharNormalRefreshPostfix(
        // bool canInteract,
        // CharacterDisplayData characterDisplayData,
        MapBlockCharNormal __instance)
    {
        if (ModEntry.MouseTipMapBlockCharList == 0) return;

        var charId = __instance.CharId;
        Transform transform = __instance.transform;

        EnableMouseTipChar(charId, transform);
    }
    
    [HarmonyPatch(typeof(MapBlockCharGrave), "Refresh")]
    [HarmonyPostfix]
    static void MapBlockCharGraveRefreshPostfix(
        // bool canInteract,
        // CharacterDisplayData characterDisplayData,
        MapBlockCharGrave __instance)
    {
        if (ModEntry.MouseTipMapBlockCharList == 0) return;

        var charId = __instance._graveDisplayData.Id;
        Transform transform = __instance.transform;

        EnableMouseTipChar(charId, transform);
    }

    private static void EnableMouseTipChar(int charId, Transform transform)
    {
        var trigger = charId > -1;
        var cbutton = transform.Find("Button");
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
            var characterId = charId;

            // Debug.Log($"charId: {characterId}, disp: {(charDisplayData?.FullName ?? ____graveDataDict[charIndex].NameData.FullName).GetName(charDisplayData?.Gender ?? ____graveDataList[charIndex].NameData.Gender, new Dictionary<int, string>())}");

            switch (ModEntry.MouseTipMapBlockCharList)
            {
                case 2:
                    GetAlternateCharTipStr(mouseTipDisplayer, null, characterId);
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