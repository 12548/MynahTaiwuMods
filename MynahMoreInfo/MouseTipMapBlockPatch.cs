using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Config;
using FrameWork;
using GameData.Domains;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Map;
using GameData.Serializer;
using HarmonyLib;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MynahMoreInfo;

[HarmonyPatch(typeof(Game.Views.MouseTips.MouseTipMapBlock), "Refresh")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class MouseTipMapBlockPatch
{
    public static void Postfix(Game.Views.MouseTips.MouseTipMapBlock __instance, ArgumentBox argsBox)
    {
        Debug.Log("MouseTipMapBlockPatch Postfix");
        argsBox.Get("MapBlockData", out MapBlockData blockData);

        if (ModEntry.ShowPosAndId)
        {
            var pos = blockData.GetBlockPos();
            var str =
                $"\n世界坐标(AreaId, BlockId): ({blockData.AreaId},{blockData.BlockId})\n区域坐标(x, y): ({pos.X},{pos.Y})";

            var mapBlockItem = MapBlock.Instance[blockData.TemplateId];
            __instance.transform.Find("DescLayout/Desc").GetComponent<TextMeshProUGUI>().text = (mapBlockItem.Desc + str).ColorReplace();
        }

        // if (ModEntry.MapBlockMouseTipHighlightResourceNumber > 0 && !blockData.IsCityTown())
        // {
        //     var names = new[] { "Food", "Wood", "Stone", "Jade", "Silk", "Herbal" };
        //     var colors = new[] { "#adcb84", "#c68639", "#81b1c0", "#52c3ad", "#c66963", "#6bb963" };
        //     for (var i = 0; i < 6; i++)
        //     {
        //         var text = __instance.transform.Find($"ResourceLayout/ResourceHolder/{names[i]}/ValueBack/Current");
        //         if (text == null) continue;
        //
        //         var curr = blockData.CurrResources.Get(i);
        //         var max = blockData.MaxResources.Get(i);
        //
        //         text.GetComponent<TextMeshProUGUI>().text = curr >= ModEntry.MapBlockMouseTipHighlightResourceNumber
        //             ? $"<color={colors[i]}>{curr}/{max}</color>"
        //             : $"{curr}/{max}</color>";
        //     }
        // }

        if (!ModEntry.MapBlockMouseTipCharList) return;

        Refers adventureLayout = __instance.CGet<Refers>("TreasureLayout");
        var blockCharList = new List<int>();
        if (blockData.CharacterSet != null)
            blockCharList.AddRange(blockData.CharacterSet);
        if (blockData.InfectedCharacterSet != null)
            blockCharList.AddRange(blockData.InfectedCharacterSet);
        Debug.Log("blockCharCount: " + blockCharList.Count);
        if (blockCharList.Count == 0)
        {
            GetCharListLayout(__instance, adventureLayout).gameObject.SetActive(false);
            
            return;
        }

        __instance.AsyncMethodCall(DomainHelper.DomainIds.Character,
            CharacterDomainHelper.MethodName2MethodId["GetNameRelatedDataList"], blockCharList,
            (offset, pool) =>
            {
                List<NameRelatedData> _nameRelatedDataList = new();
                Serializer.Deserialize(pool, offset, ref _nameRelatedDataList);
                _nameRelatedDataList.Sort((a, b) => b.OrgGrade - a.OrgGrade);
                var stringBuilder = new StringBuilder();
                for (var index = 0; index < _nameRelatedDataList.Count; ++index)
                {
                    if (index >= blockCharList.Count) break;
                    var num = blockCharList[index];
                    var nameRelatedData = _nameRelatedDataList[index];
                    var displayName =
                        NameCenter.GetMonasticTitleOrDisplayName(ref nameRelatedData, false);
                    var realName = NameCenter.GetRealName(ref nameRelatedData);
                    var gradeColor = Colors.Instance.GradeColors[nameRelatedData.OrgGrade];
                    stringBuilder.Append(displayName == realName
                        ? $"{displayName.SetColor(gradeColor)}"
                        : $"{displayName.SetColor(gradeColor)}/{realName.SetColor(gradeColor)}");

                    if (ModEntry.ShowPosAndId) stringBuilder.Append($"({num})");

                    // stringBuilder.AppendLine();
                    if (index % 2 == 1) stringBuilder.AppendLine();
                    else stringBuilder.Append("\t");

                    if (index >= 11)
                    {
                        if (_nameRelatedDataList.Count - index > 1)
                        {
                            stringBuilder.AppendLine($"(还有{_nameRelatedDataList.Count - index - 1}人未显示)");
                        }

                        break;
                    }
                }

                try
                {
                    if (__instance.isActiveAndEnabled)
                    {
                        var charListLayout = GetCharListLayout(__instance, adventureLayout);

                        var subTitle = charListLayout.Find("SubtitleLayout/SubTitle");
                        subTitle.GetComponent<TextMeshProUGUI>().text = "人物列表";
                        var content = charListLayout.Find("DescLayout/Desc");
                        // content.GetComponent<TextMeshProUGUI>().text = stringBuilder.ToString();
                        MouseTip_Util.SetMultiLineAutoHeightText(content.GetComponent<TextMeshProUGUI>(),
                            stringBuilder.ToString());

                        charListLayout.gameObject.SetActive(true);
                    }
                }
                catch (Exception _)
                {
                    // ignored
                }
            });
    }

    private static Transform GetCharListLayout(Game.Views.MouseTips.MouseTipMapBlock mouseTipInstance, Refers adventureLayout)
    {
        var charListLayout = mouseTipInstance.transform.Find("charListLayout");

        if (charListLayout == null)
        {
            var obj = Object.Instantiate(adventureLayout.gameObject, mouseTipInstance.transform, false);
            obj.name = "charListLayout";
            charListLayout = obj.transform;
        }

        return charListLayout;
    } 
}