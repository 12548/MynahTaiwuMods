using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using Game.Views.MouseTips.Item;
using GameData.Domains;
using GameData.Domains.CombatSkill;
using GameData.Domains.Item.Display;
using GameData.Serializer;
using HarmonyLib;
using TMPro;
using UnityEngine;
using CombatSkillType = Config.CombatSkillType;

namespace MynahMoreInfo;

[HarmonyPatch]
public class MouseTipBookPatch
{
    [HarmonyPatch(typeof(TooltipBook), "Refresh")]
    [HarmonyPostfix]
    public static void InitPostfix(TooltipBook __instance)
    {
        // if (!ModEntry.ShowBookSpecialEffect) return;
        if (__instance == null) return;

        var ____isCombatSkill = __instance._isCombatSkill;
        
        // __instance.GetComponent<RectTransform>().SetWidth(790);
        // __instance.transform.Find("SeparateLine").GetComponent<RectTransform>().SetWidth(750);
    
        // var dh = __instance.transform.Find("DescriptionHolder");
        // if (dh == null)
        // {
        //     Debug.Log("cant find DescriptionHolder");
        //     return;
        // }
        //
        // var combatSkillTip = UIElement.MouseTipCombatSkill.UiBaseAs<MouseTipCombatSkill>();
        // if (combatSkillTip == null || combatSkillTip.transform == null)
        // {
        //     Debug.Log("cant find combatSkillTip transform");
        //     return;
        // }
        //
        // var specialHolder = combatSkillTip.transform.Find("DescriptionHolder/SpecialEffect");
        // if (specialHolder == null)
        // {
        //     Debug.Log("cant find specialHolder");
        //     return;
        // }
        //
        // if (specialHolder.gameObject == null)
        // {
        //     Debug.Log("cant find specialHolderGO");
        //     return;
        // }
    
        // var specialEffectTrans = dh.Find("SpecialEffect");
        // if (specialEffectTrans == null)
        // {
        //     specialEffectTrans =
        //         Object.Instantiate(specialHolder.gameObject, dh, false).transform;
        //     specialEffectTrans.name = "SpecialEffect";
        // }
        //
        // var specialEffectObj = specialEffectTrans.gameObject;
        //
        if (!____isCombatSkill)
        {
            // specialEffectObj.SetActive(false);
            return;
        }
        
        if(__instance._itemData == null) return;
    
        var skillBookItem = SkillBook.Instance[__instance._itemData.Key.TemplateId];
    
        if (skillBookItem == null)
        {
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
    
        __instance.AsyncMethodCall(domainId, methodId, SingletonObject.getInstance<BasicGameData>().TaiwuCharId,
            list,
            (offset, dataPool) =>
            {
                var combatSkillItem = CombatSkill.Instance[skillBookItem.CombatSkillTemplateId];
                var item = EasyPool.Get<List<CombatSkillDisplayData>>();
                Serializer.Deserialize(dataPool, offset, ref item);
                var combatSkillDisplayData = item[0];
                EasyPool.Free(item);
                var flag = combatSkillDisplayData.EffectType != -1;
    
                if (ModEntry.ShowBookFiveElements)
                {
                    var fiveElementsStr = LocalStringManager
                        .Get($"LK_FiveElements_Type_{combatSkillItem.FiveElements}")
                        .SetColor(Colors.Instance.FiveElementsColors[combatSkillItem.FiveElements]);
                    var bookTypeStr = CombatSkillType.Instance[skillBookItem.CombatSkillType].Name;
                    var bookSubtypeStr =
                        LocalStringManager.Get($"LK_ItemSubType_{(object)skillBookItem.ItemSubType}");
                    __instance.commonArea.textMaterial.text = 
                        fiveElementsStr + __instance.commonArea.textMaterial.text;
                }

                var castTime = combatSkillItem.PrepareTotalProgress;
                if (ModEntry.ShowCastTime && castTime > 0)
                {
                    __instance.commonArea.textDesc.text += $"\n基本施展时间：{(castTime / 7200.0):0.##}秒";
                }
    
                // if (true) // flag
                // {
                //     var flag4 = combatSkillDisplayData.EffectType == 0;
                //
                //     MouseTipCombatSkillPatch.ShowAllSpecialEffects(specialEffectObj, combatSkillItem, flag, flag4);
                //
                //     //
                //     // var prefix1 = flag && flag4 ? "当前：" : "如果正练：";
                //     // var prefix2 = flag && !flag4 ? "当前：" : "如果逆练：";
                //     //
                // }
                //
                // if (ModEntry.ShowLearningProgress)
                // {
                //     var s = MouseTipCombatSkillPatch.GetCombatSkillReadingProgressString(combatSkillDisplayData);
                //     var pracStr = combatSkillDisplayData.PracticeLevel < 0
                //         ? "未习得"
                //         : $"修习程度：{combatSkillDisplayData.PracticeLevel}%";
                //
                //     var desc = $"{skillBookItem.Desc}\n{s}\n{pracStr}";
                //     MouseTip_Util.SetMultiLineAutoHeightText(__instance.CGet<TextMeshProUGUI>("Desc"), desc);
                // }
            });
    
        EasyPool.Free(list);
    }
}