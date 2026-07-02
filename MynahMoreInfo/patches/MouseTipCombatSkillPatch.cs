using System.Collections.Generic;
using System.Linq;
using CSharpDiff.Converters;
using CSharpDiff.Diffs;
using Game.Views.Combat;
using Game.Views.MouseTips;
using GameData.Domains.CombatSkill;
using HarmonyLib;
using UnityEngine;

namespace MynahMoreInfo.patches;

[HarmonyPatch]
public static class MouseTipCombatSkillPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(TooltipCombatSkill), nameof(TooltipCombatSkill.RefreshSpecialEffect))]
    public static void Postfix(TooltipCombatSkill __instance)
    {
        // sbyte effectType = __instance._combatSkillDisplayData.EffectType;
        // int? nullable = effectType.HasValue ? new int?((int) effectType.GetValueOrDefault()) : new int();
        // bool flag1 = !(nullable.GetValueOrDefault() == num & nullable.HasValue);
        
        var uiCombat = UIElement.Combat.UiBaseAs<ViewCombat>();
        
        if (uiCombat != null && uiCombat.gameObject.activeInHierarchy)
        {
            return;
        }

        
        string specialEffectDesc1 = __instance.rightDirectEffectDescText.text;
        string specialEffectDesc2 = __instance.rightReverseEffectDescText.text;
        
        if (ModEntry.HintEffectDiff > 0)
        {
            var diff = new Diff();
        
            var d1 = diff
                .diff(specialEffectDesc1, specialEffectDesc2)
                .Where(it => !(it.removed ?? false));
            var d2 = diff
                .diff(specialEffectDesc2, specialEffectDesc1)
                .Where(it => !(it.removed ?? false));
        
            var dt1 = DiffConvert.ToXml(d2.ToList())
                .Replace("<ins>", "<color=\"red\">")
                .Replace("</ins>", "</color>");
        
            var rt1 = DiffConvert.ToXml(d1.ToList())
                .Replace("<ins>", "<color=\"red\">")
                .Replace("</ins>", "</color>");
        
            __instance.rightDirectEffectDescText.text = dt1;
            __instance.rightReverseEffectDescText.text = rt1;
        }
    }
    [HarmonyPostfix]
    [HarmonyPatch(typeof(TooltipCombatSkill), nameof(TooltipCombatSkill.RefreshDirectionEffectConfigOnly))]
    public static void RefreshDirectionEffectConfigOnlyPostfix(TooltipCombatSkill __instance)
    {
        string specialEffectDesc1 = __instance.rightDirectEffectDescText.text;
        string specialEffectDesc2 = __instance.rightReverseEffectDescText.text;
        
        if (ModEntry.HintEffectDiff > 0)
        {
            var diff = new Diff();
        
            var d1 = diff
                .diff(specialEffectDesc1, specialEffectDesc2)
                .Where(it => !(it.removed ?? false));
            var d2 = diff
                .diff(specialEffectDesc2, specialEffectDesc1)
                .Where(it => !(it.removed ?? false));
        
            var dt1 = DiffConvert.ToXml(d2.ToList())
                .Replace("<ins>", "<color=\"red\">")
                .Replace("</ins>", "</color>");
        
            var rt1 = DiffConvert.ToXml(d1.ToList())
                .Replace("<ins>", "<color=\"red\">")
                .Replace("</ins>", "</color>");
        
            __instance.rightDirectEffectDescText.text = dt1;
            __instance.rightReverseEffectDescText.text = rt1;
        }
        // sbyte effectType = __instance._combatSkillDisplayData.EffectType;
        // int? nullable = effectType.HasValue ? new int?((int) effectType.GetValueOrDefault()) : new int();
        // bool flag1 = !(nullable.GetValueOrDefault() == num & nullable.HasValue);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(TooltipCombatSkill), nameof(TooltipCombatSkill.RefreshDataDependentInfo))]
    public static void TooltipCombatSkillPostfix(TooltipCombatSkill __instance)
    {
        if (__instance == null) return;
        // var specialEffectGameObject = __instance.CGet<GameObject>("SpecialEffect");
    
        var uiCombat = UIElement.Combat.UiBaseAs<ViewCombat>();
        
        if (uiCombat != null && uiCombat.gameObject.activeInHierarchy)
        {
            return;
        }
    
        CombatSkillDisplayData combatSkillDisplayData = __instance._combatSkillDisplayData;
        // Serializer.Deserialize(dataPool, offset, ref combatSkillDisplayData);
        var flag = combatSkillDisplayData.EffectType != -1;
    
        // specialEffectGameObject.SetActive(true);
        // if (true) // flag
        // {
        //     var flag4 = combatSkillDisplayData.EffectType == 0;
        //     ShowAllSpecialEffects(specialEffectGameObject, __instance._configData, flag, flag4);
        // }
        //
        // ShowCastTime(__instance, __instance._configData);
    
        if (ModEntry.ShowLearningProgress || ModEntry.ShowCastTime || ModEntry.ShowENAP)
        {
            List<string> strList = new List<string>();
            strList.Add(__instance._configData.Desc);

            if (ModEntry.ShowLearningProgress)
            {
                strList.Add(GetCombatSkillReadingProgressString(combatSkillDisplayData));
            }

            if (ModEntry.ShowCastTime && __instance._configData.PrepareTotalProgress > 0)
            {
                strList.Add($"基本施展时间：{(__instance._configData.PrepareTotalProgress / 7200.0):0.##}秒");
            }

            if (ModEntry.ShowENAP)
            {
                var enap = __instance._configData.ExtraNeiliAllocationProgress;
                if (enap.Any(it => it >= 0))
                {
                    var s = "基本周天真气进度：";
                    for (var i = 0; i < 4; i++)
                    {
                        var min = enap[i];
                        var delta = enap[4];
                        var s1 = (delta > 0) ? $"{min}~{min + delta}" : min.ToString();
                        if (min > 0 || delta > 0)
                        {
                            List<string> titles = ["催破", "轻灵", "护体", "奇窍"];
                            s += $" {titles[i]}{s1}";
                        }
                    }
                    strList.Add(s);
                }
            }
            var desc = strList.Join(delimiter: "\n");
            MouseTip_Util.SetMultiLineAutoHeightText(__instance.descText, desc);
        }
    
        var element = __instance.Element;
        element?.ShowAfterRefresh();
    }
    //
    // [HarmonyPatch(typeof(MouseTipCombatSkill), "UpdateOnlyTemplateData")]
    // [HarmonyPostfix]
    // public static void UpdateOnlyTemplateDataPostfix(MouseTipCombatSkill __instance)
    // {
    //     // if (!ShowCombatSkillSpecialEffect) return;
    //     // __instance.CGet<GameObject>("DirectEffectTitle").SetActive(true);
    //     // __instance.CGet<GameObject>("DirectDesc").SetActive(true);
    //     // __instance.CGet<GameObject>("ReverseEffectTitle").SetActive(true);
    //     // __instance.CGet<GameObject>("ReverseDesc").SetActive(true);
    //     //
    //     // __instance.CGet<TextMeshProUGUI>("DirectEffectDesc").text =
    //     //     ("     " + SpecialEffect.Instance[____configData.DirectEffectID].Desc[0]);
    //     // __instance.CGet<TextMeshProUGUI>("ReverseEffectDesc").text =
    //     //     ("     " + SpecialEffect.Instance[____configData.ReverseEffectID].Desc[0]);
    //     
    //     var specialEffectGameObject = __instance.CGet<GameObject>("SpecialEffect");
    //     ShowAllSpecialEffects(specialEffectGameObject, __instance._configData, false, false, true);
    //     ShowCastTime(__instance, __instance._configData);
    // }
    //
    // public static void ShowAllSpecialEffects(GameObject specialEffectObj, CombatSkillItem combatSkillItem,
    //     bool active, bool activeDirection, bool doubleActive = false)
    // {
    //     specialEffectObj.transform.Find("DirectEffectTitle").gameObject.SetActive(true); // flag4
    //     var directDesc = specialEffectObj.transform.Find("DirectDesc");
    //     directDesc.gameObject.SetActive(true);
    //     specialEffectObj.transform.Find("ReverseEffectTitle").gameObject.SetActive(true); // !flag4
    //     var reverseDesc = specialEffectObj.transform.Find("ReverseDesc");
    //     reverseDesc.gameObject.SetActive(true);
    //
    //     var template1 = active && activeDirection ? "     当前：{0}" : "     如果正练：{0}".SetColor("lightgrey");
    //     var template2 = active && !activeDirection ? "     当前：{0}" : "     如果逆练：{0}".SetColor("lightgrey");
    //
    //     if (doubleActive)
    //     {
    //         template1 = "{0}";
    //         template2 = "{0}";
    //     }
    //
    //     var directText = CommonUtils.GetSpecialEffectDesc(combatSkillItem.TemplateId, true);
    //     var reverseText = CommonUtils.GetSpecialEffectDesc(combatSkillItem.TemplateId, false);
    //     // var directText = SpecialEffect
    //     //     .Instance[combatSkillItem.DirectEffectID]
    //     //     .Desc[0];
    //     // var reverseText = SpecialEffect
    //     //     .Instance[combatSkillItem.ReverseEffectID]
    //     //     .Desc[0];
    //
    //     var specialEffectDisplayer = specialEffectObj.GetOrAddComponent<SpecialEffectDisplayer>();
    //
    //     if (ModEntry.HintEffectDiff > 0)
    //     {
    //         var diff = new Diff();
    //
    //         var d1 = diff
    //             .diff(directText, reverseText)
    //             .Where(it => !(it.removed ?? false));
    //         var d2 = diff
    //             .diff(reverseText, directText)
    //             .Where(it => !(it.removed ?? false));
    //
    //         var dt1 = DiffConvert.ToXml(d2.ToList())
    //             .Replace("<ins>", "<color=\"red\">")
    //             .Replace("</ins>", "</color>");
    //
    //         var rt1 = DiffConvert.ToXml(d1.ToList())
    //             .Replace("<ins>", "<color=\"red\">")
    //             .Replace("</ins>", "</color>");
    //
    //         specialEffectDisplayer.directEffect1 = string.Format(template1, dt1);
    //         specialEffectDisplayer.reverseEffect1 = string.Format(template2, rt1);
    //     }
    //
    //     var directEffectStr = string.Format(template1, directText);
    //     specialEffectDisplayer.directEffect = directEffectStr;
    //     
    //     // UpdateSpecialEffectText(specialEffectObj.transform.Find("DirectDesc/DirectEffectDesc")
    //     //     .GetComponent<TextMeshProUGUI>(), directEffectStr);
    //
    //     var reverseEffectStr = string.Format(template2, reverseText);
    //     specialEffectDisplayer.reverseEffect = reverseEffectStr;
    //     // UpdateSpecialEffectText(specialEffectObj.transform.Find("ReverseDesc/ReverseEffectDesc")
    //     //     .GetComponent<TextMeshProUGUI>(), reverseEffectStr);
    //
    //     specialEffectDisplayer.enabled = true;
    //     specialEffectDisplayer.UpdateText();
    // }
    //
    //
    // public static void UpdateSpecialEffectText(TextMeshProUGUI effectText, string effectStr)
    // {
    //     // effectStr = "     " + effectStr;
    //     var x = effectText.rectTransform.sizeDelta.x;
    //     var preferredValues = effectText.GetPreferredValues(effectStr, x, float.PositiveInfinity);
    //     effectText.rectTransform.sizeDelta = preferredValues.SetX(x);
    //     effectText.text = effectStr;
    // }
    //
    //
    // public static void ShowCastTime(
    //     MouseTipCombatSkill __instance,
    //     CombatSkillItem ____configData
    // ) {
    //     if (!ModEntry.ShowCastTime) return;
    //
    //     Debug.Log($"{____configData.Name} - {____configData.PrepareTotalProgress}");
    //
    //     try {
    //         var typeTrans = __instance.transform.Find("DescriptionHolder/Type");
    //
    //         if (typeTrans == null) return;
    //
    //         var secondTypeTrans = typeTrans.Find("Type");
    //
    //         if (secondTypeTrans == null) return;
    //
    //         var adtName = "PrepareTotalProgressTips";
    //         var transform = typeTrans.Find(adtName);
    //         GameObject adt;
    //         if (transform == null) {
    //             adt = Object.Instantiate(secondTypeTrans.gameObject, typeTrans, false);
    //             adt.name = adtName;
    //         }
    //         else {
    //             adt = transform.gameObject;
    //         }
    //
    //         if (____configData.PrepareTotalProgress > 0) {
    //             // var s = $"基础施展时间: {____configData.PrepareTotalProgress}\n";
    //
    //             adt.transform.Find("TextHolder/Tips").GetComponent<TextMeshProUGUI>().text = "基础施展时间";
    //             adt.transform.Find("TextHolder/TypeIcon").gameObject.SetActive(false);
    //             adt.transform.Find("TextHolder/Type").GetComponent<TextMeshProUGUI>().text =
    //                 $"{(____configData.PrepareTotalProgress / 7200.0):0.##}秒";
    //             adt.SetActive(true);
    //         }
    //         else {
    //             adt.SetActive(false);
    //         }
    //     }
    //     catch (Exception) { }
    // }
    //
    //
    public static string GetCombatSkillReadingProgressString(CombatSkillDisplayData combatSkillDisplayData)
    {
        const string s1 = "承合解异独";
        const string s2 = "修思源参藏";
        const string s3 = "用奇巧化绝";

        var p1 = new List<sbyte>(new sbyte[] { 0, 1, 2, 3, 4 }).Select(page =>
            CombatSkillStateHelper.IsPageRead(combatSkillDisplayData.ReadingState,
                CombatSkillStateHelper.GetOutlinePageInternalIndex(page))).ToArray();
        var p2 = new List<byte>(new byte[] { 1, 2, 3, 4, 5 }).Select(page =>
            CombatSkillStateHelper.IsPageRead(combatSkillDisplayData.ReadingState,
                CombatSkillStateHelper.GetNormalPageInternalIndex(0, page))).ToArray();
        var p3 = new List<byte>(new byte[] { 1, 2, 3, 4, 5 }).Select(page =>
            CombatSkillStateHelper.IsPageRead(combatSkillDisplayData.ReadingState,
                CombatSkillStateHelper.GetNormalPageInternalIndex(1, page))).ToArray();

        string ts1 = "", ts2 = "", ts3 = "";
        for (var i = 0; i < 5; i++)
        {
            ts1 += p1[i] ? $"<color=#ffffffff>{s1[i]}</color>" : $"<color=#474747ff>{s1[i]}</color>";
            ts2 += p2[i] ? $"<color=#00ffffff>{s2[i]}</color>" : $"<color=#004747ff>{s2[i]}</color>";
            ts3 += p3[i] ? $"<color=#ffa500ff>{s3[i]}</color>" : $"<color=#5C3C00ff>{s3[i]}</color>";
        }

        // var s = $"{p1}-<color=#00ffffff>{p2}</color>-<color=orange>{p3}</color>";
        return $"{ts1} {ts2} {ts3}";
    }
}