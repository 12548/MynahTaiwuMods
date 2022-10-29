using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FrameWork;
using GameData.Domains.Item;
using HarmonyLib;
using TMPro;

namespace MakeItemPreviewFrontend;

[HarmonyPatch]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class MouseTipBasePatch
{
    // public static IEnumerable<MethodBase> TargetMethods()
    // {
    //     // if possible use nameof() or SymbolExtensions.GetMethodInfo() here 
    //     yield return AccessTools.Method(typeof(MouseTipAccessory), "Init");
    //     yield return AccessTools.Method(typeof(MouseTipArmor), "Init");
    //     yield return AccessTools.Method(typeof(MouseTipClothing), "Init");
    //     yield return AccessTools.Method(typeof(MouseTipCarrier), "Init");
    //     yield return AccessTools.Method(typeof(MouseTipFood), "Init");
    //     yield return AccessTools.Method(typeof(MouseTipMedicine), "Init");
    //     yield return AccessTools.Method(typeof(MouseTipWeapon), "Init");
    //     yield return AccessTools.Method(typeof(MouseTipWeapon), "OnInit");
    // }
    //
    // // [HarmonyPostfix, HarmonyPatch(typeof(MouseTipBase), "OnInit")]
    // public static void Postfix(object __instance, ArgumentBox argsBox)
    // {
    //     var mouseTip = __instance as MouseTipBase;
    //     if (mouseTip == null) return;
    //     
    //     var b = argsBox.Get("_mip_MakeResult", out UI_Make.MakeResult makeResult);
    //
    //     if (!b) return;
    //
    //     var s = "";
    //     for (var i = 0; i < makeResult.MakeResultItemArray.Length; i++)
    //     {
    //         var makeResultItem = makeResult.MakeResultItemArray[i];
    //         if (!makeResultItem.IsInit) continue;
    //
    //         s += ((i == 0)
    //             ? "可能的产物："
    //             : $"造诣达到{makeResultItem.LifeSkillRequiredAttainment}时可能的产物：".SetColor("grey"));
    //         
    //         bool buildingLimitEnabled = !string.IsNullOrEmpty(makeResult.UpgradeBuildingName) &&
    //                                     i == makeResult.MakeResultItemArray.Length - 1;
    //         if (buildingLimitEnabled)
    //         {
    //             s += $"(需要建筑：{makeResult.UpgradeBuildingName})".SetColor("lightblue");  
    //         }
    //
    //         s += ItemTemplateHelper.GetName(makeResult.ItemType, makeResultItem.TemplateId)
    //             .SetColor(Colors.Instance.GradeColors[makeResultItem.Grade]) + "\n";
    //     }
    //
    //     if (s.Length > 0)
    //     {
    //         var text = mouseTip.CGet<TextMeshProUGUI>("Desc");
    //         if (!text.text.Contains(s))
    //         {
    //             MouseTip_Util.SetMultiLineAutoHeightText(text, s + "当前为不含加成的预览物品\n" + text.text);
    //         }
    //     }
    // }
}