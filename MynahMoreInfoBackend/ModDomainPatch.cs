﻿using System.Collections.Generic;
using GameData.Domains;
using GameData.Domains.Mod;
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
        if (!dataName.StartsWith("GetCharacterData|")) return true;
        var charId = int.Parse(dataName.Split("|")[1]);
        var retValue = new Dictionary<string, object>();

        var charGot = DomainManager.Character.TryGetElement_Objects(charId, out var character);
        // DomainManager.Character.GetDeadCharacter(charId);
        if (!charGot) return false;

        retValue["FeatureIds"] = character.GetFeatureIds();
        retValue["LovingItemSubType"] = character.GetLovingItemSubType();
        retValue["HatingItemSubType"] = character.GetHatingItemSubType();
        retValue["IsBisexual"] = character.GetBisexual();
        retValue["XiangshuInfection"] = character.GetXiangshuInfection(); // 入魔值
        
        // 等他更新到正式版
        // var location = character.GetLocation();
        // if (location.IsValid())
        // {
        //     var pos = DomainManager.Map.GetBlock(location).GetBlockPos();
        //     retValue["BlockFullName"] =
        //         DomainManager.Map.GetBlockFullName(location)
        //         + $"({pos.X}, {pos.Y})";
        // }

        __result = Json.Serialize(retValue);

        // AdaptableLog.Info($"Result Set: {__result}");
        return false;
    }
}