using System.Collections.Generic;
using UnityEngine;

namespace ItemSubtypeFilter;

public static class SecondFilterHelper
{
    public static readonly Dictionary<ItemSortAndFilterType, ExtraFilterPlace> SecondFilterPlaces =
        new()
        {
            {
                ItemSortAndFilterType.Shop,
                new ExtraFilterPlace(
                    new Vector3(-1.46f, 2.35f, 240f),
                    new Vector2(0, -45),
                    new Vector3(-4.4f, 2.8f, 240f),
                    extraFilterPos: new Vector3(-2.8f, 2.62f, 240f))
            },
            {
                ItemSortAndFilterType.ShopInventory,
                new ExtraFilterPlace(
                    new Vector3(5.19f, 2.35f, 240f),
                    new Vector2(0, -45),
                    new Vector3(2.25f, 2.8f, 240f),
                    extraFilterPos:new Vector3(3.85f, 2.62f, 240f))
            },
            {
                ItemSortAndFilterType.Warehouse,
                new ExtraFilterPlace(
                    new Vector3(-4.02f, 1.39f, 240f),
                    new Vector2(0, -90),
                    new Vector3(-6.65f, 1.8f, 240f),
                    extraFilterPos: new Vector3(-5.25f, 1.8f, 240f)
                )
            },
            {
                ItemSortAndFilterType.WarehouseInventory,
                new ExtraFilterPlace(
                    new Vector3(4.13f, 1.39f, 240f),
                    new Vector2(0, -90),
                    new Vector3(1.5f, 1.8f, 240f),
                    extraFilterPos: new Vector3(2.8f, 1.8f, 240f)
                )
            },
            {
                ItemSortAndFilterType.CharacterMenuItems,
                new ExtraFilterPlace(
                    new Vector3(0.48f, 2.80f, 240f),
                    new Vector2(10f, -158f),
                    new Vector3(-3.13f, 3.20f, 240.0f),
                    extraFilterPos: new Vector3(-1.53f, 3.20f, 240.0f))
            },
            {
                ItemSortAndFilterType.Event,
                new ExtraFilterPlace(
                    new Vector3(-0.17f, -0.63f, 240f),
                    new Vector2(-20f, -125f),
                    new Vector3(-3.8f, -0.22f, 240f),
                    extraFilterPos: new Vector3(-2.2f, -0.22f, 240f)
                )
            },

            {
                ItemSortAndFilterType.TeaHorse,
                new ExtraFilterPlace(
                    new Vector3(3.61f, -1.19f, 240f),
                    new Vector2(988f, 250f),
                    new Vector3(0.24f, -0.78f, 240f)
                )
            },
            {
                ItemSortAndFilterType.ExchangeBookRight,
                new ExtraFilterPlace(
                    new Vector3(3.93f, 1.92f, 240f),
                    new Vector2(848f, 400f),
                    new Vector3(0.9f, 2.37f, 240f),
                    extraFilterPos: new Vector3(2.5f, 2.2f, 240f)
                )
            },
            {
                ItemSortAndFilterType.ExchangeBookLeft,
                new ExtraFilterPlace(
                    new Vector3(-3.97f, 1.92f, 240f),
                    new Vector2(848f, 400f),
                    new Vector3(-6.96f, 2.37f, 240f),
                    extraFilterPos: new Vector3(-5.36f, 2.2f, 240f)
                )
            },
        };

    public static readonly Dictionary<ItemSortAndFilter.ItemFilterType, Dictionary<int, string>>
        ItemFilterTypeToSubTypeString =
            new Dictionary<ItemSortAndFilter.ItemFilterType, Dictionary<int, string>>
            {
                {
                    ItemSortAndFilter.ItemFilterType.Food, new Dictionary<int, string>
                    {
                        { 700, "素" },
                        { 701, "荤" },
                        { 900, "茶" },
                        { 901, "酒" },
                        // { 0, "针" },
                        // { 1, "刺" },
                        // { 2, "暗" },
                        // { 3, "笛" },
                        // { 4, "掌" },
                        // { 5, "杵" },
                        // { 6, "拂" },
                        // { 7, "鞭" },
                        // { 8, "剑" },
                        // { 9, "刀" },
                        // { 10, "长" },
                        // { 11, "琴" },
                        // { 12, "机" },
                        // { 13, "符" },
                        // { 14, "霜" },
                        // { 15, "砂" },
                        // { 16, "神" },
                        // { 16, "兽" },
                    }
                },
                {
                    ItemSortAndFilter.ItemFilterType.Book, new Dictionary<int, string>
                    {
                        { 1001, "功" },
                        { 1000, "技" },
                    }
                },
                {
                    ItemSortAndFilter.ItemFilterType.Material, new Dictionary<int, string>
                    {
                        { 500, "食" },
                        { 501, "木" },
                        { 502, "金" },
                        { 503, "石" },
                        { 504, "织" },
                        { 505, "药" },
                        { 506, "毒" },
                    }
                },
                {
                    ItemSortAndFilter.ItemFilterType.Medicine, new Dictionary<int, string>
                    {
                        { 800, "丹" },
                        { 801, "毒" },
                    }
                },
                {
                    ItemSortAndFilter.ItemFilterType.Other, new Dictionary<int, string>
                    {
                        { 1100, "蛐" },
                        { 1200, "杂" },
                        { 1201, "罐" },
                        { 1202, "典" },
                        { 1203, "宝" },
                        { 1204, "虫" },
                        { 1205, "心" },
                        { 1206, "绳" },
                        { 1207, "奖" },
                    }
                },
            };

    public static ItemSortAndFilterType? GetItemSortAndFilterType(ItemSortAndFilter sortAndFilter)
    {
        var parentTransform = sortAndFilter.transform.parent;
        if (parentTransform.parent.parent.parent.name == "UI_CharacterMenuItems")
        {
            return ItemSortAndFilterType.CharacterMenuItems;
        }

        // 茶马帮
        // UI_TeaHorseCaravan/MainWindow/SelfPanelHolder/WarehouseItemsHolder/Warehouse/WarehouseItemScroll/ItemSortAndFilter
        if (parentTransform.parent.parent.parent.name == "SelfPanelHolder")
        {
            return ItemSortAndFilterType.TeaHorse;
        }

        // 多选物品： UI_MultiSelectItem/PopupWindowBase/ElementsRoot/InventoryItemScroll
        // 仓库： UI_Warehouse/MainWindow/Warehouse/ScrollBack/WarehouseItemScroll

        if (parentTransform.parent.parent.parent.parent.name == "UI_Warehouse")
        {
            switch (parentTransform.name)
            {
                case "InventoryItemScroll":
                    return ItemSortAndFilterType.WarehouseInventory;
                case "WarehouseItemScroll":
                    return ItemSortAndFilterType.Warehouse;
            }
        }

        return parentTransform.parent.name switch
        {
            "ShopItems" => ItemSortAndFilterType.Shop,
            "SelfItems" => ItemSortAndFilterType.ShopInventory,
            "OperateArea" => ItemSortAndFilterType.Event,
            "TaiwuBooks" => ItemSortAndFilterType.ExchangeBookRight,
            "NpcBooks" => ItemSortAndFilterType.ExchangeBookLeft,
            _ => null
        };
    }
}