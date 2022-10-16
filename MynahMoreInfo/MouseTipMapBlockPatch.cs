using System.Collections.Generic;
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

namespace MynahMoreInfo;

[HarmonyPatch(typeof(MouseTipMapBlock), "Init")]
public class MouseTipMapBlockPatch
{
    public static void Postfix(MouseTipMapBlock __instance, ArgumentBox argsBox)
    {
        argsBox.Get("MapBlockData", out MapBlockData blockData);
        
        if (ModEntry.ShowPosAndId)
        {
            var pos = blockData.GetBlockPos();
            var str = $"\n世界坐标(AreaId, BlockId): ({blockData.AreaId},{blockData.BlockId})\n区域坐标(x, y): ({pos.X},{pos.Y})";
         
            var mapBlockItem = MapBlock.Instance[blockData.TemplateId];
            __instance.CGet<TextMeshProUGUI>("Desc").text = (mapBlockItem.Desc + str).ColorReplace();
        }

        if (ModEntry.MapBlockMouseTipHighlightResource && !blockData.IsCityTown())
        {
            var names = new[] { "Food", "Wood", "Stone", "Jade", "Silk", "Herbal" };
            var colors = new[] { "#adcb84", "#c68639", "#81b1c0", "#52c3ad", "#c66963", "#6bb963" };
            for (var i = 0; i < 6; i++)
            {
                var text = __instance.transform.Find($"ResourceLayout/ResourceHolder/{names[i]}/ValueBack/Current");
                if(text == null) continue;

                var curr = blockData.CurrResources.Get(i);
                var max = blockData.MaxResources.Get(i);

                text.GetComponent<TextMeshProUGUI>().text = curr > 100
                    ? $"<color={colors[i]}>{curr}/{max}</color>"
                    : $"{curr}/{max}</color>";
            }
        }

        if (!ModEntry.MapBlockMouseTipCharList) return;

        Refers adventureLayout = __instance.CGet<Refers>("AdventureLayout");
        var blockCharList = new List<int>();
        if (blockData.CharacterSet != null)
            blockCharList.AddRange(blockData.CharacterSet);
        if (blockData.InfectedCharacterSet != null)
            blockCharList.AddRange(blockData.InfectedCharacterSet);
        if (blockCharList.Count == 0)
        {
            GetCharListLayout(__instance, adventureLayout).gameObject.SetActive(false);
            return;
        }

        __instance.AsynchMethodCall(DomainHelper.DomainIds.Character,
            CharacterDomainHelper.MethodIds.GetNameRelatedDataList, blockCharList,
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
                    var byNameRelatedData =
                        NameCenter.GetCharMonasticTitleAndNameByNameRelatedData(ref nameRelatedData, false, false);
                    var name = NameCenter.GetName(ref nameRelatedData, false, true);
                    var str = name.surname + name.givenName;
                    var gradeColor = Colors.Instance.GradeColors[nameRelatedData.OrgGrade];
                    if (byNameRelatedData == str)
                        stringBuilder.Append($"{byNameRelatedData.SetColor(gradeColor)}");
                    else
                        stringBuilder.Append(
                            $"{byNameRelatedData.SetColor(gradeColor)}/{str.SetColor(gradeColor)}");

                    if (ModEntry.ShowPosAndId) stringBuilder.Append($"({num})");

                    if (index % 2 == 1) stringBuilder.AppendLine();
                    else stringBuilder.Append("<pos=40%>");

                    if (index >= 11)
                    {
                        if (_nameRelatedDataList.Count - index > 1)
                        {
                            stringBuilder.AppendLine($"(还有{_nameRelatedDataList.Count - index - 1}人未显示)");
                        }

                        break;
                    }
                }

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
            });
    }

    private static Transform GetCharListLayout(MouseTipMapBlock mouseTipInstance, Refers adventureLayout)
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