using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MynahMoreInfo;

// [HarmonyPatch]
public class ResourcesPatch
{
    // private static readonly Dictionary<string, Sprite> _dict = new();
    // public static IEnumerable<MethodBase> TargetMethods()
    // {
    //     Debug.Log("Finding Target methods");
    //     // if possible use nameof() or SymbolExtensions.GetMethodInfo() here
    //     var methods =  AccessTools.GetDeclaredMethods(typeof(Resources));
    //     foreach (var methodInfo in methods)
    //     {
    //         if (methodInfo.Name.Equals("Load") && methodInfo.GetParameters().Length == 2)
    //         {
    //             Debug.Log("Resources.Load Found:" + methodInfo.FullDescription());
    //             yield return methodInfo;
    //         }
    //     }
    // }
    //
    // [HarmonyPrefix]
    // public static bool Prefix(string path, ref Object __result)
    // {
    //     Debug.Log("Loading:" + path);
    //     if (!path.StartsWith("Sprites/")) return true;
    //     var key = path.Split('/')[1];
    //     if (!_dict.ContainsKey(key)) return true;
    //     __result = _dict[key];
    //     return false;
    // }
    //
    // public static void Init()
    // {
    //     AtlasInfo.Instance.GetSprite("sp_icon_renwutexing_10", sprite => _dict["sp_icon_renwutexing_10"] = sprite);
    //     AtlasInfo.Instance.GetSprite("sp_icon_renwutexing_11", sprite => _dict["sp_icon_renwutexing_11"] = sprite);
    //     AtlasInfo.Instance.GetSprite("sp_icon_renwutexing_4", sprite => _dict["sp_icon_renwutexing_4"] = sprite);
    //     AtlasInfo.Instance.GetSprite("sp_icon_renwutexing_3", sprite => _dict["sp_icon_renwutexing_3"] = sprite);
    //     AtlasInfo.Instance.GetSprite("sp_icon_renwutexing_5", sprite => _dict["sp_icon_renwutexing_5"] = sprite);
    //     AtlasInfo.Instance.GetSprite("sp_icon_renwutexing_6", sprite => _dict["sp_icon_renwutexing_6"] = sprite);
    //     AtlasInfo.Instance.GetSprite("sp_icon_renwutexing_7", sprite => _dict["sp_icon_renwutexing_7"] = sprite);
    //     AtlasInfo.Instance.GetSprite("sp_icon_renwutexing_8", sprite => _dict["sp_icon_renwutexing_8"] = sprite);
    //     AtlasInfo.Instance.GetSprite("sp_icon_renwutexing_9", sprite => _dict["sp_icon_renwutexing_9"] = sprite);
    //  }
    //
 }