using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using GameData.Domains;
using GameData.Domains.Character.Display;
using GameData.Domains.CombatSkill;
using GameData.Domains.Item.Display;
using GameData.Domains.TaiwuEvent.DisplayEvent;
using GameData.Serializer;
using GameData.Utilities;
using HarmonyLib;
using MynahBaseModBase;
using TaiwuModdingLib.Core.Plugin;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MynahMoreInfo;

[PluginConfig("MynahMoreInfo", "myna12548", "1")]
public class ModEntry : TaiwuRemakeHarmonyPlugin
{
    [ModSetting("显示不传之秘", description: "显示门派武学列表中的不传之秘")]
    public static readonly bool ShowNonPublicSkill = true;

    [ModSetting("左侧人物浮窗", description: "为地图左侧人物列表增加鼠标浮窗")]
    public static readonly bool ShowMouseTipMapBlockCharList = true;

    [ModSetting("功法正逆练", description: "显示功法的正逆练效果")]
    public static readonly bool ShowCombatSkillSpecialEffect = true;

    [ModSetting("书籍正逆练", description: "显示书籍所载功法的正逆练效果")]
    public static readonly bool ShowBookSpecialEffect = true;

    [ModSetting("打点显示", description: "显示催破功法的打点分布")]
    public static readonly bool ShowAttackDistribution = true;

    [ModSetting("学习进度", description: "开启正逆练显示时生效，显示功法、功法书籍的读书、修炼进度，目前显示位置不太好，不喜欢可以关闭")]
    public static readonly bool ShowLearningProgress = true;
    
    [ModSetting("对话人物浮窗", description: "为事件界面（人物对话互动等）的左右两个人物增加鼠标浮窗")]
    public static readonly bool ShowEventUICharacterMouseTip = true;
    
    [ModSetting("村民人物浮窗", description: "为居民信息块（工作派遣等）增加鼠标浮窗（头像边上的空白处）")]
    public static readonly bool ShowResidentCharacterMouseTip = true;

    public override void OnModSettingUpdate()
    {
        base.OnModSettingUpdate();
        MynahBaseModFrontend.MynahBaseModFrontend.OnModSettingUpdate(this);
    }

    public override void Initialize()
    {
        base.Initialize();
        OnModSettingUpdate();

        var original = AccessTools.FirstMethod(typeof(MouseTipCombatSkill),
            it => it.Name.Contains("OnGetSkillDisplayData"));
        var postfix = typeof(Patch).GetMethod("Postfix");
        this.HarmonyInstance.Patch(original, postfix: new HarmonyMethod(postfix));

        var original2 = AccessTools.FirstMethod(typeof(MouseTipCombatSkill),
            it => it.Name.Contains("UpdateOnlyTemplateData"));
        var postfix2 = typeof(Patch).GetMethod("PostFixUpdateOnlyTemplateData");
        this.HarmonyInstance.Patch(original2, postfix: new HarmonyMethod(postfix2));

        var o3 = AccessTools.FirstMethod(typeof(UI_CombatSkillTree), it => it.Name.Contains("RefreshSkillItem"));
        var prefix3 = typeof(Patch).GetMethod("PreFixRefreshSkillItem");
        this.HarmonyInstance.Patch(o3, prefix: new HarmonyMethod(prefix3));


        var o4 = AccessTools.FirstMethod(typeof(MouseTipBook), it => it.Name.Contains("Init"));
        var postfix4 = typeof(Patch).GetMethod("PostFixMouseTipBookInit");
        this.HarmonyInstance.Patch(o4, postfix: new HarmonyMethod(postfix4));

        Debug.Log("ShowSpecialEffect all patched");
    }

