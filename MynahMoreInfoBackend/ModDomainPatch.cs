using System;
using System.Collections.Generic;
using System.Linq;
using ConchShip.EventConfig.Taiwu;
using Config;
using Config.EventConfig;
using GameData.Domains;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.Mod;
using GameData.Domains.TaiwuEvent;
using GameData.Utilities;
using HarmonyLib;
using MiniJSON;

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
        if (dataName == null || !dataName.StartsWith("GetCharacterData|")) return true;
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
                retValue["Favorability"] = DomainManager.Character.GetFavorability(charId, DomainManager.Taiwu.GetTaiwuCharId());

                var loc = DomainManager.Character.GetCharacterDisplayData(charId).Location;
                if (loc.IsValid())
                {
                    var pos = DomainManager.Map.GetBlock(loc).GetBlockPos();
                    retValue["BlockFullName"] =
                        MapDomainUtils.GetBlockFullName(loc, "", "", false)
                        + $"({pos.X}, {pos.Y})";
                }

                if (DomainManager.Character.TryGetElement_Graves(charId, out var grave))
                {
                    retValue["GraveLevel"] = grave.GetLevel();
                    retValue["GraveDurability"] = grave.GetDurability();
                    
                    if (ModEntry.ShowNpcGoodItemsCount > 0)
                    {
                        var inventory = grave.GetInventory();
                        var items = inventory.Items.Keys;
                        
                        var displayItems = items
                            .Where(it => it.ItemType is > ItemType.Invalid and < ItemType.Count)
                            .OrderByDescending(it => ItemTemplateHelper.GetGrade(it.ItemType, it.TemplateId))
                            .Take(ModEntry.ShowNpcGoodItemsCount)
                            .ToList();

                        retValue["AvailableItems"] = displayItems
                            .Select(it => it.TemplateId)
                            .ToList();
                        retValue["AvailableItemTypes"] = displayItems
                            .Select(it => it.ItemType)
                            .ToList();
                    }

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

        __result = Json.Serialize(retValue);

        // AdaptableLog.Info($"Result Set: {__result}");
        return false;
    }
}