using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Config;
using Config.ConfigCells.Character;
using FrameWork;
using GameData.Domains;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Mod;
using GameData.Serializer;
using HarmonyLib;
using MiniJSON;
using UICommon.Character.Elements;
using UnityEngine;
using LifeSkillType = Config.LifeSkillType;

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
                    GetAlternateCharTipStr(__instance, mouseTipDisplayer, charDisplayData);
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
        // mouseTipDisplayer.Type = TipType.SingleDesc;
        //

        //
        // CharacterItem item = Character.Instance.GetItem(Character.Instance[charDisplayData.TemplateId].TemplateId);
        // var s = "";
        // s += CommonUtils.GetGenderString(charDisplayData.Gender) + "·";
        // s += CommonUtils.GetCharmLevelText(
        //     item.BaseAttraction,
        //     item.Gender,
        //     20, 1) + "(基础值)·";
        // s += CommonUtils.GetBehaviorString(charDisplayData.BehaviorType) + "·";
        // s += SingletonObject.getInstance<WorldMapModel>().GetSettlementName(charDisplayData.OrgInfo);
        // s += GetIdentityText(item, charDisplayData.OrgInfo);
        //
        // mouseTipDisplayer.PresetParam = new[] { s };
    }

    static void GetAlternateCharTipStr(UIBase ui, MouseTipDisplayer displayer, CharacterDisplayData displayData)
    {
        displayer.Type = TipType.SimpleWide;
        displayer.RuntimeParam = new ArgumentBox();
        displayer.RuntimeParam.Set("arg0", "人物浮窗加载中");
        displayer.RuntimeParam.Set("arg1", "人物浮窗加载中");
        displayer.RuntimeParam.SetObject("_mmi_mtc_charDisplayData", displayData);
        displayer.enabled = true;
        
        // MynahBaseModFrontend.MynahBaseModFrontend.ModGetString(
        //     ModEntry.StaticModIdStr,
        //     ui,
        //     $"GetCharacterData|{displayData.CharacterId}",
        //     false,
        //     data =>
        //     {
        //         var characterData = (Dictionary<string, object>)Json.Deserialize(data);
        // List<int> featureIds = new();
        // data.Get("featureIds", out string featureIdStr);
        // if (featureIdStr != null)
        // {
        //     featureIds = featureIdStr.Split(',').ToList().Select(int.Parse).ToList();
        // }
        // data.Get("groupCharDisplayData", out GroupCharDisplayData groupCharDisplayData);
        //
        //     Title(displayData, characterData.GroupCharDisplayData, characterData.FeatureIds, displayer);
        // });

        // ui.AsynchMethodCall(DomainHelper.DomainIds.Character,
        //     CharacterDomainHelper.MethodIds.GetGroupCharDisplayDataList, new List<int> { displayData.CharacterId },
        //     (offset, dataPool) =>
        //     {
        //         var item = EasyPool.Get<List<GroupCharDisplayData>>();
        //         Serializer.Deserialize(dataPool, offset, ref item);
        //         if (item.Count < 1) return;
        //         var groupCharDisplayData = item[0];
        //         EasyPool.Free(item);
        //
        //         if (groupCharDisplayData == null) return;
        //
        //         var title = Title(displayData, groupCharDisplayData, out var sb);
        //

        //     });
    }

}