    [HarmonyPatch]
    public static class MouseTipCharacterPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UI_EventWindow), "UpdateMainCharacter")]
        static void UpdateMainCharacterPostfix(UI_EventWindow __instance)
        {
            if(!ShowEventUICharacterMouseTip) return;
            var data = (TaiwuEventDisplayData)Traverse.Create(__instance).Property("Data").GetValue();
            CharacterDisplayData mainCharacter = data.MainCharacter;
            Refers refers = __instance.CGet<Refers>("MainCharacter");
            var transform = refers.transform.Find("MoveRoot/AvatarArea/ShowMainCharacterMenu");
            if (transform == null) return;
            var mouseTipObj = transform.gameObject;

            if (mainCharacter == null || !HasCharacter(__instance))
            {
                if (refers != null)
                {
                    var mouseTipDisplayer = Util.EnsureMouseTipDisplayer(mouseTipObj);
                    mouseTipDisplayer.enabled = false;
                }
            }
            else
            {
                var mouseTipDisplayer = Util.EnsureMouseTipDisplayer(mouseTipObj);
                Util.EnableMouseTipCharacter(mouseTipDisplayer, mainCharacter.CharacterId);
            }
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UI_EventWindow), "UpdateTargetCharacter")]
        static void UpdateTargetCharacterPostfix(UI_EventWindow __instance)
        {
            if(!ShowEventUICharacterMouseTip) return;
            var data = (TaiwuEventDisplayData)Traverse.Create(__instance).Property("Data").GetValue();
            CharacterDisplayData mainCharacter = data.TargetCharacter;
            Refers refers = __instance.CGet<Refers>("TargetCharacter");
            var transform = refers.transform.Find("CanvasChanger/AvatarArea/ShowTargetCharacterMenu");
            if(transform == null) return;
            var mouseTipObj = transform.gameObject;

            if (mainCharacter == null || !HasCharacter(__instance))
            {
                if (refers != null)
                {
                    var mouseTipDisplayer = Util.EnsureMouseTipDisplayer(mouseTipObj);
                    mouseTipDisplayer.enabled = false;
                }
            }
            else
            {
                var mouseTipDisplayer = Util.EnsureMouseTipDisplayer(mouseTipObj);
                Util.EnableMouseTipCharacter(mouseTipDisplayer, mainCharacter.CharacterId);
            }
        }
        
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(UI_EventWindow), "HasCharacter")]
        static bool HasCharacter(UI_EventWindow instance)
        {
            throw new Exception("stub!");
        }
        
        /// <summary>
        /// 纯为了避免出错，设置一个直接跳过的条件
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="dataPool"></param>
        /// <returns></returns>
        [HarmonyPatch(typeof(MouseTipCharacter), "OnGetCharDisplayData")]
        [HarmonyPrefix]
        static bool MouseTipOnGetDataPrefix(int offset, RawDataPool dataPool)
        {
            List<CharacterDisplayData> list = EasyPool.Get<List<CharacterDisplayData>>();
            list.Clear();
            Serializer.Deserialize(dataPool, offset, ref list);
            if (list.Count != 1)
            {
                EasyPool.Free(list);
                return false;
            }

            EasyPool.Free(list);
            return true;
        }

        [HarmonyPatch(typeof(UI_MapBlockCharList), "OnRenderChar")]
        [HarmonyPostfix]
        static void OnRenderCharPostfix(int index, Refers charRefers, bool ____canSeeDetail, CToggleGroup ____togGroup,
            List<int> ____infectedList, List<int> ____normalCharList,
            Dictionary<int, CharacterDisplayData> ____charDataDict)
        {
            var trigger = ____canSeeDetail && ShowMouseTipMapBlockCharList;
            var key = ____togGroup.GetActive().Key;
            var showingRandomEnemy = key == 1 && index >= ____infectedList.Count;
            if (showingRandomEnemy) trigger = false;
            if (key > 1) trigger = false;

            var cbutton = charRefers.CGet<CButton>("Button");
            var obj = cbutton.gameObject;
            
            var mouseTipDisplayer = Util.EnsureMouseTipDisplayer(obj);

            if (trigger)
            {
                
                var charDisplayData = ____charDataDict[
                    ((key == 0) ? ____normalCharList : ____infectedList)[
                        index]];
                var characterId = charDisplayData.CharacterId;

                Util.EnableMouseTipCharacter(mouseTipDisplayer, characterId);
            }
            else
            {
                mouseTipDisplayer.enabled = false;
            }
            // mouseTipDisplayer.Type = TipType.SingleDesc;
            //

            //
            // CharacterItem item = Character.Instance.GetItem(Character.Instance[charDisplayData.TemplateId].TemplateId);
            // var s = "";
            // s += CommonUtils.GetGenderString(charDisplayData.Gender) + "·";
            // s += CommonUtils.GetCharmLevelText(
            //     item.BaseAttraction,
            //     item.Gender,
            //     20, 1) + "(基础值)·";
            // s += CommonUtils.GetBehaviorString(charDisplayData.BehaviorType) + "·";
            // s += SingletonObject.getInstance<WorldMapModel>().GetSettlementName(charDisplayData.OrgInfo);
            // s += GetIdentityText(item, charDisplayData.OrgInfo);
            //
            // mouseTipDisplayer.PresetParam = new[] { s };
        }

        // static string GetIdentityText(CharacterItem item, OrganizationInfo organizationInfo)
        // {
        //     OrganizationItem organizationItem = Organization.Instance[organizationInfo.OrgTemplateId];
        //
        //     OrganizationMemberItem organizationMemberItem =
        //         OrganizationMember.Instance[organizationItem.Members[(int)organizationInfo.Grade]];
        //     bool flag4 = item.ActualAge >= 0 && item.ActualAge < organizationMemberItem.IdentityActiveAge;
        //     if (flag4)
        //     {
        //         string text = LocalStringManager.Get((ushort)((AgeGroup.GetAgeGroup(item.ActualAge) == 0)
        //             ? 2027
        //             : ((item.Gender == 0) ? 2029 : 2028)));
        //         text = string.Concat(new string[]
        //         {
        //             "<color=",
        //             Colors.Instance.GradeColors[(int)organizationInfo.Grade].ColorToHexString("#"),
        //             ">",
        //             text,
        //             "</color>"
        //         });
        //         return text;
        //     }
        //
        //     string text2 = organizationInfo.Principal
        //         ? organizationMemberItem.GradeName
        //         : organizationMemberItem.SpouseAnonymousTitles[(int)item.Gender];
        //     text2 = string.Concat(new string[]
        //     {
        //         "<color=",
        //         Colors.Instance.GradeColors[organizationInfo.Grade].ColorToHexString("#"),
        //         ">",
        //         text2,
        //         "</color>"
        //     });
        //     return text2;
        // }
    }


    public static class Patch
    {
        static GameObject _attackDistributionObj;

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

            SkillBookItem skillBookItem = SkillBook.Instance[arg.Key.TemplateId];

            if (skillBookItem == null)
            {
                specialEffectObj.SetActive(false);
                return;
            }

            List<short> list = EasyPool.Get<List<short>>();
            list.Clear();
            list.Add(skillBookItem.CombatSkillTemplateId);

            var domainId = DomainHelper.DomainName2DomainId["CombatSkill"];
            var methodId = MynahBaseModFrontend.MynahBaseModFrontend.GetMethodIdByName(
                typeof(CombatSkillDomainHelper.MethodIds),
                "GetCombatSkillDisplayData"
            );

            __instance.AsynchMethodCall(domainId, methodId, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, list,
                (offset, dataPool) =>
                {
                    var combatSkillItem = CombatSkill.Instance[skillBookItem.CombatSkillTemplateId];
                    var item = EasyPool.Get<List<CombatSkillDisplayData>>();
                    Serializer.Deserialize(dataPool, offset, ref item);
                    var combatSkillDisplayData = item[0];
                    EasyPool.Free(item);
                    var flag = combatSkillDisplayData.EffectType != -1;

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
                        string s = GetCombatSkillReadingProgressString(combatSkillDisplayData);
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
            if (!ShowCombatSkillSpecialEffect) return;
            __instance.CGet<GameObject>("DirectEffectTitle").SetActive(true);
            __instance.CGet<GameObject>("DirectDesc").SetActive(true);
            __instance.CGet<GameObject>("ReverseEffectTitle").SetActive(true);
            __instance.CGet<GameObject>("ReverseDesc").SetActive(true);

            __instance.CGet<TextMeshProUGUI>("DirectEffectDesc").text =
                ("     " + SpecialEffect.Instance[____configData.DirectEffectID].Desc[0]);
            __instance.CGet<TextMeshProUGUI>("ReverseEffectDesc").text =
                ("     " + SpecialEffect.Instance[____configData.ReverseEffectID].Desc[0]);

            ShowAttackPartDistribution(__instance, ____configData);
        }

        public static void Postfix(MouseTipCombatSkill __instance, CombatSkillItem ____configData, int offset,
            RawDataPool dataPool)
        {
            if (!ShowCombatSkillSpecialEffect) return;
            if (__instance != null)
            {
                GameObject specialEffectGameObject = __instance.CGet<GameObject>("SpecialEffect");

                var uiCombat = UIElement.Combat.UiBaseAs<UI_Combat>();

                if (uiCombat != null && uiCombat.gameObject.activeInHierarchy)
                {
                    return;
                }

                List<CombatSkillDisplayData> item = EasyPool.Get<List<CombatSkillDisplayData>>();
                Serializer.Deserialize(dataPool, offset, ref item);
                CombatSkillDisplayData combatSkillDisplayData = item[0];
                EasyPool.Free(item);
                bool flag = combatSkillDisplayData.EffectType != -1;

                specialEffectGameObject.SetActive(true);
                if (true) // flag
                {
                    bool flag4 = combatSkillDisplayData.EffectType == 0;
                    ShowAllSpecialEffects(specialEffectGameObject, ____configData, flag, flag4);
                }

                ShowAttackPartDistribution(__instance, ____configData);

                if (ShowLearningProgress)
                {
                    string s = GetCombatSkillReadingProgressString(combatSkillDisplayData);
                    var desc = $"{____configData.Desc}\n{s}";
                    MouseTip_Util.SetMultiLineAutoHeightText(__instance.CGet<TextMeshProUGUI>("Desc"), desc);
                }

                UIElement element = __instance.Element;
                if (element != null)
                {
                    element.ShowAfterRefresh();
                }
            }
        }

        private static void ShowAllSpecialEffects(GameObject specialEffectObj, CombatSkillItem combatSkillItem,
            bool active, bool activeDirection)
        {
            specialEffectObj.transform.Find("DirectEffectTitle").gameObject.SetActive(true); // flag4
            specialEffectObj.transform.Find("DirectDesc").gameObject.SetActive(true);
            specialEffectObj.transform.Find("ReverseEffectTitle").gameObject.SetActive(true); // !flag4
            specialEffectObj.transform.Find("ReverseDesc").gameObject.SetActive(true);

            // specialEffectObj.CGet<GameObject>("DirectEffectTitle").SetActive(true); // flag4
            // specialEffectObj.CGet<GameObject>("DirectDesc").SetActive(true);
            // specialEffectObj.CGet<GameObject>("ReverseEffectTitle").SetActive(true); // !flag4
            // specialEffectObj.CGet<GameObject>("ReverseDesc").SetActive(true);

            var template1 = active && activeDirection ? "     当前：{0}" : "     如果正练：{0}".SetColor("lightgrey");
            var template2 = active && !activeDirection ? "     当前：{0}" : "     如果逆练：{0}".SetColor("lightgrey");

            // var prefix1 = active && activeDirection ? "当前：" : "如果正练：";
            // var prefix2 = active && !activeDirection ? "当前：" : "如果逆练：";

            specialEffectObj.transform.Find("DirectDesc/DirectEffectDesc")
                .GetComponent<TextMeshProUGUI>().text = string.Format(template1,
                SpecialEffect
                    .Instance[combatSkillItem.DirectEffectID]
                    .Desc[0]);
            specialEffectObj.transform.Find("ReverseDesc/ReverseEffectDesc")
                .GetComponent<TextMeshProUGUI>().text = string.Format(template2,
                SpecialEffect
                    .Instance[combatSkillItem.ReverseEffectID]
                    .Desc[0]);

            // uiBase.CGet<TextMeshProUGUI>("DirectEffectDesc").text = string.Format(template1,
            //     SpecialEffect.Instance[combatSkillItem.DirectEffectID].Desc[0]);
            // uiBase.CGet<TextMeshProUGUI>("ReverseEffectDesc").text = string.Format(template2,
            //     SpecialEffect.Instance[combatSkillItem.DirectEffectID].Desc[0]);

            // uiBase.CGet<TextMeshProUGUI>("DirectEffectDesc").text =
            //     ("     " + prefix1 + SpecialEffect.Instance[combatSkillItem.DirectEffectID].Desc[0]);
            // uiBase.CGet<TextMeshProUGUI>("ReverseEffectDesc").text = ("     " +
            //                                                           prefix2 + SpecialEffect
            //                                                               .Instance[
            //                                                                   combatSkillItem.ReverseEffectID]
            //                                                               .Desc[0]);
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
            for (int i = 0; i < 5; i++)
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

            if (____configData.EquipType == 1)
            {
                GameObject attackEffectObj = __instance.CGet<GameObject>("AttackProperty");
                var transform = attackEffectObj.transform.Find("AttackDisturbTips");
                GameObject adt;
                if (transform == null)
                {
                    var t = attackEffectObj.transform.Find("AcupointTips").gameObject;
                    adt = Object.Instantiate(t, attackEffectObj.transform, false);
                    adt.name = "AttackDisturbTips";
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