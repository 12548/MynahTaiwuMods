using System.Collections.Generic;
using System.Linq;
using Config;
using CSharpDiff.Converters;
using CSharpDiff.Diffs;
using FrameWork;
using GameData.Domains.CombatSkill;
using GameData.Serializer;
using GameData.Utilities;
using HarmonyLib;
using MynahMoreInfo.Components;
using TMPro;
using UnityEngine;

namespace MynahMoreInfo;

[HarmonyPatch]
public static class MouseTipCombatSkillPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(MouseTipCombatSkill), "OnGetSkillDisplayData")]
    public static void Postfix(MouseTipCombatSkill __instance, CombatSkillItem ____configData, int offset,
        RawDataPool dataPool)
    {
        if (!ModEntry.ShowCombatSkillSpecialEffect) return;
        if (__instance != null)
        {
            var specialEffectGameObject = __instance.CGet<GameObject>("SpecialEffect");

            var uiCombat = UIElement.Combat.UiBaseAs<UI_Combat>();

            if (uiCombat != null && uiCombat.gameObject.activeInHierarchy)
            {
                return;
            }

            var item = EasyPool.Get<List<CombatSkillDisplayData>>();
            Serializer.Deserialize(dataPool, offset, ref item);
            var combatSkillDisplayData = item[0];
            EasyPool.Free(item);
            var flag = combatSkillDisplayData.EffectType != -1;

            specialEffectGameObject.SetActive(true);
            if (true) // flag
            {
                var flag4 = combatSkillDisplayData.EffectType == 0;
                ShowAllSpecialEffects(specialEffectGameObject, ____configData, flag, flag4);
            }

            ShowAttackPartDistribution(__instance, ____configData);

            if (ModEntry.ShowLearningProgress)
            {
                var s = GetCombatSkillReadingProgressString(combatSkillDisplayData);
                var desc = $"{____configData.Desc}\n{s}";
                MouseTip_Util.SetMultiLineAutoHeightText(__instance.CGet<TextMeshProUGUI>("Desc"), desc);
            }

            var element = __instance.Element;
            element?.ShowAfterRefresh();
        }
    }

    [HarmonyPatch(typeof(MouseTipCombatSkill), "UpdateOnlyTemplateData")]
    [HarmonyPostfix]
    public static void UpdateOnlyTemplateDataPostfix(MouseTipCombatSkill __instance,
        CombatSkillItem ____configData)
    {
        // if (!ShowCombatSkillSpecialEffect) return;
        // __instance.CGet<GameObject>("DirectEffectTitle").SetActive(true);
        // __instance.CGet<GameObject>("DirectDesc").SetActive(true);
        // __instance.CGet<GameObject>("ReverseEffectTitle").SetActive(true);
        // __instance.CGet<GameObject>("ReverseDesc").SetActive(true);
        //
        // __instance.CGet<TextMeshProUGUI>("DirectEffectDesc").text =
        //     ("     " + SpecialEffect.Instance[____configData.DirectEffectID].Desc[0]);
        // __instance.CGet<TextMeshProUGUI>("ReverseEffectDesc").text =
        //     ("     " + SpecialEffect.Instance[____configData.ReverseEffectID].Desc[0]);
        
        var specialEffectGameObject = __instance.CGet<GameObject>("SpecialEffect");
        ShowAllSpecialEffects(specialEffectGameObject, ____configData, false, false, true);
        ShowAttackPartDistribution(__instance, ____configData);
    }

    public static void ShowAllSpecialEffects(GameObject specialEffectObj, CombatSkillItem combatSkillItem,
        bool active, bool activeDirection, bool doubleActive = false)
    {
        specialEffectObj.transform.Find("DirectEffectTitle").gameObject.SetActive(true); // flag4
        var directDesc = specialEffectObj.transform.Find("DirectDesc");
        directDesc.gameObject.SetActive(true);
        specialEffectObj.transform.Find("ReverseEffectTitle").gameObject.SetActive(true); // !flag4
        var reverseDesc = specialEffectObj.transform.Find("ReverseDesc");
        reverseDesc.gameObject.SetActive(true);

        var template1 = active && activeDirection ? "     当前：{0}" : "     如果正练：{0}".SetColor("lightgrey");
        var template2 = active && !activeDirection ? "     当前：{0}" : "     如果逆练：{0}".SetColor("lightgrey");

        if (doubleActive)
        {
            template1 = "{0}";
            template2 = "{0}";
        }
        
        var directText = SpecialEffect
            .Instance[combatSkillItem.DirectEffectID]
            .Desc[0];
        var reverseText = SpecialEffect
            .Instance[combatSkillItem.ReverseEffectID]
            .Desc[0];

        var specialEffectDisplayer = specialEffectObj.GetOrAddComponent<SpecialEffectDisplayer>();

        if (ModEntry.HintEffectDiff > 0)
        {
            var diff = new Diff();

            var d1 = diff
                .diff(directText, reverseText)
                .Where(it => !(it.removed ?? false));
            var d2 = diff
                .diff(reverseText, directText)
                .Where(it => !(it.removed ?? false));

            var dt1 = DiffConvert.ToXml(d2.ToList())
                .Replace("<ins>", "<color=\"red\">")
                .Replace("</ins>", "</color>");

            var rt1 = DiffConvert.ToXml(d1.ToList())
                .Replace("<ins>", "<color=\"red\">")
                .Replace("</ins>", "</color>");

            specialEffectDisplayer.directEffect1 = string.Format(template1, dt1);
            specialEffectDisplayer.reverseEffect1 = string.Format(template2, rt1);
        }

        var directEffectStr = string.Format(template1, directText);
        specialEffectDisplayer.directEffect = directEffectStr;
        
        // UpdateSpecialEffectText(specialEffectObj.transform.Find("DirectDesc/DirectEffectDesc")
        //     .GetComponent<TextMeshProUGUI>(), directEffectStr);

        var reverseEffectStr = string.Format(template2, reverseText);
        specialEffectDisplayer.reverseEffect = reverseEffectStr;
        // UpdateSpecialEffectText(specialEffectObj.transform.Find("ReverseDesc/ReverseEffectDesc")
        //     .GetComponent<TextMeshProUGUI>(), reverseEffectStr);

        specialEffectDisplayer.enabled = true;
        specialEffectDisplayer.UpdateText();
    }


    public static void UpdateSpecialEffectText(TextMeshProUGUI effectText, string effectStr)
    {
        // effectStr = "     " + effectStr;
        var x = effectText.rectTransform.sizeDelta.x;
        var preferredValues = effectText.GetPreferredValues(effectStr, x, float.PositiveInfinity);
        effectText.rectTransform.sizeDelta = preferredValues.SetX(x);
        effectText.text = effectStr;
    }


    public static void ShowAttackPartDistribution(MouseTipCombatSkill __instance,
        CombatSkillItem ____configData)
    {
        if (!ModEntry.ShowAttackDistribution) return;

        Debug.Log($"{____configData.Name} - {____configData.PrepareTotalProgress}");

        {
            var typeTrans = __instance.transform.Find("DescriptionHolder/Type");
            var secondTypeTrans = typeTrans.Find("Type");

            var adtName = "PrepareTotalProgressTips";
            var transform = typeTrans.Find(adtName);
            GameObject adt;
            if (transform == null)
            {
                adt = Object.Instantiate(secondTypeTrans.gameObject, typeTrans, false);
                adt.name = adtName;
            }
            else
            {
                adt = transform.gameObject;
            }

            if (____configData.PrepareTotalProgress > 0)
            {
                // var s = $"基础施展时间: {____configData.PrepareTotalProgress}\n";

                adt.transform.Find("TextHolder/Tips").GetComponent<TextMeshProUGUI>().text = "基础施展时间";
                adt.transform.Find("TextHolder/TypeIcon").gameObject.SetActive(false);
                adt.transform.Find("TextHolder/Type").GetComponent<TextMeshProUGUI>().text =
                    $"{(____configData.PrepareTotalProgress / 7200.0):0.##}秒";
                adt.SetActive(true);
            }
            else
            {
                adt.SetActive(false);
            }
        }

        if (____configData.EquipType == 1)
        {
            var attackEffectObj = __instance.CGet<GameObject>("AttackProperty");
            var adtName = "AttackDisturbTips";
            var transform = attackEffectObj.transform.Find(adtName);
            GameObject adt;
            if (transform == null)
            {
                var t = attackEffectObj.transform.Find("AcupointTips").gameObject;
                adt = Object.Instantiate(t, attackEffectObj.transform, false);
                adt.name = adtName;
            }
            else
            {
                adt = transform.gameObject;
            }

            var s = "打点：";
            var d = ____configData.InjuryPartAtkRateDistribution;

            if (d.Length > 0 && d[0] > 0) s += "胸" + d[0];
            if (d.Length > 1 && d[1] > 0) s += " 腹" + d[1];
            if (d.Length > 2 && d[2] > 0) s += " 头" + d[2];
            if (d.Length > 3 && d[3] > 0) s += " 左手" + d[3];
            if (d.Length > 4 && d[4] > 0) s += " 右手" + d[4];
            if (d.Length > 5 && d[5] > 0) s += " 左腿" + d[5];
            if (d.Length > 6 && d[6] > 0) s += " 右腿" + d[6];

            adt.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = s;
            adt.SetActive(true);
        }
    }


    public static string GetCombatSkillReadingProgressString(CombatSkillDisplayData combatSkillDisplayData)
    {
        var s1 = "承合解异独";
        var s2 = "修思源参藏";
        var s3 = "用奇巧化绝";

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