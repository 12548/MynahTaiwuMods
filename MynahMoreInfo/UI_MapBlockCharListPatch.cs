using System.Collections.Generic;
using System.Text;
using Config;
using FrameWork;
using GameData.Domains;
using GameData.Domains.Character;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using GameData.Serializer;
using HarmonyLib;
using UnityEngine;
using LifeSkillType = Config.LifeSkillType;

namespace MynahMoreInfo;

[HarmonyPatch]
public class UI_MapBlockCharListPatch
{
    [HarmonyPatch(typeof(UI_MapBlockCharList), "OnRenderChar")]
    [HarmonyPostfix]
    static void OnRenderCharPostfix(int index, Refers charRefers, bool ____canSeeDetail, CToggleGroup ____togGroup,
        List<int> ____infectedList, List<int> ____normalCharList,
        Dictionary<int, CharacterDisplayData> ____charDataDict, UI_MapBlockCharList __instance)
    {
        if (ModEntry.MouseTipMapBlockCharList == 0) return;
        var trigger = ____canSeeDetail;
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

            switch (ModEntry.MouseTipMapBlockCharList)
            {
                case 2:
                    GetAlternateCharTipStr(__instance, mouseTipDisplayer, charDisplayData);
                    break;
                case 1:
                    Util.EnableMouseTipCharacter(mouseTipDisplayer, characterId);
                    break;
            }
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

    static void GetAlternateCharTipStr(UIBase ui, MouseTipDisplayer displayer, CharacterDisplayData displayData)
    {
        ui.AsynchMethodCall(DomainHelper.DomainIds.Character,
            CharacterDomainHelper.MethodIds.GetGroupCharDisplayDataList, new List<int> { displayData.CharacterId },
            (offset, dataPool) =>
            {
                var item = EasyPool.Get<List<GroupCharDisplayData>>();
                Serializer.Deserialize(dataPool, offset, ref item);
                var groupCharDisplayData = item[0];
                EasyPool.Free(item);

                if (groupCharDisplayData == null) return;

                var isTaiwu = displayData.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
                var showName = NameCenter.GetCharMonasticTitleOrNameByDisplayData(displayData, isTaiwu);
                var realName = NameCenter.GetNameByDisplayData(displayData, isTaiwu, true);
                // var color = displayData.AliveState == 0 ? "white" : "red";

                WorldMapModel worldMapModel = SingletonObject.getInstance<WorldMapModel>();

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

                displayer.Type = TipType.SimpleWide;
                displayer.RuntimeParam = new ArgumentBox();
                displayer.RuntimeParam.Set("arg0", title);

                // bool isFixedCharacter =
                //     CreatingType.IsFixedPresetType(Character.Instance.GetItem(displayData.TemplateId)
                //         .CreatingType);

                var sb = new StringBuilder();

                var healthInfo =
                    CommonUtils.GetCharacterHealthInfo(groupCharDisplayData.Health, groupCharDisplayData.MaxLeftHealth);
                sb.Append("健康：" + healthInfo.Item1 +
                          $"({groupCharDisplayData.Health}/{groupCharDisplayData.MaxLeftHealth})");

                sb.Append("<pos=33%>魅力：" + CommonUtils.GetCharmLevelText(
                    groupCharDisplayData.Charm,
                    displayData.Gender,
                    displayData.AvatarRelatedData.DisplayAge,
                    displayData.AvatarRelatedData.ClothingDisplayId,
                    false, displayData.AvatarRelatedData.AvatarData.FaceVisible) + $"({groupCharDisplayData.Charm})");
                var happinessType = HappinessType.GetHappinessType(groupCharDisplayData.Happiness);
                sb.Append("<pos=66%>心情：" + CommonUtils.GetHappinessString(happinessType) +
                          $"({groupCharDisplayData.Happiness})");

                sb.AppendLine();

                sb.Append("好感：" + CommonUtils.GetFavorString(groupCharDisplayData.FavorabilityToTaiwu) +
                          $"({groupCharDisplayData.FavorabilityToTaiwu})");

                var fameType = FameType.GetFameType(groupCharDisplayData.Fame);
                sb.Append("<pos=33%>声望：" + CommonUtils.GetFameString(fameType) + $"({groupCharDisplayData.Fame})");

                sb.AppendLine("<pos=66%>轮回：" + groupCharDisplayData.PreexistenceCharCount);

                var attackMedal = groupCharDisplayData.AttackMedal;
                var amSprite = attackMedal > 0
                    ? "#72D4C8"
                    : (attackMedal < 0 ? "#BA5B3C" : "#A3A2A3");

                sb.Append($"进攻：<color={amSprite}>{Mathf.Abs(attackMedal)}</color>");

                var defenceMedal = groupCharDisplayData.DefenceMedal;
                var dmSprite = defenceMedal > 0
                    ? "#72D4C8"
                    : (defenceMedal < 0 ? "#BA5B3C" : "#A3A2A3");

                sb.Append($"   守御：<color={dmSprite}>{Mathf.Abs(defenceMedal)}</color>");

                var wisdomMedal = groupCharDisplayData.WisdomMedal;
                var wmSprite = groupCharDisplayData.WisdomMedal > 0
                    ? "#72D4C8"
                    : (groupCharDisplayData.WisdomMedal < 0 ? "#BA5B3C" : "#A3A2A3");

                sb.Append($"   机略：<color={wmSprite}>{Mathf.Abs(wisdomMedal)}</color>");

                int birthMonth = displayData.BirthDate % 12;
                if (birthMonth < 0)
                    birthMonth += 12;

                MonthItem monthItem = Month.Instance[birthMonth];

                var fiveElements = monthItem.FiveElementsTypeDesc
                    .SetColor(Colors.Instance.FiveElementsColors[monthItem.FiveElementsType]);

                sb.AppendLine(
                    $"    生于{monthItem.Name}({fiveElements})");

                sb.AppendLine();

                AgeEffectItem ageEffectItem =
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

                displayer.RuntimeParam.Set("arg1", sb.ToString());
                // var cal = __instance.transform.Find("CharAttributeList");
                // var gameObject = new GameObject();
                // var tmp = gameObject.AddComponent<TextMeshProUGUI>();
                // tmp.text = "最高功法资质：" + CombatSkillType.Instance[highCombatSkills[0]].Name +
                //            _getCSQValue(groupCharDisplayData, highCombatSkills[0]);

                // GameObject.Instantiate(gameObject, cal, false);
                // groupCharDisplayData.CombatSkillQualifications
            });
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