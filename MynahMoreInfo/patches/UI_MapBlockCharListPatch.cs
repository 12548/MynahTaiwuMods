using System;
using System.Diagnostics.CodeAnalysis;
using Game.Views.MapBlockCharList;
using GameData.Domains.Character.Display;
using HarmonyLib;
using MynahMoreInfo.utils;
using UnityEngine;

namespace MynahMoreInfo.patches;

[HarmonyPatch]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
public class UI_MapBlockCharListPatch
{
    // [HarmonyPatch(typeof(MapBlockChar), "Refresh")]
    // [HarmonyPostfix]
    // static void MapBlockCharNormalRefreshPostfix(
    //     // bool canInteract,
    //     // CharacterDisplayData characterDisplayData,
    //     MapBlockCharNormal __instance)
    // {
    //     if (ModEntry.MTC_MapBlockCharList == false) return;
    //
    //     var charId = __instance.CharId;
    //     Transform transform = __instance.transform;
    //
    //     EnableMouseTipChar(charId, transform);
    // }

    [HarmonyPatch(typeof(MapBlockChar), nameof(MapBlockChar.Set), typeof(IMapBlockCharHolder), typeof(GraveDisplayData))]
    [HarmonyPostfix]
    static void MapBlockCharGraveRefreshPostfix(
        GraveDisplayData graveDisplayData,
        MapBlockChar __instance)
    {
        if(graveDisplayData == null) return;
        Debug.Log($"SetAsGrave {graveDisplayData.Id}");
        
        if (!ModEntry.MTC_MapBlockCharList) return;
        var charId = graveDisplayData.Id;
        Util.EnableMouseTipCharacter(__instance.displayer, charId, true);
    }
    
    [HarmonyPatch(typeof(MapBlockChar), nameof(MapBlockChar.Set), typeof(IMapBlockCharHolder), typeof(CharacterDisplayData), typeof(bool), typeof(bool))]
    [HarmonyPostfix]
    static void MapBlockCharGravePostfix(
        MapBlockChar __instance)
    {
        // 避免被坟墓覆盖，这里改回来
        __instance.displayer.Type = TipType.CharacterOnMapBlock;
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

            if (ModEntry.MTC_MapBlockCharList)
            {
                Util.EnableMouseTipCharacter(mouseTipDisplayer, characterId);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
}