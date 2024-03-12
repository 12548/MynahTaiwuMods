using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace ItemSubtypeFilter;

public class ThirdFilterHelper
{
    private static readonly Dictionary<ItemSortAndFilterType, ExtraFilterPlace> ThirdFilterPlaces =
        new()
        {
            {
                ItemSortAndFilterType.CharacterMenuItems,
                new ExtraFilterPlace(
                    new Vector3(0.48f, 2.42f, 240f),
                    new Vector2(10, -214),
                    new Vector3(-3.4f, 2.79f, 240f))
            },
            {
                ItemSortAndFilterType.Event,
                new ExtraFilterPlace(
                    new Vector3(-0.17f, -1.03f, 240f),
                    new Vector2(-20f, -180f),
                    new Vector3(-3.8f, -0.65f, 240f)
                )
            },
            {
                ItemSortAndFilterType.TeaHorse,
                new ExtraFilterPlace(
                    new Vector3(3.61f, -1.6f, 240f),
                    new Vector2(988f, 190f),
                    new Vector3(0.24f, -1.2f, 240f)
                )
            },

            {
                ItemSortAndFilterType.ExchangeBookLeft,
                new ExtraFilterPlace(
                    new Vector3(-3.97f, 1.6f, 240f),
                    new Vector2(848f, 350f),
                    new Vector3(-5.5f, 2.2f, 240f),
                    doubleLine: true,
                    extraFilterPos: new Vector3(-6.96f, 1.83f, 240f)
                )
            },
            {
                ItemSortAndFilterType.ExchangeBookRight,
                new ExtraFilterPlace(
                    new Vector3(3.93f, 1.6f, 240f),
                    new Vector2(848f, 350f),
                    new Vector3(2.38f, 2.2f, 240f),
                    doubleLine: true,
                    extraFilterPos: new Vector3(0.9f, 1.83f, 240f)
                )
            },

            {
                ItemSortAndFilterType.Shop,
                new ExtraFilterPlace(
                    new Vector3(-1.5f, 2f, 240f),
                    new Vector2(848f, -105f),
                    new Vector3(-2.62f, 2.6f, 240f),
                    doubleLine: true,
                    extraFilterPos: new Vector3(-4.4f, 2.26f, 240f)
                )
            },
            {
                ItemSortAndFilterType.ShopInventory,
                new ExtraFilterPlace(
                    new Vector3(5.1f, 2f, 240f),
                    new Vector2(848f, -105f),
                    new Vector3(3.7f, 2.63f, 240f),
                    doubleLine: true,
                    extraFilterPos:new Vector3(2.25f, 2.26f, 240f)
                )
            },

            {
                ItemSortAndFilterType.Warehouse,
                new ExtraFilterPlace(
                    new Vector3(-4.02f, 1.02f, 240f),
                    new Vector2(0, -140),
                    new Vector3(-5.2f, 1.8f, 240f),
                    doubleLine: true,
                    extraFilterPos: new Vector3(-6.65f, 1.43f, 240f)
                )
            },
            {
                ItemSortAndFilterType.WarehouseInventory,
                new ExtraFilterPlace(
                    new Vector3(4.13f, 1.02f, 240f),
                    new Vector2(0, -140),
                    new Vector3(2.98f, 1.8f, 240f),
                    doubleLine: true,
                    extraFilterPos: new Vector3(1.5f, 1.43f, 240f)
                )
            },
        };

