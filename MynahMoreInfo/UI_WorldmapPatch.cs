using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Config;
using FrameWork;
using GameData.Domains.Character.Display;
using GameData.Domains.Map;
using GameData.Serializer;
using HarmonyLib;
using UnityEngine;

namespace MynahMoreInfo;

/// <summary>
/// 从官方mod拿来的
/// </summary>
[HarmonyPatch]
public class UI_WorldmapPatch
{
    private static List<NameRelatedData> _nameRelatedDataList = new List<NameRelatedData>();

    [HarmonyPostfix, HarmonyPatch(typeof(UI_Worldmap), "OnEnable")]
    public static void Postfix(
        UI_Worldmap __instance,
        WorldMapModel ____mapModel,
        HashSet<Location> ____mapBlockSet)
    {
        if (!ModEntry.MapBlockMouseTip) return;

        var _OnMapBlockPointEnter = __instance.MapClickReceiver.OnMapBlockPointEnter;

        __instance.MapClickReceiver.OnMapBlockPointEnter = (x, y) =>
        {
            if (!ModEntry.MapBlockMouseTip)
            {
                _OnMapBlockPointEnter(x, y);
                return;
            }

            var altClicked = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
            if (ModEntry.MapBlockMouseTipByAlt && !altClicked)
            {
                __instance.MapClickReceiver.OnMapBlockPointExit(x, y);
                return;
            }

            __instance.MapClickReceiver.ScaleListening = true;
            var currMouseObj = (GameObject)typeof(MouseTipManager).GetField("_currMouseOverObj", (BindingFlags)(-1))!
                .GetValue(SingletonObject.getInstance<MouseTipManager>());

            if (currMouseObj == null) return;

            if (currMouseObj != __instance.gameObject && !currMouseObj.transform.IsChildOf(__instance.transform))
            {
                return;
            }

            var blockData = FindBlockByLogicalPosition(x, y);
            var mouseTips = __instance.MapClickReceiver.TipDisplayer;
            if (blockData == null || blockData.AreaId != ____mapModel.CurrentAreaId || !blockData.Visible)
            {
                __instance.MapClickReceiver.OnMapBlockPointExit(x, y);
                return;
            }

            var mapBlockItem = MapBlock.Instance[blockData.TemplateId];
            mouseTips.enabled = true;
            var argBox = new ArgumentBox();
            argBox.Clear();
            argBox.Set("arg0", mapBlockItem.Name);
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(mapBlockItem.Desc);

            if (blockData.IsCityTown())
            {
                _OnMapBlockPointEnter(x, y);
                return;
            }

            var names = new[] { "食物", "木材", "金铁", "玉石", "织物", "药材" };
            var colors = new[] { "#adcb84", "#c68639", "#81b1c0", "#52c3ad", "#c66963", "#6bb963" };
            for (var i = 0; i < 6; i++)
            {
                var curr = blockData.CurrResources.Get(i);
                var max = blockData.MaxResources.Get(i);
                if (curr >= 100)
                {
                    stringBuilder.Append($"<color={colors[i]}>{names[i]}:{curr}/{max}</color>");
                }
                else
                {
                    stringBuilder.Append($"{names[i]}:{curr}/{max}</color>");
                }

                if (i % 2 == 1) stringBuilder.AppendLine();
                else stringBuilder.Append("<pos=30%>");
            }

            stringBuilder.AppendLine();

            if (ModEntry.ShowPosAndId)
            {
                stringBuilder.AppendLine($"世界坐标(AreaId, BlockId): ({blockData.AreaId},{blockData.BlockId})");
                stringBuilder.AppendLine($"区域坐标(x, y): ({x},{y})");
            }
            
            if (blockData.CharacterSet != null || blockData.InfectedCharacterSet != null)
            {
                stringBuilder.AppendLine("地块人物:");
                var blockCharList = new List<int>();
                if (blockData.CharacterSet != null)
                    blockCharList.AddRange(blockData.CharacterSet);
                if (blockData.InfectedCharacterSet != null)
                    blockCharList.AddRange(blockData.InfectedCharacterSet);
                __instance.AsynchMethodCall(4, 7, blockCharList, (offset, dataPool) =>
                {
                    Serializer.Deserialize(dataPool, offset, ref _nameRelatedDataList);
                    _nameRelatedDataList.Sort((a, b) => b.OrgGrade - a.OrgGrade);
                    for (var index = 0; index < _nameRelatedDataList.Count; ++index)
                    {
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

                    _nameRelatedDataList.Clear();
                    blockCharList.Clear();
                    argBox.Set("arg1", stringBuilder.ToString());
                    mouseTips.RuntimeParam = argBox;
                    mouseTips.enabled = true;
                    mouseTips.ShowTips();
                });
            }
            else
            {
                argBox.Set("arg1", stringBuilder.ToString());
                mouseTips.RuntimeParam = argBox;
                mouseTips.enabled = true;
                mouseTips.Refresh();
                mouseTips.ShowTips();
            }
        };

        MapBlockData FindBlockByLogicalPosition(int x, int y)
        {
            if (____mapModel == null || ____mapBlockSet == null)
                return null;
            var showingAreaId = ____mapModel.ShowingAreaId;
            foreach (var mapBlock in ____mapBlockSet)
            {
                if (mapBlock.AreaId == showingAreaId)
                {
                    var blockData = ____mapModel.GetBlockData(mapBlock);
                    var blockPos = blockData.GetBlockPos();
                    if (x == blockPos.X && y == blockPos.Y)
                        return blockData;
                }
            }

            return null;
        }
    }
}