using Config;
using FrameWork;
using GameData.Domains;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Serializer;
using HarmonyLib;
using TaiwuModdingLib.Core.Utils;
using UnityEngine;

namespace MakeItemPreviewFrontend;

[HarmonyPatch]
public class UI_MakePatch
{
    // /// <summary>
    // /// 制作物品预览 
    // /// </summary>
    // [HarmonyPostfix, HarmonyPatch(typeof(UI_Make), "RefreshMakeTip")]
    // public static void RefreshMakeTipPrefix(UI_Make __instance, ItemDisplayData ____currentTarget,
    //     UI_Make.UIMakeTab ____curTab, short ____makeItemSubTypeId, short ____makeItemTypeId)
    // {
    //     var mouseTipDisplayer = __instance.CGet<MouseTipDisplayer>("PreviewTip");
    //     if (____currentTarget == null || ____curTab != UI_Make.UIMakeTab.Make)
    //     {
    //         Debug.Log($"no preview: {____currentTarget} or {____curTab}!={UI_Make.UIMakeTab.Make}");
    //         mouseTipDisplayer.Type = TipType.MakeItem;
    //         return;
    //     }
    //
    //     // var makeResult = (UI_Make.MakeResult)typeof(UI_Make).GetMethod("GetMakeResult", (BindingFlags)(-1)).Invoke(__instance, new object[]{});
    //     var makeResult = (UI_Make.MakeResult)__instance.CallMethod("GetMakeResult");
    //     MakeItemSubTypeItem makeItemSubTypeItem = MakeItemSubType.Instance[____makeItemSubTypeId];
    //     MakeItemTypeItem makeItemTypeItem = MakeItemType.Instance[____makeItemTypeId];
    //
    //     var ik = new ItemKey
    //     {
    //         Id = -12548,
    //         ItemType = makeResult.ItemType,
    //         TemplateId = makeResult.TargetResultStage.TemplateId
    //     };
    //
    //     Debug.Log("calling!");
    //     __instance.AsyncMethodCall(DomainHelper.DomainIds.Item, ItemDomainHelper.MethodIds.GetItemDisplayData, ik, -12548,
    //         (offset, datapool) =>
    //         {
    //             ItemDisplayData data = null;
    //             Serializer.Deserialize(datapool, offset, ref data);
    //
    //             if (data == null)
    //             {
    //                 Debug.Log("failed!");
    //                 return;
    //             }
    //
    //             mouseTipDisplayer.Type = MouseTipManager.ItemTypeToTipType[makeResult.ItemType];
    //
    //             mouseTipDisplayer.enabled = true;
    //             mouseTipDisplayer.RuntimeParam = new ArgumentBox()
    //                 .SetObject("_mip_MakeResult", makeResult)
    //                 .SetObject("ItemData", data);
    //             // .Set("Title", name)
    //             // .Set("Desc", makeItemSubTypeItem.Desc);
    //         });
    //
    //     return;
    // }
    //
    // [HarmonyPostfix, HarmonyPatch(typeof(UI_Make), "ChangeTab")]
    // public static void ChangeTabPostfix(UI_Make __instance, UI_Make.UIMakeTab newTab)
    // {
    //     var mouseTipDisplayer = __instance.CGet<MouseTipDisplayer>("PreviewTip");
    //     if (newTab != UI_Make.UIMakeTab.Make)
    //     {
    //         mouseTipDisplayer.enabled = false;
    //     }
    // }
}