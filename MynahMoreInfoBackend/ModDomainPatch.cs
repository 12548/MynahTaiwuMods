using System.Collections.Generic;
using GameData.Domains;
using GameData.Domains.Mod;
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
        if (!dataName.StartsWith("GetCharacterData|")) return true;
        var charId = int.Parse(dataName.Split("|")[1]);
        var retValue = new Dictionary<string, object>();

        var charGot = DomainManager.Character.TryGetElement_Objects(charId, out var character);
        if (!charGot) return false;

        retValue["FeatureIds"] = character.GetFeatureIds();
        retValue["LovingItemSubType"] = character.GetLovingItemSubType();
        retValue["HatingItemSubType"] = character.GetHatingItemSubType();
        
        __result = Json.Serialize(retValue);
        
        // AdaptableLog.Info($"Result Set: {__result}");
        return false;
    }

}