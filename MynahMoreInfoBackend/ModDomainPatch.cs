using System;
using System.Collections.Generic;
using System.Linq;
using ConchShip.EventConfig.Taiwu;
using Config;
using Config.EventConfig;
using GameData.Domains;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Mod;
using GameData.Domains.TaiwuEvent;
using GameData.Utilities;
using HarmonyLib;
using MiniJSON;
using SkillBook = GameData.Domains.Item.SkillBook;

namespace MynahMoreInfoBackend;

[HarmonyPatch]
public class ModDomainPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ModDomain), "GetString")]
    private static bool GetStringPrefix(string modIdStr, string dataName, bool isArchive, ref string __result)
    {
        // AdaptableLog.Info($"GetString: {modIdStr} {dataName} {isArchive}");
        // if (modIdStr != ModEntry.StaticModIdStr) return true;
        // if (isArchive) return true;
        if (!dataName.StartsWith("GetCharacterData|")) return true;
        var charId = int.Parse(dataName.Split("|")[1]);
        var retValue = new Dictionary<string, object>();

        var list = new List<short>();

        var charGot = DomainManager.Character.TryGetElement_Objects(charId, out var character);
        // DomainManager.Character.GetDeadCharacter(charId);
        if (!charGot)
        {
            var deadGuy = DomainManager.Character.TryGetDeadCharacter(charId);
            if (deadGuy != null)
            {
                retValue["IsDead"] = true;

                // retValue["FullName"] = deadGuy.FullName;
                retValue["Gender"] = deadGuy.Gender;
                retValue["Happiness"] = deadGuy.Happiness;
                retValue["Morality"] = deadGuy.Morality;

                retValue["FameType"] = deadGuy.FameType;
                retValue["Attraction"] = deadGuy.Attraction;

                retValue["LiveAge"] = deadGuy.GetActualAge();
                retValue["DeathDate"] = deadGuy.DeathDate;
                retValue["BirthDate"] = deadGuy.BirthDate;

                retValue["PreexistenceCharCount"] = deadGuy.PreexistenceCharIds.Count;

                retValue["FeatureIds"] = deadGuy.FeatureIds;

                var loc = DomainManager.Character.GetCharacterDisplayData(charId).Location;
                if (loc.IsValid())
                {
                    var pos = DomainManager.Map.GetBlock(loc).GetBlockPos();
                    retValue["BlockFullName"] =
                        MapDomainUtils.GetBlockFullName(loc, "", "", false)
                        + $"({pos.X}, {pos.Y})";
                }
            }

            __result = Json.Serialize(retValue);
            return false;
        }

        retValue["FeatureIds"] = character.GetFeatureIds();
        retValue["LovingItemSubType"] = character.GetLovingItemSubType();
        retValue["HatingItemSubType"] = character.GetHatingItemSubType();
        retValue["IsBisexual"] = character.GetBisexual();
        retValue["XiangshuInfection"] = character.GetXiangshuInfection(); // 入魔值

        var location = character.GetLocation();
        if (location.IsValid())
        {
            var pos = DomainManager.Map.GetBlock(location).GetBlockPos();
            retValue["BlockFullName"] =
                MapDomainUtils.GetBlockFullName(location, "", "", false)
                + $"({pos.X}, {pos.Y})";
        }

        for (sbyte i = 0; i < LifeSkillType.Instance.Count; i++)
        {
            list.Add(character.GetLifeSkillAttainment(i));
        }

        retValue["LifeSkillAttainments"] = list.ToArray();

        list = new List<short>();

        for (sbyte i = 0; i < CombatSkillType.Instance.Count; i++)
        {
            list.Add(character.GetCombatSkillAttainment(i));
        }

        retValue["CombatSkillAttainments"] = list.ToArray();

        var availableBattleSkillList = new List<int>();
        retValue["AvailableBattleSkills"] = availableBattleSkillList;

        if (ModEntry.ShowNpcGoodBattleSkillsCount > 0)
        {
            var skills = DomainManager.CombatSkill
                .GetCharCombatSkills(charId)
                .OrderByDescending(it => CombatSkill.Instance[it.Value.GetId().SkillTemplateId].Grade)
                .Select(it => it.Value.GetId().SkillTemplateId);

            retValue["AvailableBattleSkills"] = skills.Take(ModEntry.ShowNpcGoodBattleSkillsCount).ToList();

            // var items = DomainManager.Character.GetAllInventoryItems(charId);
            // items.Sort(((a, b) =>
            // {
            //     
            // }));
        }


        var availableItemsList = new List<int>();
        retValue["AvailableItems"] = availableItemsList;
        retValue["AvailableItemTypes"] = availableItemsList;

        if (ModEntry.ShowNpcGoodItemsCount > 0)
        {
            var items = DomainManager.Character.GetAllInventoryItems(charId);

            items.AddRange(DomainManager.Character
                .GetAllEquipmentItems(charId));

            var displayItems = items
                .Where(it => it.Key.ItemType is > ItemType.Invalid and < ItemType.Count)
                .OrderByDescending(it => ItemTemplateHelper.GetGrade(it.Key.ItemType, it.Key.TemplateId))
                .ThenByDescending(it => it.Price)
                .Take(ModEntry.ShowNpcGoodItemsCount)
                .ToList();

            retValue["AvailableItems"] = displayItems
                .Select(it => it.Key.TemplateId)
                .ToList();
            retValue["AvailableItemTypes"] = displayItems
                .Select(it => it.Key.ItemType)
                .ToList();
        }

        var availableLifeSkillList = new List<int>();
        retValue["AvailableLifeSkills"] = availableLifeSkillList;

        if (ModEntry.ShowLearnableSkill)
        {
            void NewFunction(TaiwuEventOption taiwuEventOption, TaiwuEventItem taiwuEventItem)
            {
                if (!taiwuEventOption.OnOptionVisibleCheck() || !taiwuEventOption.OnOptionAvailableCheck()) return;

                var selectResult = taiwuEventOption.OnOptionSelect();
                var skillId = -1;
                if (selectResult.StartsWith("2636780e") && taiwuEventItem.ArgBox.Get("SkillCanLearn", ref skillId) &&
                    skillId > -1)
                {
                    if (!availableLifeSkillList.Contains(skillId))
                    {
                        availableLifeSkillList.Add(skillId);
                    }

                    taiwuEventItem.ArgBox.Remove<int>("SkillCanLearn");
                }
                else
                {
                    AdaptableLog.Info($"不可请教：{selectResult}");
                }
            }

            try
            {
                var eventArgBox = new EventArgBox();
                eventArgBox.Set("CharacterId", charId);
                var interactMainEvent = new TaiwuEvent_a9d0bcd8e3784ee996a61e5b9db17371
                {
                    ArgBox = eventArgBox
                };

                // 向门派成员请教技艺
                var opt1 = interactMainEvent.EventOptions[0];
                if (opt1.OnOptionVisibleCheck() && opt1.OnOptionAvailableCheck() &&
                    opt1.OnOptionSelect() == "49ae4b01-b8d7-47eb-b37b-cf5e4381a624")
                {
                    // 可以请教
                    var learnWayEvent = new TaiwuEvent_49ae4b01b8d747ebb37bcf5e4381a624
                    {
                        ArgBox = new EventArgBox(eventArgBox)
                    };

                    // 选项：0门派请教 1私下请教

                    var sectLearnOption = learnWayEvent.EventOptions[0];
                    if (sectLearnOption.OnOptionVisibleCheck() && sectLearnOption.OnOptionAvailableCheck())
                    {
                        var sectLearnResult = sectLearnOption.OnOptionSelect();

                        AdaptableLog.Info($"sectLearnResult: {sectLearnResult}");

                        if (sectLearnResult.StartsWith("e9f38297")
                            || sectLearnResult.StartsWith("482e21d1")
                            || sectLearnResult.StartsWith("962785de")
                            || sectLearnResult.StartsWith("f97119a5")
                            || sectLearnResult.StartsWith("3cc18ea2")
                            || sectLearnResult.StartsWith("6562eab6")
                            || sectLearnResult.StartsWith("a641b4bb")
                            || sectLearnResult.StartsWith("3f493d7e")
                           )
                        {
                            var chooseTypeEvent = DomainManager.TaiwuEvent.GetEvent(sectLearnResult).EventConfig;
                            chooseTypeEvent.ArgBox = new EventArgBox(eventArgBox);

                            // 选择请教哪一项技艺，最后一项是返回
                            for (var i = 0; i < chooseTypeEvent.EventOptions.Length - 1; i++)
                            {
                                var op = chooseTypeEvent.EventOptions[i];
                                NewFunction(op, chooseTypeEvent);

                                chooseTypeEvent.ArgBox = new EventArgBox(eventArgBox);
                            }

                            // TaiwuEvent_e9f382970aa24566b27f6eb8b3123807
                        }
                    }

                    NewFunction(learnWayEvent.EventOptions[1], learnWayEvent);

                    // var learnWayEvent = DomainManager.TaiwuEvent.GetEvent("49ae4b01-b8d7-47eb-b37b-cf5e4381a624");
                }

                // 向城镇人请教技艺
                var opt2 = interactMainEvent.EventOptions[1];
                AdaptableLog.Info($"1");
                NewFunction(opt2, interactMainEvent);

                // 向同道请教功法
                var opt3 = interactMainEvent.EventOptions[2];
                if (opt3.OnOptionVisibleCheck() && opt3.OnOptionAvailableCheck())
                {
                    // 可以请教
                }

                // 向门派成员请教功法
                var opt4 = interactMainEvent.EventOptions[3];
                if (opt4.OnOptionVisibleCheck() && opt4.OnOptionAvailableCheck())
                {
                    // 可以请教
                }
            }
            catch (Exception e)
            {
                AdaptableLog.Error(e.Message);
                AdaptableLog.Error(e.StackTrace);
                throw;
            }
        }


        __result = Json.Serialize(retValue);

        // AdaptableLog.Info($"Result Set: {__result}");
        return false;
    }
}