    public static List<ThirdFilterInfo> ThirdFilters = new()
    {
        new ThirdFilterInfo(ThirdFilterType.CombatSkillBook, 0, "内", 100),
        new ThirdFilterInfo(ThirdFilterType.CombatSkillBook, 1, "身", 101),
        new ThirdFilterInfo(ThirdFilterType.CombatSkillBook, 2, "绝", 102),
        new ThirdFilterInfo(ThirdFilterType.CombatSkillBook, 3, "拳", 103),
        new ThirdFilterInfo(ThirdFilterType.CombatSkillBook, 4, "指", 104),
        new ThirdFilterInfo(ThirdFilterType.CombatSkillBook, 5, "腿", 105),
        new ThirdFilterInfo(ThirdFilterType.CombatSkillBook, 6, "暗", 106),
        new ThirdFilterInfo(ThirdFilterType.CombatSkillBook, 7, "剑", 107),
        new ThirdFilterInfo(ThirdFilterType.CombatSkillBook, 8, "刀", 108),
        new ThirdFilterInfo(ThirdFilterType.CombatSkillBook, 9, "长", 109),
        new ThirdFilterInfo(ThirdFilterType.CombatSkillBook, 10, "奇", 110),
        new ThirdFilterInfo(ThirdFilterType.CombatSkillBook, 11, "软", 111),
        new ThirdFilterInfo(ThirdFilterType.CombatSkillBook, 12, "御", 112),
        new ThirdFilterInfo(ThirdFilterType.CombatSkillBook, 13, "乐", 113),

        new ThirdFilterInfo(ThirdFilterType.CombatSkillBook, 0, "无", 301),
        new ThirdFilterInfo(ThirdFilterType.CombatSkillBook, 1, "少", 302),
        new ThirdFilterInfo(ThirdFilterType.CombatSkillBook, 2, "峨", 303),
        new ThirdFilterInfo(ThirdFilterType.CombatSkillBook, 3, "百", 304),
        new ThirdFilterInfo(ThirdFilterType.CombatSkillBook, 4, "武", 305),
        new ThirdFilterInfo(ThirdFilterType.CombatSkillBook, 5, "元", 306),
        new ThirdFilterInfo(ThirdFilterType.CombatSkillBook, 6, "狮", 307),
        new ThirdFilterInfo(ThirdFilterType.CombatSkillBook, 7, "然", 308),
        new ThirdFilterInfo(ThirdFilterType.CombatSkillBook, 8, "璇", 309),
        new ThirdFilterInfo(ThirdFilterType.CombatSkillBook, 9, "铸", 310),
        new ThirdFilterInfo(ThirdFilterType.CombatSkillBook, 10, "空", 311),
        new ThirdFilterInfo(ThirdFilterType.CombatSkillBook, 11, "金", 312),
        new ThirdFilterInfo(ThirdFilterType.CombatSkillBook, 12, "五", 313),
        new ThirdFilterInfo(ThirdFilterType.CombatSkillBook, 13, "界", 314),
        new ThirdFilterInfo(ThirdFilterType.CombatSkillBook, 14, "伏", 315),
        new ThirdFilterInfo(ThirdFilterType.CombatSkillBook, 15, "血", 316),
        new ThirdFilterInfo(ThirdFilterType.CombatSkillBook, 16, "它", 317),

        new ThirdFilterInfo(ThirdFilterType.LifeSkillBook, 0, "音", 200),
        new ThirdFilterInfo(ThirdFilterType.LifeSkillBook, 1, "棋", 201),
        new ThirdFilterInfo(ThirdFilterType.LifeSkillBook, 2, "书", 202),
        new ThirdFilterInfo(ThirdFilterType.LifeSkillBook, 3, "画", 203),
        new ThirdFilterInfo(ThirdFilterType.LifeSkillBook, 4, "术", 204),
        new ThirdFilterInfo(ThirdFilterType.LifeSkillBook, 5, "品", 205),
        new ThirdFilterInfo(ThirdFilterType.LifeSkillBook, 6, "锻", 206),
        new ThirdFilterInfo(ThirdFilterType.LifeSkillBook, 7, "木", 207),
        new ThirdFilterInfo(ThirdFilterType.LifeSkillBook, 8, "医", 208),
        new ThirdFilterInfo(ThirdFilterType.LifeSkillBook, 9, "毒", 209),
        new ThirdFilterInfo(ThirdFilterType.LifeSkillBook, 10, "织", 210),
        new ThirdFilterInfo(ThirdFilterType.LifeSkillBook, 11, "巧", 211),
        new ThirdFilterInfo(ThirdFilterType.LifeSkillBook, 12, "道", 212),
        new ThirdFilterInfo(ThirdFilterType.LifeSkillBook, 13, "佛", 213),
        new ThirdFilterInfo(ThirdFilterType.LifeSkillBook, 14, "厨", 214),
        new ThirdFilterInfo(ThirdFilterType.LifeSkillBook, 15, "杂", 215),
    };

