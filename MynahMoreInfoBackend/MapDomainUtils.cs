using Config;
using GameData.Domains;
using GameData.Domains.Map;

namespace MynahMoreInfoBackend;

public class MapDomainUtils
{
    public static string GetBlockFullName(
        Location location,
        string destroyedStr,
        string stockadeStr,
        bool showState)
    {
        if (!location.IsValid())
            return string.Empty;
        var mapStateItem = MapState.Instance[DomainManager.Map.GetStateTemplateIdByAreaId(location.AreaId)];
        var elementAreas = DomainManager.Map.GetElement_Areas(location.AreaId);
        var block1 = DomainManager.Map.GetBlock(location);
        var nameIndex = GetNameIndex(block1);
        var rootBlock = block1.GetRootBlock();
        var blockName = GetBlockName(rootBlock, nameIndex, stockadeStr);
        if (!rootBlock.Destroyed)
            destroyedStr = string.Empty;
        var str = string.Empty;
        if (rootBlock.BelongBlockId >= 0)
        {
            var block2 = GetBlock(rootBlock.AreaId, rootBlock.BelongBlockId);
            if (block2 != null)
                str = "-" + GetBlockName(block2, nameIndex, stockadeStr);
        }

        var name1 = mapStateItem.Name;
        var name2 = elementAreas.GetConfig().Name;
        string blockFullName;
        if (!showState)
            blockFullName = name2 + str + "-" + blockName + destroyedStr;
        else
            blockFullName = name1 + "-" + name2 + str + "-" + blockName + destroyedStr;
        return blockFullName;
    }

    private static string GetBlockName(MapBlockData block, int nameIndex, string stockadeStr)
    {
        var config = block.GetConfig();
        var elementAreas = DomainManager.Map.GetElement_Areas(block.AreaId);
        if (block.AreaId == 138 && DomainManager.Map.GetBlock(block.GetLocation()).TemplateId == 36)
            return stockadeStr;
        if (config.SubType == EMapBlockSubType.SwordTomb)
            return nameIndex < 0 || nameIndex >= config.BlockNames.Length ? config.Name : config.BlockNames[nameIndex];
        var index = 0;
        for (var length = elementAreas.SettlementInfos.Length; index < length; ++index)
        {
            if (block.BlockId == elementAreas.SettlementInfos[index].BlockId)
            {
                var settlementInfo = elementAreas.SettlementInfos[index];
                if (settlementInfo.SettlementId >= 0 && settlementInfo.RandomNameId >= 0)
                    return LocalTownNames.Instance.TownNameCore[settlementInfo.RandomNameId].Name;
            }
        }

        return nameIndex < 0 || nameIndex >= config.BlockNames.Length ? config.Name : config.BlockNames[nameIndex];
    }

    private static int GetNameIndex(MapBlockData block)
    {
        var config = block.GetConfig();
        var nameIndex = -1;
        if (config.Size > 1)
        {
            var areaSize = DomainManager.Map.GetAreaSize(block.AreaId);
            if (block.RootBlockId < 0)
            {
                nameIndex = config.Size == 2 ? 2 : 6;
            }
            else
            {
                var num1 = block.BlockId - block.RootBlockId;
                if (config.Size == 2)
                {
                    nameIndex = num1 == 1 ? 3 : (num1 == areaSize ? 0 : 1);
                }
                else
                {
                    int num2;
                    switch (num1)
                    {
                        case 1:
                            num2 = 7;
                            break;
                        case 2:
                            num2 = 8;
                            break;
                        default:
                            num2 = num1 == areaSize
                                ? 3
                                : (num1 == areaSize + 1
                                    ? 4
                                    : (num1 == areaSize + 2
                                        ? 5
                                        : (num1 == areaSize * 2 ? 0 : (num1 == areaSize * 2 + 1 ? 1 : 2))));
                            break;
                    }

                    nameIndex = num2;
                }
            }
        }

        return nameIndex;
    }

    public static MapBlockData GetBlock(short areaId, short blockId)
    {
        return DomainManager.Map.GetBlock(new Location(areaId, blockId));
    }
}