using System;
using System.Collections.Generic;
using GameData.Domains.Mod;
using HarmonyLib;

namespace MynahBaseModBackend;

[HarmonyPatch]
public class Patch
{
    private static readonly Dictionary<string, Func<SerializableModData, SerializableModData>> MethodHandlers = new Dictionary<string, Func<SerializableModData, SerializableModData>>();

    public static void handleMethod(string modIdStr, string methodName, Func<SerializableModData, SerializableModData> handler)
    {
        MethodHandlers[$"_{modIdStr}_method_{methodName}_"] = handler;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ModDomain), "SetSerializableModData")]
    private static void SetSerializableModDataPrefix(string modIdStr, string dataName, bool isArchive, ref SerializableModData val)
    {
        // if (modIdStr != StaticModIdStr) return;
        if (isArchive == true) return;

        if (MethodHandlers.TryGetValue(dataName, out var handler))
        {
            val = handler(val);
        }
    }

}