using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using HarmonyLib;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ItemSubtypeFilter;

[HarmonyPatch]
public class Patch
{
    /// <summary>
    /// 完全替换原来的UpdateItemList，优先细类划分。
    /// </summary>
    /// <param name="__instance"></param>
    /// <param name="____filterTogGroup"></param>
    /// <param name="____equipFilterTogGroup"></param>
    /// <param name="____itemList"></param>
    /// <param name="____onItemListChanged"></param>
    /// <returns></returns>
    [HarmonyPatch(typeof(ItemSortAndFilter), "UpdateItemList")]
    [HarmonyPrefix]
    public static bool UpdateItemListPrefix(
        ItemSortAndFilter __instance,
        CToggleGroup ____filterTogGroup,
        CToggleGroup ____equipFilterTogGroup,
        List<ItemDisplayData> ____itemList,
        Action ____onItemListChanged
    )
    {
        __instance.OutputItemList.Clear();

        if (__instance.transform == null) return true;

        var thirdFilter = __instance.transform.Find("ThirdFilter");
        if (thirdFilter != null && thirdFilter.gameObject.activeSelf &&
            thirdFilter.GetComponent<CToggleGroup>().GetActive().Key != 0)
        {
            // 有三级筛选且三级筛选不为“全”时进入
            Debug.Log("thirdFilter!");

            var togKey = thirdFilter.GetComponent<CToggleGroup>().GetActive().Key;
            List<ItemSortAndFilter.EquipFilterType> typeList = __instance.SortFilterSetting.EquipFilterType;
            if (typeList.Count == 0 || typeList[0] == ItemSortAndFilter.EquipFilterType.Invalid)
            {
                __instance.OutputItemList.AddRange(____itemList);
            }
            else
            {
                __instance.OutputItemList.AddRange(____itemList.FindAll(data =>
                {
                    var itemSubType = ItemTemplateHelper.GetItemSubType(data.Key.ItemType, data.Key.TemplateId);
                    if (itemSubType == 1001)
                    {
                        var skillBookItem = SkillBook.Instance[data.Key.TemplateId];
                        var thirdFilterInfo = ThirdFilterHelper.ThirdFilters.Find(it => it.togKey == togKey);
                        Debug.Log(
                            $"Combat skill: {skillBookItem.CombatSkillType} vs {thirdFilterInfo.combatOrLifeSkillTypeOrSubtype}");
                        return (thirdFilterInfo.ThirdFilterType == ThirdFilterType.CombatSkillBook &&
                                skillBookItem.CombatSkillType == thirdFilterInfo.combatOrLifeSkillTypeOrSubtype);
                    }
                    else if (itemSubType == 1000)
                    {
                        var skillBookItem = SkillBook.Instance[data.Key.TemplateId];
                        var thirdFilterInfo = ThirdFilterHelper.ThirdFilters.Find(it => it.togKey == togKey);
                        return (thirdFilterInfo.ThirdFilterType == ThirdFilterType.LifeSkillBook &&
                                skillBookItem.LifeSkillType == thirdFilterInfo.combatOrLifeSkillTypeOrSubtype);
                    }
                    else return false;
                }));
            }
        }
        else if (____equipFilterTogGroup.gameObject.activeSelf && ____equipFilterTogGroup.GetActive().Key != 0)
        {
            // 有二级筛选且二级筛选不为“全”时进入
            List<ItemSortAndFilter.EquipFilterType> typeList = __instance.SortFilterSetting.EquipFilterType;
            if (typeList.Count == 0 || typeList[0] == ItemSortAndFilter.EquipFilterType.Invalid)
            {
                __instance.OutputItemList.AddRange(____itemList);
            }
            else
            {
                // 99以下为原版装备筛选，100以上为子类型筛选
                var togKey = ____equipFilterTogGroup.GetActive().Key;
                if (togKey < 99)
                {
                    __instance.OutputItemList.AddRange(____itemList.FindAll(data =>
                        typeList.Contains(ItemSortAndFilter.GetEquipFilterType(data.Key))));
                }
                else
                {
                    var subType = togKey - 100;
                    __instance.OutputItemList.AddRange(____itemList.FindAll(data =>
                        ItemTemplateHelper.GetItemSubType(data.Key.ItemType, data.Key.TemplateId) == subType));
                }
            }
        }
        else if (____filterTogGroup.gameObject.activeSelf)
        {
            // 如果没有细分 按大类分
            List<ItemSortAndFilter.ItemFilterType> typeList = __instance.SortFilterSetting.ItemFilterType;
            if (typeList.Count == 0 || typeList[0] == ItemSortAndFilter.ItemFilterType.Invalid)
            {
                __instance.OutputItemList.AddRange(____itemList);
            }
            else
            {
                __instance.OutputItemList.AddRange(____itemList.FindAll(data =>
                    typeList.Contains(ItemSortAndFilter.GetFilterType(data.Key.ItemType))));
            }
        }
        else
        {
            // 否则不分
            __instance.OutputItemList.AddRange(____itemList);
        }

        bool sortEnabled = __instance.SortEnabled;
        if (sortEnabled)
        {
            __instance.OutputItemList.Sort(
                (itemL, itemR) => ItemCompare(__instance, itemL, itemR));
        }

        Action onItemListChanged = ____onItemListChanged;
        if (onItemListChanged != null)
        {
            onItemListChanged();
        }

        // 完全替换原函数
        return false;
    }

