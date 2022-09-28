using System.Collections.Generic;

namespace ItemSubtypeFilter;

public static class Utils
{
    public static Dictionary<ItemSortAndFilter.ItemFilterType, Dictionary<int, string>> ItemFilterTypeToSubTypeString =
        new Dictionary<ItemSortAndFilter.ItemFilterType, Dictionary<int, string>>()
        {
            {
                ItemSortAndFilter.ItemFilterType.Food, new Dictionary<int, string>()
                {
                    {700, "素"},
                    {701, "荤"},
                    {900, "茶"},
                    {901, "酒"},
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
                ItemSortAndFilter.ItemFilterType.Book, new Dictionary<int, string>()
                {
                    {1001, "功"},
                    {1000, "技"},
                }
            },
            {
                ItemSortAndFilter.ItemFilterType.Material, new Dictionary<int, string>()
                {
                    {500, "食"},
                    {501, "木"},
                    {502, "金"},
                    {503, "石"},
                    {504, "织"},
                    {505, "药"},
                    {506, "毒"},
                }
            },
            {
                ItemSortAndFilter.ItemFilterType.Medicine, new Dictionary<int, string>()
                {
                    {800, "丹"},
                    {801, "毒"},
                }
            },
            {
                ItemSortAndFilter.ItemFilterType.Other, new Dictionary<int, string>()
                {
                    {1100, "蛐"},
                    {1200, "杂"},
                    {1201, "罐"},
                    {1202, "典"},
                    {1203, "宝"},
                    {1204, "虫"},
                    {1205, "心"},
                    {1206, "绳"},
                    {1207, "奖"},
                }
            },
        };
}