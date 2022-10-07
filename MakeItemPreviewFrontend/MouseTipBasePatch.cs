using System.Collections.Generic;
using System.Reflection;
using FrameWork;
using GameData.Domains.Item;
using GameData.Domains.Mod;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace MakeItemPreviewFrontend;

[HarmonyPatch]
public class MouseTipBasePatch
{
    static IEnumerable<MethodBase> TargetMethods()
    {
        // if possible use nameof() or SymbolExtensions.GetMethodInfo() here
        yield return AccessTools.Method(typeof(MouseTipAccessory), "Init");
        yield return AccessTools.Method(typeof(MouseTipArmor), "Init");
        yield return AccessTools.Method(typeof(MouseTipClothing), "Init");
        yield return AccessTools.Method(typeof(MouseTipCarrier), "Init");
        yield return AccessTools.Method(typeof(MouseTipFood), "Init");
        yield return AccessTools.Method(typeof(MouseTipMedicine), "Init");
        yield return AccessTools.Method(typeof(MouseTipWeapon), "Init");
        yield return AccessTools.Method(typeof(MouseTipWeapon), "OnInit");
    }

    // [HarmonyPostfix, HarmonyPatch(typeof(MouseTipBase), "OnInit")]
    public static void Postfix(MouseTipBase __instance, ArgumentBox argsBox)
    {
        var b = argsBox.Get("MakeResult", out UI_Make.MakeResult makeResult);

        if (!b) return;

        Debug.Log("正在处理MakeResult");

        var s = "";
        for (var i = 0; i < makeResult.MakeResultItemArray.Length; i++)
        {
            var makeResultItem = makeResult.MakeResultItemArray[i];
            if (!makeResultItem.IsInit) continue;

            bool buildingLimitEnabled = !string.IsNullOrEmpty(makeResult.BuildingName) &&
                                        i == makeResult.MakeResultItemArray.Length - 1;
            if (buildingLimitEnabled)
            {
                s += $"(需要建筑：{makeResult.BuildingName})".SetColor("lightblue");
            }

            s += ((i == 0)
                ? "可能的产物："
                : $"造诣达到{makeResultItem.LifeSkillRequiredAttainment}时可能的产物：".SetColor("grey"));

            s += ItemTemplateHelper.GetName(makeResult.ItemType, makeResultItem.TemplateId)
                .SetColor(Colors.Instance.GradeColors[makeResultItem.Grade]) + "\n";
        }

        if (s.Length > 0)
        {
            var text = __instance.CGet<TextMeshProUGUI>("Desc");
            if (!text.text.Contains(s))
            {
                MouseTip_Util.SetMultiLineAutoHeightText(text, s + "当前为不含加成的预览物品\n" + text.text);
            }
        }
    }
}