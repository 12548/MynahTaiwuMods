using System;
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
using GameData.Domains.Item;
using GameData.Domains.Mod;
using GameData.Serializer;
using HarmonyLib;
using JetBrains.Annotations;
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
        var hasCharId = argsBox.Get("_mmi_charId", out int charId);
        var hasShowLocation = argsBox.Get("_mmi_locationShow", out bool _);
        var hasDisplayData = argsBox.Get("_mmi_mtc_charDisplayData", out CharacterDisplayData displayData);
        if (!hasDisplayData && !hasCharId) return;

        if (hasDisplayData)
        {
            charId = displayData.CharacterId;
        }

        var titleText = __instance.CGet<TextMeshProUGUI>("Title");
        var contentText = __instance.CGet<TextMeshProUGUI>("Desc");
        var gCall = new GroupCallBuilder();

        if (!hasDisplayData)
        {
            __instance.AsynchMethodCall(DomainHelper.DomainIds.Character,
                CharacterDomainHelper.MethodName2MethodId["GetCharacterDisplayDataList"],
                new List<int> { charId },
                gCall.AddAction("CharacterDisplayDataList"));
        }

        __instance.AsynchMethodCall(DomainHelper.DomainIds.Character,
            CharacterDomainHelper.MethodName2MethodId["GetGroupCharDisplayDataList"],
            new List<int> { charId },
            gCall.AddAction("GroupCharDisplayDataList"));

        __instance.AsynchMethodCall(DomainHelper.DomainIds.Mod,
            ModDomainHelper.MethodIds.GetString,
            ModEntry.StaticModIdStr,
            $"GetCharacterData|{charId}",
            false,
            gCall.AddAction("CharacterData"));

        gCall.OnAllOver = dict =>
        {
            try
            {
                GroupCharDisplayData groupCharDisplayData = null;
                var (gcddOffset, gcddDatapool) = dict["GroupCharDisplayDataList"];

                var item = EasyPool.Get<List<GroupCharDisplayData>>();
                Serializer.Deserialize(gcddDatapool, gcddOffset, ref item);
                if (item.Count > 0)
                {
                    groupCharDisplayData = item[0];
                }

                EasyPool.Free(item);

                if (dict.ContainsKey("CharacterDisplayDataList"))
                {
                    var (cddOffset, cddDatapool) = dict["CharacterDisplayDataList"];
                    var item2 = EasyPool.Get<List<CharacterDisplayData>>();
                    Serializer.Deserialize(cddDatapool, cddOffset, ref item2);
                    if (item2.Count > 0)
                    {
                        displayData = item2[0];
                    }

                    EasyPool.Free(item2);
                }

                var (cdOffset, cdDatapool) = dict["CharacterData"];
                var cdString = "";
                Serializer.Deserialize(cdDatapool, cdOffset, ref cdString);

                var cdDict = (Dictionary<string, object>)Json.Deserialize(cdString);
                if (cdDict != null)
                {
                    var objects = (List<object>)cdDict["FeatureIds"];
                    if (!hasShowLocation)
                    {
                        cdDict.Remove("BlockFullName");
                    }

                    SetContent(displayData, groupCharDisplayData,
                        objects.Select(Convert.ToInt32).ToList(),
                        titleText,
                        contentText,
                        cdDict
                    );
                }
                else
                {
                    Debug.LogWarning($"Failed to parse json: {cdString} on char: {charId}");
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                // ignore
                contentText.text = "详细人物浮窗加载失败";
            }
        };
    }


    private static void SetContent(
        CharacterDisplayData displayData,
        [CanBeNull] GroupCharDisplayData groupCharDisplayData,
        List<int> featureIds,
        TextMeshProUGUI titleText,
        TextMeshProUGUI contentText,
        Dictionary<string, object> otherData
    )
    {
        if (displayData == null)
        {
            return;
        }

        var isAlive = groupCharDisplayData != null;

        var isTaiwu = groupCharDisplayData != null && groupCharDisplayData.CharacterId ==
            SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
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


        // 年龄
        if (isAlive)
        {
            title += " · " + groupCharDisplayData!.CurrAge + "岁";
        }

        // 性格
        title += " · " + CommonUtils.GetBehaviorString(displayData.BehaviorType);

        // 所属
        title += " · " + CommonUtils.GetOrganizationString(displayData.OrgInfo.OrgTemplateId,
            worldMapModel.SettlementRandNameDict!.ContainsKey(displayData.OrgInfo.SettlementId)
                ? worldMapModel.SettlementRandNameDict[displayData.OrgInfo.SettlementId]
                : (short)-1);

        // 身份
        title += CommonUtils.GetCharacterGradeString(displayData.OrgInfo, displayData.Gender,
            displayData.CurrAge);

        // bool isFixedCharacter =
        //     CreatingType.IsFixedPresetType(Character.Instance.GetItem(displayData.TemplateId)
        //         .CreatingType);

        var sb = new StringBuilder();

        if (isAlive)
        {
            var healthInfo =
                CommonUtils.GetCharacterHealthInfo(groupCharDisplayData!.Health, groupCharDisplayData.MaxLeftHealth);
            sb.Append("健康：" + healthInfo.Item1.Replace("\n", ""));
            if (!healthInfo.Item1.Contains("("))
            {
                sb.Append($"({groupCharDisplayData.Health}/{groupCharDisplayData.MaxLeftHealth})");
            }
        }
        else
        {
            sb.Append("健康：无");
        }

        var charm = groupCharDisplayData?.Charm ?? Convert.ToInt16(otherData["Attraction"]);
        var charmLevelText = CommonUtils.GetCharmLevelText(
            charm,
            displayData.Gender,
            displayData.AvatarRelatedData.DisplayAge,
            displayData.AvatarRelatedData.ClothingDisplayId,
            false, displayData.AvatarRelatedData.AvatarData.FaceVisible);
        sb.Append("<pos=35%>魅力：" + charmLevelText.Replace("\n", ""));
        if (!charmLevelText.Contains("\n"))
        {
            sb.Append($"({charm})");
        }

        var happiness = groupCharDisplayData?.Happiness ?? Convert.ToSByte(otherData["Happiness"]);
        var happinessType =
            HappinessType.GetHappinessType(happiness);
        sb.Append($"<pos=72%>心情：{CommonUtils.GetHappinessString(happinessType)}({happiness})");

        sb.AppendLine();

        if (isAlive)
        {
            var favorString = CommonUtils.GetFavorString(groupCharDisplayData!.FavorabilityToTaiwu);
            sb.Append("好感：" + favorString.Replace("\n", ""));
            if (!favorString.Contains("\n"))
            {
                sb.Append($"({groupCharDisplayData.FavorabilityToTaiwu})");
            }
        }
        else
        {
            sb.Append("好感：前尘已忘");
        }

        var fameType = groupCharDisplayData != null
            ? FameType.GetFameType(groupCharDisplayData.Fame)
            : Convert.ToSByte(otherData["FameType"]);
        sb.Append("<pos=35%>声望：" + CommonUtils.GetFameString(fameType) +
                  $"({groupCharDisplayData?.Fame.ToString() ?? "-"})");

        sb.AppendLine("<pos=72%>轮回：" + (groupCharDisplayData?.PreexistenceCharCount.ToString() ??
                                        otherData["PreexistenceCharCount"].ToString()));

        if (isAlive)
        {
            var attackMedal = groupCharDisplayData!.AttackMedal;
            var amSprite = attackMedal > 0
                ? "sp_icon_renwutexing_10"
                : (attackMedal < 0 ? "sp_icon_renwutexing_4" : "sp_icon_renwutexing_7");

            sb.Append($"进攻：{Util.GetSpriteStr(amSprite)} x{Mathf.Abs(attackMedal)}");

            var defenceMedal = groupCharDisplayData.DefenceMedal;
            var dmSprite = defenceMedal > 0
                ? "sp_icon_renwutexing_9"
                : (defenceMedal < 0 ? "sp_icon_renwutexing_3" : "sp_icon_renwutexing_6");

            sb.Append($"   守御：{Util.GetSpriteStr(dmSprite)} x{Mathf.Abs(defenceMedal)}</color>");

            var wisdomMedal = groupCharDisplayData.WisdomMedal;
            var wmSprite = groupCharDisplayData.WisdomMedal > 0
                ? "sp_icon_renwutexing_11"
                : (groupCharDisplayData.WisdomMedal < 0 ? "sp_icon_renwutexing_5" : "sp_icon_renwutexing_8");

            sb.Append($"   机略：{Util.GetSpriteStr(wmSprite)} x{Mathf.Abs(wisdomMedal)}</color>");
        }

        var timeManager = SingletonObject.getInstance<TimeManager>();

        var birthMonth = displayData.BirthDate % 12;
        if (birthMonth < 0)
            birthMonth += 12;

        var monthItem = Month.Instance[birthMonth];

        var fiveElements = monthItem.FiveElementsTypeDesc
            .SetColor(Colors.Instance.FiveElementsColors[monthItem.FiveElementsType]);

        if (isAlive)
        {
            sb.AppendLine($"    生于{monthItem.Name}({fiveElements})");
        }
        else
        {
            var BirthDate = timeManager.GetDateDisplayContent(Convert.ToInt32(otherData["BirthDate"]));
            var DeathDate = timeManager.GetDateDisplayContent(Convert.ToInt32(otherData["DeathDate"]));
            sb.AppendLine($"生于{BirthDate} 卒于{DeathDate} 享年{otherData["LiveAge"]}岁");
        }

        if (otherData.TryGetValue("BlockFullName", out var blockFullName))
        {
            var title1 = groupCharDisplayData != null ? "所在" : "葬于";
            sb.Append($"{title1}： {blockFullName}    ");
        }

        if (groupCharDisplayData != null)
        {
            if (otherData.TryGetValue("XiangshuInfection", out var xiangshuInfection))
            {
                sb.Append($"入魔值: {xiangshuInfection}   ");
            }

            var lovingItemSubType = Convert.ToInt32(otherData["LovingItemSubType"]);
            var hatingItemSubType = Convert.ToInt32(otherData["HatingItemSubType"]);

            var loveItemStr = lovingItemSubType >= 0
                ? LocalStringManager.Get($"LK_ItemSubType_{lovingItemSubType}")
                : LocalStringManager.Get("LK_None");

            var bixStr = Convert.ToBoolean(otherData["IsBisexual"]) ? "(双)" : "";
            sb.Append($"喜好： <color=#adc7de>{loveItemStr}</color> {bixStr}");
            var hateItemStr = hatingItemSubType >= 0
                ? LocalStringManager.Get(
                    $"LK_ItemSubType_{hatingItemSubType}")
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
                                sb.Append(Util.GetSpriteStr(sprite));
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

            sb.AppendLine($"功法造诣/资质（{s}）：");

            for (var i = 0; i < CombatSkillType.Instance.Count; i++)
            {
                var skillType = CombatSkillType.Instance[i];
                var sqValue = _getCSQValue(groupCharDisplayData, i);
                var atValue = Convert.ToInt32(((List<object>)otherData["CombatSkillAttainments"])[i]);
                var sqColor = CommonUtils.GetCharacterSkillColorByValue((short)sqValue);
                sb.Append($"{skillType.Name} {atValue}/{sqValue.ToString().SetColor(sqColor)}");
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

            sb.AppendLine($"技艺造诣/资质（{s}）：");

            for (var i = 0; i < LifeSkillType.Instance.Count; i++)
            {
                var skillType = LifeSkillType.Instance[i];
                var sqValue = _getLSQValue(groupCharDisplayData, i);
                var atValue = Convert.ToInt32(((List<object>)otherData["LifeSkillAttainments"])[i]);
                var sqColor = CommonUtils.GetCharacterSkillColorByValue((short)sqValue);
                sb.Append($"{skillType.Name} {atValue}/{sqValue.ToString().SetColor(sqColor)}");
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


            if (otherData.TryGetValue("AvailableLifeSkills", out var value1))
            {
                var list = (List<object>)value1;
                if (list.Count > 0)
                {
                    var str = list.Select(Convert.ToInt32)
                        .Select(it => SkillBook.Instance[it])
                        .Select(it => it.Name.SetGradeColor(it.Grade))
                        .Join(null, "<space=1em>");
                    sb.AppendLine("可请教技艺：" + str);
                }
            }

            if (otherData.TryGetValue("AvailableBattleSkills", out var value))
            {
                var list = (List<object>)value;
                if (list.Count > 0)
                {
                    var str = list.Select(Convert.ToInt32)
                        .Select(it => CombatSkill.Instance[it])
                        .Select(it => it.Name.SetGradeColor(it.Grade))
                        .Join(null, "<space=1em>");
                    sb.AppendLine("擅长功法：" + str + " 等");
                }
            }

            if (otherData.TryGetValue("AvailableItems", out var value2))
            {
                var list = (List<object>)value2;
                var typeList = (List<object>)otherData["AvailableItemTypes"];
                Debug.Log("t4");
                if (list.Count > 0)
                {
                    var ids = list.Select(Convert.ToInt16).ToList();
                    var types = typeList.Select(Convert.ToInt16).ToList();
                    var names = new List<string>(ids.Count);
                    names.AddRange(ids.Select((t, i) => ItemTemplateHelper.GetName((sbyte)types[i], t)
                        .SetGradeColor(ItemTemplateHelper.GetGrade((sbyte)types[i], t))));

                    // var types = 
                    // .Select(it => SkillBook.Instance[it])
                    // .Select(it => it.Name.SetGradeColor(it.Grade))
                    // .Join(null, " ");
                    // ItemTemplateHelper.GetName()
                    sb.AppendLine("持有物品：" + names.Join(null, "<space=1em>") + " 等");
                }
            }
        }

        if (!titleText.isActiveAndEnabled || !contentText.isActiveAndEnabled) return;
        titleText.text = title;
        contentText.text = sb.ToString().ColorReplace();
        contentText.GetComponent<TMPTextSpriteHelper>().Parse();
        //
        // foreach (var skillBookItem in SkillBook.Instance)
        // {
        //     Debug.Log($"{skillBookItem.TemplateId}: {skillBookItem.Name}");
        // }
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