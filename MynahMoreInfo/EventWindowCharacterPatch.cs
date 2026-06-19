using System.Reflection;
using HarmonyLib;
using UnityEngine;
using EWC = Game.Components.EventWindow.EventWindowCharacter;

namespace MynahMoreInfo;

[HarmonyPatch]
public class EventWindowCharacterPatch
{
    // [HarmonyPatch(typeof(EWC), nameof(EWC.Refresh))]
    // [HarmonyPostfix]
    // public static void Postfix(EWC __instance, MethodBase __originalMethod)
    // {
    //     EWCPostfix(__instance, __originalMethod);
    // }
    // [HarmonyPatch(typeof(EWC), nameof(EWC.RefreshFavorView))]
    // [HarmonyPostfix]
    // public static void Postfix6(EWC __instance, MethodBase __originalMethod)
    // {
    //     EWCPostfix(__instance, __originalMethod);
    // }
    // [HarmonyPatch(typeof(EWC), nameof(EWC.FillFavorProgress))]
    // [HarmonyPostfix]
    // public static void Postfix7(EWC __instance, MethodBase __originalMethod)
    // {
    //     EWCPostfix(__instance, __originalMethod);
    // }
    // [HarmonyPatch(typeof(EWC), nameof(EWC.TryInit))]
    // [HarmonyPostfix]
    // public static void Postfix5(EWC __instance, MethodBase __originalMethod)
    // {
    //     EWCPostfix(__instance, __originalMethod);
    // }
    [HarmonyPatch(typeof(EWC), nameof(EWC.RefreshAsActor))]
    [HarmonyPostfix]
    public static void Postfix1(EWC __instance, MethodBase __originalMethod)
    {
        EWCPostfix(__instance, __originalMethod);
    }
    [HarmonyPatch(typeof(EWC), nameof(EWC.RefreshAsNormalCharacter))]
    [HarmonyPostfix]
    public static void Postfix2(EWC __instance, MethodBase __originalMethod)
    {
        EWCPostfix(__instance, __originalMethod);
    }
    [HarmonyPatch(typeof(EWC), nameof(EWC.RefreshAsTemplateCharacter))]
    [HarmonyPostfix]
    public static void Postfix3(EWC __instance, MethodBase __originalMethod)
    {
        EWCPostfix(__instance, __originalMethod);
    }
    
    [HarmonyPatch(typeof(EWC), nameof(EWC.RefreshAsMerchant))]
    [HarmonyPostfix]
    public static void Postfix4(EWC __instance, MethodBase __originalMethod)
    {
        EWCPostfix(__instance, __originalMethod);
    }
    
    [HarmonyPatch(typeof(EWC), nameof(EWC.RefreshAsXiangShuAvatar))]
    [HarmonyPostfix]
    public static void Postfix41(EWC __instance, MethodBase __originalMethod)
    {
        EWCPostfix(__instance, __originalMethod);
    }
    
    
    public static void EWCPostfix(EWC __instance, MethodBase __originalMethod)
    {
        if (!ModEntry.ShowEventUICharacterMouseTip) return;
        // Debug.Log($"EWCPostfix {__originalMethod.FullDescription()}");
        if (__instance == null) return;
        
        var transform = __instance.transform.Find("CanvasChanger/AvatarArea");
        if (transform == null) return;
        
        var mou = Util.EnsureMouseTipDisplayer(transform.gameObject);
        mou.enabled = __instance.GetHasCharacter();
        
        if (__instance.Data == null) return;
        if (!__instance.GetHasCharacter()) return;
        
        var character = __instance.isLeftCharacter
            ? __instance.Data.MainCharacter
            : __instance.Data.TargetCharacter;
        
        if(character == null) return;
        
        Util.EnableMouseTipCharacter(mou, character.CharacterId, character.AliveState != 0);
    }
}