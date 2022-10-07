using System.Reflection;
using Config;
using FrameWork;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Mod;
using HarmonyLib;
using TaiwuModdingLib.Core.Utils;
using UnityEngine;

namespace MakeItemPreviewFrontend;

[HarmonyPatch]
public class UI_MakePatch
{
    /// <summary>
    /// 制作物品预览
    /// </summary>
    [HarmonyPostfix, HarmonyPatch(typeof(UI_Make), "RefreshMakeTip")]
    public static void RefreshMakeTipPrefix(UI_Make __instance, ItemDisplayData ____currentTarget,
        UI_Make.UIMakeTab ____curTab, short ____makeItemSubTypeId, short ____makeItemTypeId)
    {
        var mouseTipDisplayer = __instance.CGet<MouseTipDisplayer>("PreviewTip");
        if (____currentTarget == null || ____curTab != UI_Make.UIMakeTab.Make)
        {
            mouseTipDisplayer.Type = TipType.MakeItem;
            return;
        }

        // var makeResult = (UI_Make.MakeResult)typeof(UI_Make).GetMethod("GetMakeResult", (BindingFlags)(-1)).Invoke(__instance, new object[]{});
        var makeResult = (UI_Make.MakeResult)__instance.CallMethod("GetMakeResult");
        MakeItemSubTypeItem makeItemSubTypeItem = MakeItemSubType.Instance[____makeItemSubTypeId];
        MakeItemTypeItem makeItemTypeItem = MakeItemType.Instance[____makeItemTypeId];
        
        var args = new SerializableModData();
        
        args.Set("itemType", makeResult.ItemType);
        args.Set("templateId", makeResult.TargetResultItem.TemplateId);
        
        MynahBaseModFrontend.MynahBaseModFrontend.CallMethodCustom(ModEntry.StaticModIdStr, __instance, "GetItemPreview", args,
            result =>
            {
                var succ = result.Get("data", out ItemDisplayData data);

                if (!succ)
                {
                    Debug.Log("failed!");
                    return;
                }

                mouseTipDisplayer.Type = MouseTipManager.ItemTypeToTipType[makeResult.ItemType];

                mouseTipDisplayer.enabled = true;
                mouseTipDisplayer.RuntimeParam = new ArgumentBox()
                    .SetObject("MakeResult", makeResult)
                    .SetObject("ItemData", data);
                // .Set("Title", name)
                // .Set("Desc", makeItemSubTypeItem.Desc);
            });

        return;
    }
}