using System;
using System.Collections.Generic;
using System.Reflection;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ItemSubtypeFilter;

public class ThirdFilterHelper
{
    public static Dictionary<ItemSortAndFilterType, SecondFilterHelper.SecondFilterPlace> ThirdFilterPlaces =
        new()
        {
            {
                ItemSortAndFilterType.CharacterMenuItems,
                new SecondFilterHelper.SecondFilterPlace(
                    new Vector3(0.48f, 2.42f, 240f),
                    new Vector2(10, -214),
                    new Vector3(-3.4f, 2.79f, 240f))
            },
            {
                ItemSortAndFilterType.Event,
                new SecondFilterHelper.SecondFilterPlace(
                    new Vector3(-0.17f, -1.03f, 240f),
                    new Vector2(-20f, -180f),
                    new Vector3(-3.8f, -0.65f, 240f)
                )
            },
            {
                ItemSortAndFilterType.TeaHorse,
                new SecondFilterHelper.SecondFilterPlace(
                    new Vector3(3.61f, -1.6f, 240f),
                    new Vector2(988f, 190f),
                    new Vector3(0.24f, -1.2f, 240f)
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
                var ____itemList = typeof(ItemSortAndFilter).GetField("_itemList", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(parentSortAndFilter);
                var ____filterTogGroup = typeof(ItemSortAndFilter).GetField("_filterTogGroup", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(parentSortAndFilter);
                var ____equipFilterTogGroup = typeof(ItemSortAndFilter).GetField("_equipFilterTogGroup", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(parentSortAndFilter);
                var ____onItemListChanged = typeof(ItemSortAndFilter).GetField("_onItemListChanged", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(parentSortAndFilter);

                Patch.UpdateItemListPrefix(parentSortAndFilter, ____filterTogGroup as CToggleGroup,
                    ____equipFilterTogGroup as CToggleGroup, ____itemList as List<ItemDisplayData>, ____onItemListChanged as Action);
            };
        }
        

        for (int i = 0; i < filterObj.childCount; i++)
        {
            var toggle = filterObj.GetChild(i).GetComponent<CToggle>();
            if (toggle.Key == 0 || (filterType == ThirdFilterType.CombatSkillBook && toggle.Key is > 99 and < 200) ||
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

        return filterObj.GetComponent<CToggleGroup>();
    }

    public static void TurnOffThirdFilter(ItemSortAndFilterType? uiType, ItemSortAndFilter parentSortAndFilter)
    {

        if(uiType == null) return;
        var parentTrans = parentSortAndFilter.transform;
        var viewport = parentTrans.parent.GetComponent<CScrollRect>().Viewport;
        var filterObj = parentTrans.Find("ThirdFilter");
        if (filterObj == null) return;
        if(!filterObj.gameObject.activeSelf) return; // 已经关了的不要再关一遍
        
        filterObj.gameObject.SetActive(false);
        ThirdFilterPlaces.TryGetValue((ItemSortAndFilterType)uiType, out var places);
        if (places?.OriginalViewportPos == null || places.OriginalViewportSize == null) return;
        viewport.position = (Vector3)places.OriginalViewportPos;
        viewport.sizeDelta = (Vector2)places.OriginalViewportSize;
    }

    public class ThirdFilterInfo
    {
        public ThirdFilterType ThirdFilterType;
        public sbyte combatOrLifeSkillTypeOrSubtype;
        public string c;
        public int togKey;

        public ThirdFilterInfo(ThirdFilterType thirdFilterType, sbyte combatOrLifeSkillTypeOrSubtype, string c,
            int togKey)
        {
            ThirdFilterType = thirdFilterType;
            this.combatOrLifeSkillTypeOrSubtype = combatOrLifeSkillTypeOrSubtype;
            this.c = c;
            this.togKey = togKey;
        }
    }
}