    public static CToggleGroup EnsureThirdFilter(ItemSortAndFilterType uiType, ItemSortAndFilter parentSortAndFilter,
        ThirdFilterType filterType)
    {
        ThirdFilterPlaces.TryGetValue(uiType, out var places);
        if (places == null) return null;

        var parentTrans = parentSortAndFilter.transform;
        var viewport = parentTrans.parent.GetComponent<CScrollRect>().Viewport;
        var filterObj = parentTrans.Find("ThirdFilter");

        if (filterObj == null)
        {
            filterObj = Object.Instantiate(parentTrans.Find("Filter"), parentTrans, false);
            filterObj.transform.name = "ThirdFilter";
            filterObj.position = places.SecondFilterPos;

            if (places.DoubleLine)
            {
                var hlg = filterObj.GetComponent<HorizontalLayoutGroup>();
                var spacing = hlg.spacing;
                Object.DestroyImmediate(hlg);
                var glg = filterObj.gameObject.AddComponent<GridLayoutGroup>();
                glg.cellSize = new Vector2(50f, 50f);
                glg.spacing = new Vector2(spacing, spacing);
                glg.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                glg.constraintCount = 9;
            }

            var toggleGroup = filterObj.GetComponent<CToggleGroup>();
            // var originalToggles = filterObj.GetComponentsInChildren<CToggle>();

            var template = filterObj.GetChild(0);

            foreach (var thirdFilterInfo in ThirdFilters)
            {
                var obj = Object.Instantiate(template, filterObj, false);
                var cToggle = obj.GetComponent<CToggle>();
                cToggle.Key = thirdFilterInfo.togKey;
                cToggle.isOn = false;
                obj.GetComponentInChildren<TextMeshProUGUI>().text = thirdFilterInfo.c;
                toggleGroup.Add(cToggle);
                obj.name = thirdFilterInfo.c;
                obj.gameObject.SetActive(false);
            }

            toggleGroup.InitPreOnToggle();

            toggleGroup.OnActiveToggleChange = (_, _) =>
            {
                var itemList = parentSortAndFilter._itemList;
                var filterTogGroup = parentSortAndFilter._filterTogGroup;
                var equipFilterTogGroup = parentSortAndFilter._equipFilterTogGroup;
                var onItemListChanged = parentSortAndFilter._onItemListChanged;

                Patch.UpdateItemListPrefix(parentSortAndFilter, filterTogGroup,
                    equipFilterTogGroup, itemList,
                    onItemListChanged);
            };
        }

        // 只显示适用的按钮
        for (int i = 0; i < filterObj.childCount; i++)
        {
            var toggle = filterObj.GetChild(i).GetComponent<CToggle>();
            if (toggle.Key == 0 || (filterType == ThirdFilterType.CombatSkillBook &&
                                    ((ModEntry.CombatSkillFilterType == 0 && toggle.Key is > 99 and < 200) ||
                                     (ModEntry.CombatSkillFilterType == 1 && toggle.Key is > 299 and < 400))) ||
                (filterType == ThirdFilterType.LifeSkillBook && toggle.Key is > 199 and < 300))
            {
                toggle.gameObject.SetActive(true);
            }
            else
            {
                // Debug.Log($"{toggle.gameObject.name} {toggle.Key} not active");
                toggle.gameObject.SetActive(false);
            }
        }

        filterObj.gameObject.SetActive(true);

        if (places.OriginalViewportPos == null || places.OriginalViewportSize == null)
        {
            places.OriginalViewportPos = viewport.position;
            places.OriginalViewportSize = viewport.sizeDelta;
        }

        filterObj.transform.position = places.SecondFilterPos;

        viewport.position = places.NewViewportPos;
        viewport.sizeDelta = places.NewViewportSize;

        var readingStateFilter = parentTrans.Find("ReadingStateFilter");
        if (places.ExtraFilterPos.HasValue && readingStateFilter != null)
        {
            readingStateFilter.position = places.ExtraFilterPos.Value;
        }

        return filterObj.GetComponent<CToggleGroup>();
    }
    
