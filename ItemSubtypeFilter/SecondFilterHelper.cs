using System.Collections.Generic;
using UnityEngine;

namespace ItemSubtypeFilter;

public static class SecondFilterHelper
{
    public static readonly Dictionary<ItemSortAndFilterType, SecondFilterPlace> SecondFilterPlaces =
        new()
        {
            {
                ItemSortAndFilterType.Shop,
                new SecondFilterPlace(
                    new Vector3(-1.46f, 2.35f, 240f),
                    new Vector2(0, -45),
                    new Vector3(-4.4f, 2.8f, 240f))
            },
            {
                ItemSortAndFilterType.ShopInventory,
                new SecondFilterPlace(
                    new Vector3(5.19f, 2.35f, 240f),
                    new Vector2(0, -45),
                    new Vector3(2.25f, 2.8f, 240f))
            },
            {
                ItemSortAndFilterType.Warehouse,
                new SecondFilterPlace(
                    new Vector3(-4.02f, 2.02f, 240f),
                    new Vector2(0, -80),
                    new Vector3(-6.9f, 2.45f, 240f))
            },
            {
                ItemSortAndFilterType.WarehouseInventory,
                new SecondFilterPlace(
                    new Vector3(4.13f, 2.02f, 240f),
                    new Vector2(0, -80),
                    new Vector3(1.25f, 2.45f, 240f))
            },
            {
                ItemSortAndFilterType.CharacterMenuItems,
                new SecondFilterPlace(
                    new Vector3(0.48f, 2.80f, 240f),
                    new Vector2(10f, -158f),
                    new Vector3(-3.13f, 3.20f, 240.0f))
            },
            {
                ItemSortAndFilterType.Event,
                new SecondFilterPlace(
                    new Vector3(-0.17f, -0.63f, 240f),
                    new Vector2(-20f, -125f),
                    new Vector3(-3.8f, -0.22f, 240f)
                    )
            },
            
            {
                ItemSortAndFilterType.TeaHorse,
                new SecondFilterPlace(
                    new Vector3(3.61f, -1.19f, 240f),
                    new Vector2(988f, 250f),
                    new Vector3(0.24f, -0.78f, 240f)
                    )
            },
            {
                ItemSortAndFilterType.ExchangeBookRight,
                new SecondFilterPlace(
                    new Vector3(3.93f, 1.92f, 240f),
                    new Vector2(848f, 400f),
                    new Vector3(0.9f, 2.37f, 240f)
                    )
            },
            {
                ItemSortAndFilterType.ExchangeBookLeft,
                new SecondFilterPlace(
                    new Vector3(-3.97f, 1.92f, 240f),
                    new Vector2(848f, 400f),
                    new Vector3(-6.96f, 2.37f, 240f)
                    )
            },
        };

    public class SecondFilterPlace
    {
        public Vector3 NewViewportPos;
        public Vector2 NewViewportSize;
        public Vector3 SecondFilterPos;
        public Vector3? OriginalViewportPos;
        public Vector2? OriginalViewportSize;
        public bool DoubleLine;

        public SecondFilterPlace(Vector3 newViewportPos, Vector2 newViewportSize, Vector3 secondFilterPos,
            Vector3? originalViewportPos = null, Vector2? originalViewportSize = null, bool doubleLine = false)
        {
            NewViewportPos = newViewportPos;
            NewViewportSize = newViewportSize;
            SecondFilterPos = secondFilterPos;
            OriginalViewportPos = originalViewportPos;
            OriginalViewportSize = originalViewportSize;
            DoubleLine = doubleLine;
        }
    }

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

        switch (parentTransform.parent.name)
        {
            case "Inventory":
                return ItemSortAndFilterType.WarehouseInventory;
            case "Warehouse":
                return ItemSortAndFilterType.Warehouse;
            case "ShopItems":
                return ItemSortAndFilterType.Shop;
            case "SelfItems":
                return ItemSortAndFilterType.ShopInventory;
            case "OperateArea":
                return ItemSortAndFilterType.Event;
            case "TaiwuBooks":
                return ItemSortAndFilterType.ExchangeBookRight;
            case "NpcBooks":
                return ItemSortAndFilterType.ExchangeBookLeft;
            default:
                return null;
        }
    }
}