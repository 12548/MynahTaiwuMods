using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using FrameWork;
using GameData.Domains.Character.Display;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;

// ReSharper disable RedundantAssignment

namespace MynahMoreInfo;

[HarmonyPatch]
public class MouseTipManagerPatch
{

    [HarmonyPrefix, HarmonyPatch(typeof(TooltipManager), (nameof(TooltipManager.ShowTips)))]
    public static bool ShowTipsPrefix(ref TipType type,
        ref ArgumentBox argsBox)
    {
        if (type == TipType.SimpleWide && argsBox.Get("_mmi_charId", out int charid) && charid < 0) return false;

        if (type != TipType.Character && type != TipType.CharacterComplete && type != TipType.CharacterOnMapBlock && type != TipType.LifeCombatSkillValue) return true;

        int charId = 0;
        
        if (argsBox.Get<AvatarRelatedData>("avatar", out _))
        {
            return true;
        }
        if (argsBox.Get<CharacterDisplayDataForTooltip>("Data", out var data))
        {
            // if (ModEntry.MouseTipCharStyle == 1)
            // {
            //     charId = data.Id;
            //     argsBox.Set("CharId", data.Id);
            // }
            // else
            // {
                return true;
            // }
        }
        
        if (argsBox.Get("_mmi_no_replace", out bool _))
        {
            return true;
        }

        if(!argsBox.Get("charId", out charId) || charId < 0)
        {
            if(!argsBox.Get("CharId", out charId) || charId < 0)
            {
                return false;
            }
        }
        // Debug.Log("charId: " +  charId);

        if (ModEntry.MouseTipCharStyle == 1)
        {
            type = TipType.SimpleWide;
            argsBox.Set("_mmi_charId", charId);
            argsBox.Set("_mmi_locationShow", type != TipType.LifeCombatSkillValue); // 石屋不显示位置
        }
        else
        {
            // type = TipType.CharacterComplete;
            argsBox.Set("CharId", charId);
        }
        // if (argsBox.Get("locationShow", out bool showLocation))
        // {
        //     argsBox.Set("_mmi_locationShow", showLocation);
        // }

        return true;
    }

}