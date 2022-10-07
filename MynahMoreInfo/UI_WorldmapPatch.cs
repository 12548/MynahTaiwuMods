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
        __instance.MapClickReceiver.OnMapBlockPointEnter += (x, y) =>
        {
            if (!ModEntry.MapBlockMouseTip) return;
            var currMouseObj = (GameObject)typeof(MouseTipManager).GetField("_currMouseOverObj", (BindingFlags)(-1))!
                .GetValue(SingletonObject.getInstance<MouseTipManager>());
            if (currMouseObj != __instance.gameObject && !currMouseObj.transform.IsChildOf(__instance.transform))
            {
                return;
            }

            MapBlockData blockData = FindBlockByLogicalPosition(x, y);
            MouseTipDisplayer mouseTips = __instance.MapClickReceiver.TipDisplayer;
            if (blockData == null || blockData.AreaId != ____mapModel.CurrentAreaId || !blockData.Visible)
                return;
            MapBlockItem mapBlockItem = MapBlock.Instance[blockData.TemplateId];
            mouseTips.enabled = true;
            ArgumentBox argBox = new ArgumentBox();
            argBox.Clear();
            argBox.Set("arg0", mapBlockItem.Name);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(mapBlockItem.Desc);

            if (!blockData.IsCityTown())
            {
                var names = new[] { "食物", "木材", "金铁", "玉石", "织物", "药材" };
                var colors = new[] { "#adcb84", "#c68639", "#81b1c0", "#52c3ad", "#c66963", "#6bb963" };
                for (int i = 0; i < 6; i++)
                {
                    var curr = blockData.CurrResources.Get(i);
                    var max = blockData.MaxResources.Get(i);
                    stringBuilder.Append($"<color={colors[i]}>{names[i]}:{curr}/{max}</color>   ");
                    if (i % 2 == 1) stringBuilder.AppendLine();
                }
            }

            stringBuilder.AppendLine();

            stringBuilder.AppendLine(string.Format("世界坐标(AreaId, BlockId): ({0},{1})", blockData.AreaId,
                blockData.BlockId));
            stringBuilder.AppendLine(string.Format("区域坐标(x, y): ({0},{1})", x, y));
            if (blockData.CharacterSet != null || blockData.InfectedCharacterSet != null)
            {
                stringBuilder.AppendLine("地块人物:");
                List<int> blockCharList = new List<int>();
                if (blockData.CharacterSet != null)
                    blockCharList.AddRange(blockData.CharacterSet);
                if (blockData.InfectedCharacterSet != null)
                    blockCharList.AddRange(blockData.InfectedCharacterSet);
                __instance.AsynchMethodCall(4, 7, blockCharList, (offset, dataPool) =>
                {
                    Serializer.Deserialize(dataPool, offset, ref _nameRelatedDataList);
                    for (int index = 0; index < _nameRelatedDataList.Count; ++index)
                    {
                        int num = blockCharList[index];
                        NameRelatedData nameRelatedData = _nameRelatedDataList[index];
                        string byNameRelatedData =
                            NameCenter.GetCharMonasticTitleAndNameByNameRelatedData(ref nameRelatedData, false, false);
                        (string surname, string givenName) name = NameCenter.GetName(ref nameRelatedData, false, true);
                        string str = name.surname + name.givenName;
                        Color gradeColor = Colors.Instance.GradeColors[nameRelatedData.OrgGrade];
                        if (byNameRelatedData == str)
                            stringBuilder.AppendLine(string.Format("\t{0}({1})", byNameRelatedData.SetColor(gradeColor),
                                num));
                        else
                            stringBuilder.AppendLine(string.Format("\t{0}/{1}({2})",
                                byNameRelatedData.SetColor(gradeColor), str.SetColor(gradeColor), num));
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
            short showingAreaId = ____mapModel.ShowingAreaId;
            foreach (Location mapBlock in ____mapBlockSet)
            {
                if (mapBlock.AreaId == showingAreaId)
                {
                    MapBlockData blockData = ____mapModel.GetBlockData(mapBlock);
                    ByteCoordinate blockPos = blockData.GetBlockPos();
                    if (x == blockPos.X && y == blockPos.Y)
                        return blockData;
                }
            }

            return null;
        }
    }
}