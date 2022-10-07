using System.Collections.Generic;
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
            Dictionary<int, CharacterDisplayData> ____charDataDict)
        {
            if(!ModEntry.ShowMouseTipMapBlockCharList) return;
            var trigger = ____canSeeDetail && ModEntry.ShowMouseTipMapBlockCharList;
            var key = ____togGroup.GetActive().Key;
            var showingRandomEnemy = key == 1 && index >= ____infectedList.Count;
            if (showingRandomEnemy) trigger = false;
            if (key > 1) trigger = false;

            var cbutton = charRefers.CGet<CButton>("Button");
            var obj = cbutton.gameObject;

            var mouseTipDisplayer = Util.EnsureMouseTipDisplayer(obj);

            if (trigger)
            {
                var charDisplayData = ____charDataDict[
                    ((key == 0) ? ____normalCharList : ____infectedList)[
                        index]];
                var characterId = charDisplayData.CharacterId;

                Util.EnableMouseTipCharacter(mouseTipDisplayer, characterId);
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

}