    [HarmonyReversePatch]
    [HarmonyPatch(typeof(ItemSortAndFilter), "UpdateItemList")]
    public static void OriginalUpdateItemList(object instance)
    {
        // its a stub so it has no initial content
        throw new Exception("It's a stub");
    }

    [HarmonyReversePatch]
    [HarmonyPatch(typeof(ItemSortAndFilter), "ItemCompare")]
    public static int ItemCompare(object instance, ItemDisplayData itemL, ItemDisplayData itemR)
    {
        // its a stub so it has no initial content
        throw new Exception("It's a stub");
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ItemSortAndFilter), "OnItemFilterTogChange")]
    public static void OnItemFilterTogChangePostfix(
        ItemSortAndFilter __instance,
        CToggleGroup ____filterTogGroup,
        CToggleGroup ____equipFilterTogGroup,
        List<ItemDisplayData> ____itemList,
        Action ____onItemListChanged,
        bool ____filterTogInitializing,
        CToggle togNew, CToggle togOld)
    {
        if (____filterTogInitializing) return;
        var currentTogKey = togNew.Key;
        
        ThirdFilterHelper.TurnOffThirdFilter(SecondFilterHelper.GetItemSortAndFilterType(__instance),
            __instance);
        
        if (TryInitSubtypeFilter(__instance, currentTogKey)) return;

        if (____equipFilterTogGroup.isActiveAndEnabled)
        {
            ____equipFilterTogGroup.Set(0);
        }

        UpdateItemListPrefix(__instance, ____filterTogGroup, ____equipFilterTogGroup, ____itemList,
            ____onItemListChanged);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ItemSortAndFilter), "OnEquipFilterTogChange")]
    public static void OnEquipFilterTogChangePostfix(
        ItemSortAndFilter __instance,
        CToggleGroup ____filterTogGroup,
        CToggleGroup ____equipFilterTogGroup,
        List<ItemDisplayData> ____itemList,
        Action ____onItemListChanged,
        bool ____filterTogInitializing,
        CToggle togNew, CToggle togOld)
    {
        if (____filterTogInitializing) return;

        if (!ModEntry.BookThirdFilter) return;

        ThirdFilterType thirdFilterType;

        switch (togNew.gameObject.name)
        {
            case "Subtype1001":
                thirdFilterType = ThirdFilterType.CombatSkillBook;
                break;
            case "Subtype1000":
                thirdFilterType = ThirdFilterType.LifeSkillBook;
                break;
            default:
                ThirdFilterHelper.TurnOffThirdFilter(SecondFilterHelper.GetItemSortAndFilterType(__instance),
                    __instance);
                return;
        }

        var sortAndFilterType = SecondFilterHelper.GetItemSortAndFilterType(__instance);
        if (sortAndFilterType == null)
        {
            return;
        }

        var toggleGroup =
            ThirdFilterHelper.EnsureThirdFilter((ItemSortAndFilterType)sortAndFilterType, __instance, thirdFilterType);

        if (toggleGroup == null) return;

        if (toggleGroup.isActiveAndEnabled)
        {
            toggleGroup.Set(0);
        }

        UpdateItemListPrefix(__instance, ____filterTogGroup, ____equipFilterTogGroup, ____itemList,
            ____onItemListChanged);
    }

    // [HarmonyPostfix]
    // [HarmonyPatch(typeof(ItemSortAndFilter), "SetItemList")]
    // public static void OnSetItemListPostfix(
    //     ItemSortAndFilter __instance,
    //     CToggleGroup ____filterTogGroup,
    //     CToggleGroup ____equipFilterTogGroup,
    //     List<ItemDisplayData> ____itemList,
    //     Action ____onItemListChanged,
    //     bool ____filterTogInitializing)
    // {
    //     if (____filterTogInitializing) return;
    //     var currentTogKey = ____filterTogGroup.GetActive().Key;
    //     if (TryInitSubtypeFilter(__instance, currentTogKey)) return;
    //     UpdateItemListPrefix(__instance, ____filterTogGroup, ____equipFilterTogGroup, ____itemList, ____onItemListChanged);
    // }

    private static bool TryInitSubtypeFilter(ItemSortAndFilter sortAndFilter, int currentTogKey)
    {
        var parentTransform = sortAndFilter.transform.parent;
        var sortAndFilterType = SecondFilterHelper.GetItemSortAndFilterType(sortAndFilter);

        switch (sortAndFilterType)
        {
            case ItemSortAndFilterType.CharacterMenuItems:
                if (!ModEntry.UseInInventory) return true;
                break;

            case ItemSortAndFilterType.WarehouseInventory:
            case ItemSortAndFilterType.Warehouse:
                if (!ModEntry.UseInWarehouse) return true;
                break;

            case ItemSortAndFilterType.Shop:
            case ItemSortAndFilterType.ShopInventory:
                if (!ModEntry.UseInShop) return true;
                break;
            case ItemSortAndFilterType.Event:
                if (!ModEntry.UseInEvent) return true;
                break;
            case ItemSortAndFilterType.TeaHorse:
                if (!ModEntry.UseInTeaHorse) return true;
                break;
            case ItemSortAndFilterType.ExchangeBookLeft:
            case ItemSortAndFilterType.ExchangeBookRight:
                if (!ModEntry.UseInBookEx) return true;
                break;

            default:
                return true;
        }
        
        var places = SecondFilterHelper.SecondFilterPlaces[(ItemSortAndFilterType)sortAndFilterType];
        if (places == null) return true;

        var viewport = parentTransform.GetComponent<CScrollRect>().Viewport;
        var equipTypeFilter = sortAndFilter.CGet<CToggleGroup>("EquipTypeFilter");
        if (viewport == null || equipTypeFilter == null) return true;

        var itemFilterType = (ItemSortAndFilter.ItemFilterType)currentTogKey;

        if (SecondFilterHelper.ItemFilterTypeToSubTypeString.ContainsKey(itemFilterType) ||
            itemFilterType == ItemSortAndFilter.ItemFilterType.Equip)
        {
            equipTypeFilter.gameObject.SetActive(true);
            if (equipTypeFilter.transform.Find("Subtype701") == null)
            {
                // 未初始化，第一次初始化
                var template = equipTypeFilter.transform.Find("All").gameObject;

                foreach (var keyValuePair in SecondFilterHelper.ItemFilterTypeToSubTypeString)
                {
                    foreach (var valuePair in keyValuePair.Value)
                    {
                        var obj = Object.Instantiate(template, equipTypeFilter.transform, false);
                        var cToggle = obj.GetComponent<CToggle>();
                        cToggle.Key = valuePair.Key + 100;
                        cToggle.isOn = false;
                        obj.GetComponentInChildren<TextMeshProUGUI>().text = valuePair.Value;
                        equipTypeFilter.Add(cToggle);
                        obj.name = "Subtype" + valuePair.Key;
                        obj.SetActive(false);
                    }
                }
            }

            SecondFilterHelper.ItemFilterTypeToSubTypeString.TryGetValue(itemFilterType, out var subTypes);
            Debug.Log($"there are {equipTypeFilter.transform.childCount} toggles");
            for (var i = 0; i < equipTypeFilter.transform.childCount; i++)
            {
                var child = equipTypeFilter.transform.GetChild(i);
                var obj = child.gameObject;
                var tog = child.GetComponent<CToggle>();

                if (subTypes == null)
                {
                    obj.SetActive(tog.Key < 100 && itemFilterType == ItemSortAndFilter.ItemFilterType.Equip);
                }
                else if (subTypes.ContainsKey(tog.Key - 100))
                {
                    obj.SetActive(true);
                }
                else if (tog.Key > 0)
                {
                    tog.isOn = false;
                    obj.SetActive(false);
                }
            }

            equipTypeFilter.gameObject.SetActive(true);

            if (places.OriginalViewportPos == null || places.OriginalViewportSize == null)
            {
                places.OriginalViewportPos = viewport.position;
                places.OriginalViewportSize = viewport.sizeDelta;
            }

            equipTypeFilter.transform.position = places.SecondFilterPos;

            viewport.position = places.NewViewportPos;
            viewport.sizeDelta = places.NewViewportSize;
        }
        else
        {
            equipTypeFilter.gameObject.SetActive(false);

            if (places.OriginalViewportPos != null && places.OriginalViewportSize != null)
            {
                viewport.position = (Vector3)places.OriginalViewportPos;
                viewport.sizeDelta = (Vector2)places.OriginalViewportSize;
            }
        }

        return false;
    }
}