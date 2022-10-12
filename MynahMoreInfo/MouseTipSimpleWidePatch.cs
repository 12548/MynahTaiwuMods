﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using Config;
using FrameWork;
using GameData.Domains;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Mod;
using GameData.Serializer;
using HarmonyLib;
using MiniJSON;
using TMPro;
using UICommon.Character.Elements;
using UnityEngine;
using LifeSkillType = Config.LifeSkillType;

namespace MynahMoreInfo;

[HarmonyPatch(typeof(MouseTipSimpleWide), "Init")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class MouseTipSimpleWidePatch
{
    public static void Postfix(MouseTipSimpleWide __instance, ArgumentBox argsBox)
    {
        // string str;
        // argsBox.Get("arg0", out str);
        // string srcString;
        // argsBox.Get("arg1", out srcString);
        // textMeshProUgui1.text = str;
        // textMeshProUgui2.text = srcString.ColorReplace();
        // textMeshProUgui2.GetComponent<TMPTextSpriteHelper>().Parse();
        var isCharInfo = argsBox.Get("_mmi_mtc_charDisplayData", out CharacterDisplayData displayData);
        if (!isCharInfo) return;
        var titleText = __instance.CGet<TextMeshProUGUI>("Title");
        var contentText = __instance.CGet<TextMeshProUGUI>("Desc");
        var gCall = new GroupCallBuilder();
        var a1 = gCall.AddAction("GroupCharDisplayDataList");
        var a2 = gCall.AddAction("CharacterData");

        __instance.AsynchMethodCall(DomainHelper.DomainIds.Character,
            CharacterDomainHelper.MethodIds.GetGroupCharDisplayDataList,
            new List<int> { displayData.CharacterId },
            a1);

        __instance.AsynchMethodCall(DomainHelper.DomainIds.Mod,
            ModDomainHelper.MethodIds.GetString,
            ModEntry.StaticModIdStr,
            $"GetCharacterData|{displayData.CharacterId}",
            false,
            a2);

        gCall.OnAllOver = dict =>
        {
            var (gcddOffset, gcddDatapool) = dict["GroupCharDisplayDataList"];
            var (cdOffset, cdDatapool) = dict["CharacterData"];
            var item = EasyPool.Get<List<GroupCharDisplayData>>();
            Serializer.Deserialize(gcddDatapool, gcddOffset, ref item);
            if (item.Count < 1) return;
                
            var groupCharDisplayData = item[0];
            EasyPool.Free(item);

            var cdString = "";
            Serializer.Deserialize(cdDatapool, cdOffset, ref cdString);

            var cdDict = (Dictionary<string, object>)Json.Deserialize(cdString);
            if (cdDict != null)
            {
                var objects = (List<object>)cdDict["FeatureIds"];
                var lovingItemSubType = Convert.ToInt32(cdDict["LovingItemSubType"]);
                var hatingItemSubType = Convert.ToInt32(cdDict["HatingItemSubType"]);
                SetContent(displayData, groupCharDisplayData,
                    objects.Select(Convert.ToInt32).ToList(),
                    lovingItemSubType,
                    hatingItemSubType,
                    titleText, contentText
                );
            }
            else
            {
                Debug.LogWarning($"Failed to parse json: {cdString} on char: {displayData.CharacterId}");
            }
        };
    }


    private static void SetContent(
        CharacterDisplayData displayData,
        GroupCharDisplayData groupCharDisplayData,
        List<int> featureIds,
        int _lovingItemSubType,
        int _hatingItemSubType,
        TextMeshProUGUI titleText,
        TextMeshProUGUI contentText
    )
    {
        var isTaiwu = displayData.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
        var showName = NameCenter.GetCharMonasticTitleOrNameByDisplayData(displayData, isTaiwu);
        var realName = NameCenter.GetNameByDisplayData(displayData, isTaiwu, true);
        // var color = displayData.AliveState == 0 ? "white" : "red";

        var worldMapModel = SingletonObject.getInstance<WorldMapModel>();

        // 姓名
        var title = (ModEntry.CharacterMouseTipShowRealName && showName != realName)
            ? $"{(object)showName}/{(object)realName}"
            : showName;

        if (ModEntry.ShowPosAndId)
            title += $"({(object)displayData.CharacterId})";

        // 性别
        title += " · " + CommonUtils.GetGenderString(displayData.Gender);

        // 性别
        title += " · " + groupCharDisplayData.CurrAge + "岁";

        // 性格
        title += " · " + CommonUtils.GetBehaviorString(displayData.BehaviorType);

        // 所属
        title += " · " + CommonUtils.GetOrganizationString(displayData.OrgInfo.OrgTemplateId,
            worldMapModel.SettlementRandNameDict.ContainsKey(displayData.OrgInfo.SettlementId)
                ? worldMapModel.SettlementRandNameDict[displayData.OrgInfo.SettlementId]
                : (short)-1);

        // 身份
        title += CommonUtils.GetCharacterGradeString(displayData.OrgInfo, displayData.Gender,
            displayData.CurrAge);

        // bool isFixedCharacter =
        //     CreatingType.IsFixedPresetType(Character.Instance.GetItem(displayData.TemplateId)
        //         .CreatingType);

        var sb = new StringBuilder();

        var healthInfo =
            CommonUtils.GetCharacterHealthInfo(groupCharDisplayData.Health, groupCharDisplayData.MaxLeftHealth);
        sb.Append("健康：" + healthInfo.Item1.Replace("\n", ""));
        if (!healthInfo.Item1.Contains("("))
        {
            sb.Append($"({groupCharDisplayData.Health}/{groupCharDisplayData.MaxLeftHealth})");
        }

        var charmLevelText = CommonUtils.GetCharmLevelText(
            groupCharDisplayData.Charm,
            displayData.Gender,
            displayData.AvatarRelatedData.DisplayAge,
            displayData.AvatarRelatedData.ClothingDisplayId,
            false, displayData.AvatarRelatedData.AvatarData.FaceVisible);
        sb.Append("<pos=35%>魅力：" + charmLevelText.Replace("\n", ""));
        if (!charmLevelText.Contains("\n"))
        {
            sb.Append($"({groupCharDisplayData.Charm})");
        }

        var happinessType = HappinessType.GetHappinessType(groupCharDisplayData.Happiness);
        sb.Append("<pos=72%>心情：" + CommonUtils.GetHappinessString(happinessType) +
                  $"({groupCharDisplayData.Happiness})");

        sb.AppendLine();

        var favorString = CommonUtils.GetFavorString(groupCharDisplayData.FavorabilityToTaiwu);
        sb.Append("好感：" + favorString.Replace("\n", ""));
        if (!favorString.Contains("\n"))
        {
            sb.Append($"({groupCharDisplayData.FavorabilityToTaiwu})");
        }

        var fameType = FameType.GetFameType(groupCharDisplayData.Fame);
        sb.Append("<pos=35%>声望：" + CommonUtils.GetFameString(fameType) + $"({groupCharDisplayData.Fame})");

        sb.AppendLine("<pos=72%>轮回：" + groupCharDisplayData.PreexistenceCharCount);

        var attackMedal = groupCharDisplayData.AttackMedal;
        var amSprite = attackMedal > 0
            ? "sp_icon_renwutexing_10"
            : (attackMedal < 0 ? "sp_icon_renwutexing_4" : "sp_icon_renwutexing_7");

        sb.Append($"进攻：<sprite=\"mmiSprites\" name=\"{amSprite}\"> x{Mathf.Abs(attackMedal)}");

        var defenceMedal = groupCharDisplayData.DefenceMedal;
        var dmSprite = defenceMedal > 0
            ? "sp_icon_renwutexing_9"
            : (defenceMedal < 0 ? "sp_icon_renwutexing_3" : "sp_icon_renwutexing_6");

        sb.Append($"   守御：<sprite=\"mmiSprites\" name=\"{dmSprite}\"> x{Mathf.Abs(defenceMedal)}</color>");

        var wisdomMedal = groupCharDisplayData.WisdomMedal;
        var wmSprite = groupCharDisplayData.WisdomMedal > 0
            ? "sp_icon_renwutexing_11"
            : (groupCharDisplayData.WisdomMedal < 0 ? "sp_icon_renwutexing_5" : "sp_icon_renwutexing_8");

        sb.Append($"   机略：<sprite=\"mmiSprites\" name=\"{wmSprite}\"> x{Mathf.Abs(wisdomMedal)}</color>");

        var birthMonth = displayData.BirthDate % 12;
        if (birthMonth < 0)
            birthMonth += 12;

        var monthItem = Month.Instance[birthMonth];

        var fiveElements = monthItem.FiveElementsTypeDesc
            .SetColor(Colors.Instance.FiveElementsColors[monthItem.FiveElementsType]);

        sb.AppendLine(
            $"    生于{monthItem.Name}({fiveElements})");

        var loveItemStr = _lovingItemSubType >= 0
            ? LocalStringManager.Get($"LK_ItemSubType_{(object)_lovingItemSubType}")
            : LocalStringManager.Get("LK_None");
        sb.Append($"喜好： <color=#adc7de>{loveItemStr}</color>");
        var hateItemStr = _hatingItemSubType >= 0
            ? LocalStringManager.Get(
                $"LK_ItemSubType_{(object)_hatingItemSubType}")
            : LocalStringManager.Get("LK_None");
        sb.AppendLine($"      厌恶： <color=#843029>{hateItemStr}</color>");

        sb.AppendLine();

        if (ModEntry.ShowCharFeatures > 0)
        {
            if (ModEntry.ShowCharFeatures == 1)
            {
                featureIds = featureIds.Where(it => !CharacterFeature.Instance[it].Hidden).ToList();
            }

            for (var i = 0; i < featureIds.Count; i++)
            {
                var featureId = featureIds[i];
                var featureItem = CharacterFeature.Instance[featureId];

                sb.Append($"<voffset=0.5em><pos={i % 4 * 25}%>");

                for (var index2 = 0; index2 < 3; ++index2)
                {
                    var featureMedal = featureItem.FeatureMedals[index2];
                    for (var index3 = 0; index3 < featureMedal.Values.Count; ++index3)
                    {
                        if (featureMedal.Values.Count > index3)
                        {
                            var featureIconConfig =
                                (string[][])typeof(FeatureItem).GetField("FeatureIconConfig", (BindingFlags)(-1))!
                                    .GetValue(null);
                            var sprite = featureIconConfig[featureMedal.Values[index3]][index2];
                            sb.Append($"<sprite=\"mmiSprites\" name=\"{sprite}\">");
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                sb.Append($"<voffset=-0.5em><pos={i % 4 * 25}%>");
                sb.Append(featureItem.Name);

                if (i % 4 == 3)
                {
                    sb.AppendLine("<voffset=0em>");
                }
                // else
                // {
                //     sb.Append($"<pos={(i % 4 + 1) * 25}%>");
                // }
            }

            sb.AppendLine("<voffset=0em>");
            sb.AppendLine();
        }

        var ageEffectItem =
            AgeEffect.Instance[Mathf.Min(groupCharDisplayData.CurrAge, AgeEffect.Instance.Count - 1)];

        var s = "";
        var num = 0;

        switch (groupCharDisplayData.CombatSkillGrowthType)
        {
            case 0:
                s = (LocalStringManager.Get("LK_Qualification_Growth_Average"));
                num = ageEffectItem.SkillQualificationAverage;
                break;
            case 1:
                s = (LocalStringManager.Get("LK_Qualification_Growth_Precocious"));
                num = ageEffectItem.SkillQualificationPrecocious;
                break;
            case 2:
                s = (LocalStringManager.Get("LK_Qualification_Growth_LateBlooming"));
                num = ageEffectItem.SkillQualificationLateBlooming;
                break;
        }

        s += num switch
        {
            > 0 => ($"+{(object)num}".SetColor("lightblue")),
            0 => ("+0".SetColor("lightgrey")),
            _ => ($"{(object)num}".SetColor("red"))
        };

        sb.AppendLine($"功法资质（{s}）：");

        for (var i = 0; i < CombatSkillType.Instance.Count; i++)
        {
            var skillType = CombatSkillType.Instance[i];
            var sqValue = _getCSQValue(groupCharDisplayData, i);
            var sqColor = CommonUtils.GetCharacterSkillColorByValue((short)sqValue);
            sb.Append($"{skillType.Name} {sqValue}".SetColor(sqColor));
            if (i % 4 == 3)
            {
                sb.AppendLine();
            }
            else
            {
                sb.Append($"<pos={(i % 4 + 1) * 25}%>");
            }
        }

        sb.AppendLine();

        switch (groupCharDisplayData.LifeSkillGrowthType)
        {
            case 0:
                s = (LocalStringManager.Get("LK_Qualification_Growth_Average"));
                num = ageEffectItem.SkillQualificationAverage;
                break;
            case 1:
                s = (LocalStringManager.Get("LK_Qualification_Growth_Precocious"));
                num = ageEffectItem.SkillQualificationPrecocious;
                break;
            case 2:
                s = (LocalStringManager.Get("LK_Qualification_Growth_LateBlooming"));
                num = ageEffectItem.SkillQualificationLateBlooming;
                break;
        }

        s += num switch
        {
            > 0 => ($"+{(object)num}".SetColor("lightblue")),
            0 => ("+0".SetColor("lightgrey")),
            _ => ($"{(object)num}".SetColor("red"))
        };

        sb.AppendLine($"技艺资质（{s}）：");

        for (var i = 0; i < LifeSkillType.Instance.Count; i++)
        {
            var skillType = LifeSkillType.Instance[i];
            var sqValue = _getLSQValue(groupCharDisplayData, i);
            var sqColor = CommonUtils.GetCharacterSkillColorByValue((short)sqValue);
            sb.Append($"{skillType.Name} {sqValue}".SetColor(sqColor));
            if (i % 4 == 3)
            {
                sb.AppendLine();
            }
            else
            {
                sb.Append($"<pos={(i % 4 + 1) * 25}%>");
            }
        }

        if (!titleText.isActiveAndEnabled || !contentText.isActiveAndEnabled) return;
        titleText.text = title;
        contentText.text = sb.ToString().ColorReplace();
        contentText.GetComponent<TMPTextSpriteHelper>().Parse();
    }

    private static unsafe int _getCSQValue(GroupCharDisplayData dd, int index)
    {
        return dd.CombatSkillQualifications.Items[index];
    }

    private static unsafe int _getLSQValue(GroupCharDisplayData dd, int index)
    {
        return dd.LifeSkillQualifications.Items[index];
    }
}