    public static CToggleGroup EnsureReadingStateFilter(ItemSortAndFilterType uiType, ItemSortAndFilter parentSortAndFilter)
    {
        SecondFilterHelper.SecondFilterPlaces.TryGetValue(uiType, out var places);
        if (places?.ExtraFilterPos == null) return null;

        var parentTrans = parentSortAndFilter.transform;
        var viewport = parentTrans.parent.GetComponent<CScrollRect>().Viewport;
        var filterObj = parentTrans.Find("ReadingStateFilter");

        if (filterObj == null)
        {
            filterObj = Object.Instantiate(parentTrans.Find("Filter"), parentTrans, false);
            filterObj.transform.name = "ReadingStateFilter";
            filterObj.position = places.ExtraFilterPos.Value;

            var toggleGroup = filterObj.GetComponent<CToggleGroup>();
            // var originalToggles = filterObj.GetComponentsInChildren<CToggle>();

            var template = filterObj.GetChild(0);
            
            var plist = new Dictionary<int, string> {
                { 10001, "未" }, { 10002, "阅" }
            };

            foreach (var thirdFilterInfo in plist)
            {
                var obj = Object.Instantiate(template, filterObj, false);
                var cToggle = obj.GetComponent<CToggle>();
                cToggle.Key = thirdFilterInfo.Key;
                cToggle.isOn = false;
                obj.GetComponentInChildren<TextMeshProUGUI>().text = thirdFilterInfo.Value;
                toggleGroup.Add(cToggle);
                obj.name = thirdFilterInfo.Value;
            }

            toggleGroup.InitPreOnToggle();

            toggleGroup.OnActiveToggleChange = (_, _) =>
            {
                var itemList = parentSortAndFilter._itemList;
                var filterTogGroup = parentSortAndFilter._filterTogGroup;
                var equipFilterTogGroup = parentSortAndFilter._equipFilterTogGroup;
                var onItemListChanged = parentSortAndFilter._onItemListChanged;

                Patch.UpdateItemListPrefix(parentSortAndFilter, filterTogGroup,
                    equipFilterTogGroup, itemList,
                    onItemListChanged);
            };
        }

        // 只显示适用的按钮
        for (int i = 0; i < filterObj.childCount; i++)
        {
            var toggle = filterObj.GetChild(i).GetComponent<CToggle>();
            if (toggle.Key == 0 || toggle.Key == 10001 || toggle.Key == 10002)
            {
                toggle.gameObject.SetActive(true);
            }
            else
            {
                // Debug.Log($"{toggle.gameObject.name} {toggle.Key} not active");
                toggle.gameObject.SetActive(false);
            }
        }

        filterObj.gameObject.SetActive(true);

        if (places.OriginalViewportPos == null || places.OriginalViewportSize == null)
        {
            places.OriginalViewportPos = viewport.position;
            places.OriginalViewportSize = viewport.sizeDelta;
        }

        filterObj.transform.position = places.SecondFilterPos;

        viewport.position = places.NewViewportPos;
        viewport.sizeDelta = places.NewViewportSize;

        return filterObj.GetComponent<CToggleGroup>();
    }

    public static void TurnOffThirdFilter(ItemSortAndFilterType? uiType, ItemSortAndFilter parentSortAndFilter)
    {
        if (uiType == null) return;
        var parentTrans = parentSortAndFilter.transform;
        var viewport = parentTrans.parent.GetComponent<CScrollRect>().Viewport;
        var filterObj = parentTrans.Find("ThirdFilter");
        if (filterObj == null) return;
        if (!filterObj.gameObject.activeSelf) return; // 已经关了的不要再关一遍

        filterObj.gameObject.SetActive(false);
        ThirdFilterPlaces.TryGetValue((ItemSortAndFilterType)uiType, out var places);
        if (places?.OriginalViewportPos == null || places.OriginalViewportSize == null) return;
        viewport.position = (Vector3)places.OriginalViewportPos;
        viewport.sizeDelta = (Vector2)places.OriginalViewportSize;
    }

    public class ThirdFilterInfo
    {
        public readonly ThirdFilterType ThirdFilterType;
        public readonly sbyte combatOrLifeSkillTypeOrSectId;
        public readonly string c;
        public readonly int togKey;

        public ThirdFilterInfo(ThirdFilterType thirdFilterType, sbyte combatOrLifeSkillTypeOrSectId, string c,
            int togKey)
        {
            ThirdFilterType = thirdFilterType;
            this.combatOrLifeSkillTypeOrSectId = combatOrLifeSkillTypeOrSectId;
            this.c = c;
            this.togKey = togKey;
        }
    }
}