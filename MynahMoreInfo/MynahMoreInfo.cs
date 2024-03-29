﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Config;
using FrameWork;
using GameData.Domains;
using GameData.Domains.CombatSkill;
using GameData.Domains.Item.Display;
using GameData.Serializer;
using GameData.Utilities;
using HarmonyLib;
using MynahBaseModBase;
using MynahMoreInfo.SpriteSheet;
using TaiwuModdingLib.Core.Plugin;
using TMPro;
using UnityEngine;
using CombatSkillType = Config.CombatSkillType;

namespace MynahMoreInfo;

[PluginConfig("MynahMoreInfo", "myna12548", "1")]
[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public partial class ModEntry : TaiwuRemakeHarmonyPlugin
{
    [ModSetting("显示不传之秘", description: "显示门派武学列表中的不传之秘")]
    public static bool ShowNonPublicSkill = true;

    // [ModSetting("左侧人物浮窗", description: "为地图左侧人物列表增加鼠标浮窗")]
    // public static readonly bool ShowMouseTipMapBlockCharList = true;

    [DropDownModSetting("左侧人物浮窗", new[] { "关闭", "原版", "详细文字" }, defaultValue: 2)]
    public static int MouseTipMapBlockCharList = 2;

    [DropDownModSetting("居民人物浮窗",
        new[] { "关闭", "原版", "详细文字" },
        description: "居住、扩建等界面已安排的人物上",
        defaultValue: 2
    )]
    public static int MouseTipResidentView = 2;

    [DropDownModSetting(
        "地块浮窗",
        new[] { "不显示", "按住alt显示", "始终显示（原版）" },
        description: "注意：如果选择按住alt显示，那么需要按住alt再移动鼠标到新的地块来显示浮窗",
        defaultValue: 2
    )]
    public static int MapBlockMouseTipStat = 2;

    [DropDownModSetting(
        "显示人物特性",
        description: "在详细文字式左侧人物浮窗中显示人物特性",
        options: new[] { "关闭", "显示可见特性", "显示全部特性" },
        defaultValue: 2
    )]
    public static int ShowCharFeatures = 2;

    [ModSetting("功法正逆练", description: "显示功法的正逆练效果")]
    public static bool ShowCombatSkillSpecialEffect = true;

    [ModSetting("书籍正逆练", description: "显示书籍所载功法的正逆练效果")]
    public static bool ShowBookSpecialEffect = true;

    [ModSetting("显示打点和施展时间", description: "显示催破功法的打点分布和可施展功法的基本施展时间（施展速度为100%时的施展时间）")]
    public static bool ShowAttackDistribution = true;

    [ModSetting("显示书籍五行属性", description: "显示功法书对应功法的五行属性")]
    public static bool ShowBookFiveElements = true;

    [ModSetting("学习进度", description: "开启正逆练显示时生效，显示功法、功法书籍的读书、修炼进度，目前显示位置不太好，不喜欢可以关闭")]
    public static bool ShowLearningProgress = true;

    [ModSetting("对话人物浮窗", description: "为事件界面（人物对话互动等）的左右两个人物增加鼠标浮窗")]
    public static bool ShowEventUICharacterMouseTip = true;

    [ModSetting("人物浮窗显示真名", description: "从官方mod拿来的功能，在人物浮窗中显示法号对应的真实姓名")]
    public static bool CharacterMouseTipShowRealName = true;
    
    [ModSetting("人物浮窗显示可请教技能", description: "在详细人物浮窗中显示可请教的技艺")]
    public static bool ShowLearnableSkill = true;

    [ModSetting("地块浮窗显示人物列表", description: "")]
    public static bool MapBlockMouseTipCharList = true;

    [ModSetting("地块浮窗高亮较多资源", description: "在地块浮窗中将100以上的资源以不同颜色显示")]
    public static bool MapBlockMouseTipHighlightResource = true;

    [ModSetting("浮窗显示坐标和人物ID", description: "")]
    public static bool ShowPosAndId = true;

    // [ModSetting("延迟去除", description: "将官方的tips机制重写为无延迟的旧版本，请务必谨慎使用")]
    // public static bool DelayFix = false;

    [ModSetting("全部详细人物浮窗", description: "将全部的原版人物浮窗替换为详细文字形式")]
    public static bool ReplaceAllCharacterTipToDetail = true;

    public static string StaticModIdStr;

    // private static bool _init;
    // private static IEnumerator _currDelayFix;
    // private static IEnumerator _origDelay;

    public override void OnModSettingUpdate()
    {
        Debug.Log("MynahMoreInfo OnModSettingUpdate");
        base.OnModSettingUpdate();
        MynahBaseModFrontend.MynahBaseModFrontend.OnModSettingUpdate(this);
        InitDelayFix();
    }

    public override void Initialize()
    {
        base.Initialize();
        StaticModIdStr = ModIdStr;
        SpriteAssetManager.Init();
        // _init = true; 
        InitDelayFix();

        var original = AccessTools.FirstMethod(typeof(MouseTipCombatSkill),
            it => it.Name.Contains("OnGetSkillDisplayData"));
        var postfix = typeof(Patch).GetMethod("Postfix");
        HarmonyInstance.Patch(original, postfix: new HarmonyMethod(postfix));

        var original2 = AccessTools.FirstMethod(typeof(MouseTipCombatSkill),
            it => it.Name.Contains("UpdateOnlyTemplateData"));
        var postfix2 = typeof(Patch).GetMethod("PostFixUpdateOnlyTemplateData");
        HarmonyInstance.Patch(original2, postfix: new HarmonyMethod(postfix2));

        var o3 = AccessTools.FirstMethod(typeof(UI_CombatSkillTree), it => it.Name.Contains("RefreshSkillItem"));
        var prefix3 = typeof(Patch).GetMethod("PreFixRefreshSkillItem");
        HarmonyInstance.Patch(o3, prefix: new HarmonyMethod(prefix3));

        var o4 = AccessTools.FirstMethod(typeof(MouseTipBook), it => it.Name.Contains("Init"));
        var postfix4 = typeof(Patch).GetMethod("PostFixMouseTipBookInit");
        HarmonyInstance.Patch(o4, postfix: new HarmonyMethod(postfix4));

        Debug.Log("ShowSpecialEffect all patched");
    }

    private static void InitDelayFix()
    {
        // Debug.Log($"init delay fix: {_init} {DelayFix}");
        // if (_init)
        // {
        //     if (DelayFix && _currDelayFix == null)
        //     {
        //         var yieldHelper = SingletonObject.getInstance<YieldHelper>();
        //         var mouseTipManager = SingletonObject.getInstance<MouseTipManager>();
        //         var fieldInfo = typeof(MouseTipManager).GetField("_updateMouseOverObjCoroutine", (BindingFlags)(-1));
        //         if (fieldInfo == null) return;
        //         _origDelay = (IEnumerator)fieldInfo.GetValue(mouseTipManager);
        //         yieldHelper.StopCoroutine(_origDelay);
        //         _currDelayFix = MouseTipManagerPatch.UpdateMouseOverObj();
        //         fieldInfo.SetValue(mouseTipManager, _currDelayFix);
        //         yieldHelper.StartYield(_currDelayFix);
        //         Debug.Log("TipsCo Replaced!");
        //     }
        //     else if (!DelayFix && _origDelay != null && _currDelayFix != null)
        //     {
        //         var yieldHelper = SingletonObject.getInstance<YieldHelper>();
        //         var mouseTipManager = SingletonObject.getInstance<MouseTipManager>();
        //         var fieldInfo = typeof(MouseTipManager).GetField("_updateMouseOverObjCoroutine", (BindingFlags)(-1));
        //         if (fieldInfo == null) return;
        //         yieldHelper.StopCoroutine(_currDelayFix);
        //         _currDelayFix = null;
        //         fieldInfo.SetValue(mouseTipManager, _origDelay);
        //         yieldHelper.StartYield(_origDelay);
        //         Debug.Log("TipsCo Replaced!");
        //     }
        // }
    }


    public static class Patch
    {
        public static void PostFixMouseTipBookInit(MouseTipBook __instance, bool ____isCombatSkill,
            ArgumentBox argsBox)
        {
            if (!ShowBookSpecialEffect) return;
            if (__instance == null) return;
            if (argsBox == null) return;

            var dh = __instance.transform.Find("DescriptionHolder");
            if (dh == null)
            {
                Debug.Log("cant find DescriptionHolder");
                return;
            }

            var combatSkillTip = UIElement.MouseTipCombatSkill.UiBaseAs<MouseTipCombatSkill>();
            if (combatSkillTip == null || combatSkillTip.transform == null)
            {
                Debug.Log("cant find combatSkillTip transform");
                return;
            }

            var specialHolder = combatSkillTip.transform.Find("DescriptionHolder/SpecialEffect");
            if (specialHolder == null)
            {
                Debug.Log("cant find specialHolder");
                return;
            }

            if (specialHolder.gameObject == null)
            {
                Debug.Log("cant find specialHolderGO");
                return;
            }

            var specialEffectTrans = dh.Find("SpecialEffect");
            if (specialEffectTrans == null)
            {
                specialEffectTrans =
                    Object.Instantiate(specialHolder.gameObject, dh, false).transform;
                specialEffectTrans.name = "SpecialEffect";
            }

            var specialEffectObj = specialEffectTrans.gameObject;

            if (!____isCombatSkill)
            {
                specialEffectObj.SetActive(false);
                return;
            }

            argsBox.Get("ItemData", out ItemDisplayData arg);

            if (arg == null)
            {
                specialEffectObj.SetActive(false);
                return;
            }

            var skillBookItem = SkillBook.Instance[arg.Key.TemplateId];

            if (skillBookItem == null)
            {
                specialEffectObj.SetActive(false);
                return;
            }

            var list = EasyPool.Get<List<short>>();
            list.Clear();
            list.Add(skillBookItem.CombatSkillTemplateId);

            var domainId = DomainHelper.DomainName2DomainId["CombatSkill"];
            var methodId = MynahBaseModFrontend.MynahBaseModFrontend.GetMethodIdByName(
                typeof(CombatSkillDomainHelper.MethodIds),
                "GetCombatSkillDisplayData"
            );

            __instance.AsynchMethodCall(domainId, methodId, SingletonObject.getInstance<BasicGameData>().TaiwuCharId,
                list,
                (offset, dataPool) =>
                {
                    var combatSkillItem = CombatSkill.Instance[skillBookItem.CombatSkillTemplateId];
                    var item = EasyPool.Get<List<CombatSkillDisplayData>>();
                    Serializer.Deserialize(dataPool, offset, ref item);
                    var combatSkillDisplayData = item[0];
                    EasyPool.Free(item);
                    var flag = combatSkillDisplayData.EffectType != -1;

                    if (ShowBookFiveElements)
                    {
                        var fiveElementsStr = LocalStringManager
                            .Get($"LK_FiveElements_Type_{combatSkillItem.FiveElements}")
                            .SetColor(Colors.Instance.FiveElementsColors[combatSkillItem.FiveElements]);
                        var bookTypeStr = CombatSkillType.Instance[skillBookItem.CombatSkillType].Name;
                        var bookSubtypeStr =
                            LocalStringManager.Get($"LK_ItemSubType_{(object)skillBookItem.ItemSubType}");
                        __instance.CGet<TextMeshProUGUI>("SubType").text =
                            fiveElementsStr + bookTypeStr + bookSubtypeStr;
                    }

                    if (true) // flag
                    {
                        var flag4 = combatSkillDisplayData.EffectType == 0;

                        ShowAllSpecialEffects(specialEffectObj, combatSkillItem, flag, flag4);

                        //
                        // var prefix1 = flag && flag4 ? "当前：" : "如果正练：";
                        // var prefix2 = flag && !flag4 ? "当前：" : "如果逆练：";
                        //
                    }

                    if (ShowLearningProgress)
                    {
                        var s = GetCombatSkillReadingProgressString(combatSkillDisplayData);
                        var pracStr = combatSkillDisplayData.PracticeLevel < 0
                            ? "未习得"
                            : $"修习程度：{combatSkillDisplayData.PracticeLevel}%";

                        var desc = $"{skillBookItem.Desc}\n{s}\n{pracStr}";
                        MouseTip_Util.SetMultiLineAutoHeightText(__instance.CGet<TextMeshProUGUI>("Desc"), desc);
                    }
                });

            EasyPool.Free(list);
            specialEffectObj.SetActive(true);
        }

        public static void PreFixRefreshSkillItem(ref bool visibleSkill)
        {
            if (ShowNonPublicSkill)
            {
                visibleSkill = true;
            }
        }

        public static void PostFixUpdateOnlyTemplateData(MouseTipCombatSkill __instance,
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

            ShowAttackPartDistribution(__instance, ____configData);
        }

        public static void Postfix(MouseTipCombatSkill __instance, CombatSkillItem ____configData, int offset,
            RawDataPool dataPool)
        {
            if (!ShowCombatSkillSpecialEffect) return;
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

                if (ShowLearningProgress)
                {
                    var s = GetCombatSkillReadingProgressString(combatSkillDisplayData);
                    var desc = $"{____configData.Desc}\n{s}";
                    MouseTip_Util.SetMultiLineAutoHeightText(__instance.CGet<TextMeshProUGUI>("Desc"), desc);
                }

                var element = __instance.Element;
                if (element != null)
                {
                    element.ShowAfterRefresh();
                }
            }
        }

        private static void UpdateSpecialEffectText(TextMeshProUGUI effectText, string effectStr)
        {
            // effectStr = "     " + effectStr;
            var x = effectText.rectTransform.sizeDelta.x;
            var preferredValues = effectText.GetPreferredValues(effectStr, x, float.PositiveInfinity);
            effectText.rectTransform.sizeDelta = preferredValues.SetX(x);
            effectText.text = effectStr;
        }

        private static void ShowAllSpecialEffects(GameObject specialEffectObj, CombatSkillItem combatSkillItem,
            bool active, bool activeDirection)
        {
            specialEffectObj.transform.Find("DirectEffectTitle").gameObject.SetActive(true); // flag4
            var directDesc = specialEffectObj.transform.Find("DirectDesc");
            directDesc.gameObject.SetActive(true);
            specialEffectObj.transform.Find("ReverseEffectTitle").gameObject.SetActive(true); // !flag4
            var reverseDesc = specialEffectObj.transform.Find("ReverseDesc");
            reverseDesc.gameObject.SetActive(true);

            var template1 = active && activeDirection ? "     当前：{0}" : "     如果正练：{0}".SetColor("lightgrey");
            var template2 = active && !activeDirection ? "     当前：{0}" : "     如果逆练：{0}".SetColor("lightgrey");

            UpdateSpecialEffectText(specialEffectObj.transform.Find("DirectDesc/DirectEffectDesc")
                .GetComponent<TextMeshProUGUI>(), string.Format(template1,
                SpecialEffect
                    .Instance[combatSkillItem.DirectEffectID]
                    .Desc[0]));
            UpdateSpecialEffectText(specialEffectObj.transform.Find("ReverseDesc/ReverseEffectDesc")
                .GetComponent<TextMeshProUGUI>(), string.Format(template2,
                SpecialEffect
                    .Instance[combatSkillItem.ReverseEffectID]
                    .Desc[0]));
        }

        private static string GetCombatSkillReadingProgressString(CombatSkillDisplayData combatSkillDisplayData)
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

        private static void ShowAttackPartDistribution(MouseTipCombatSkill __instance,
            CombatSkillItem ____configData)
        {
            if (!ShowAttackDistribution) return;

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
    }
}