using System;
using System.Collections.Generic;
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
    private static bool togInited = false;

    private static float _scrollRectOriginalPositionY;
    private static float _scrollRectOriginalSizeY;

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
    static bool UpdateItemListPrefix(
        ItemSortAndFilter __instance,
        CToggleGroup ____filterTogGroup,
        CToggleGroup ____equipFilterTogGroup,
        List<ItemDisplayData> ____itemList,
        Action ____onItemListChanged
    )
    {
        __instance.OutputItemList.Clear();

        // 有二级筛选切二级筛选不为“全”时进入
        if (____equipFilterTogGroup.gameObject.activeSelf && ____equipFilterTogGroup.GetActive().Key != 0)
        {
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
        if (TryInitSubtypeFilter(__instance, currentTogKey)) return;

        if (____equipFilterTogGroup.isActiveAndEnabled)
        {
            ____equipFilterTogGroup.Set(0);
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
        if (sortAndFilter.transform.parent.parent.parent.parent.name != "UI_CharacterMenuItems")
            return true;

        var viewport = sortAndFilter.transform.parent.GetComponent<CScrollRect>().Viewport;
        var equipTypeFilter = sortAndFilter.CGet<CToggleGroup>("EquipTypeFilter");
        if (viewport == null || equipTypeFilter == null) return true;

        var itemFilterType = (ItemSortAndFilter.ItemFilterType)currentTogKey;

        if (Utils.ItemFilterTypeToSubTypeString.ContainsKey(itemFilterType) ||
            itemFilterType == ItemSortAndFilter.ItemFilterType.Equip)
        {
            equipTypeFilter.gameObject.SetActive(true);
            if (true)
            {
                if (!togInited)
                {
                    // 未初始化，第一次初始化
                    togInited = true;
                    var template = equipTypeFilter.transform.Find("All").gameObject;

                    foreach (var keyValuePair in Utils.ItemFilterTypeToSubTypeString)
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
            }

            Dictionary<int, string> subTypes = null;
            Utils.ItemFilterTypeToSubTypeString.TryGetValue(itemFilterType, out subTypes);

            Debug.Log($"there are {equipTypeFilter.transform.childCount} toggles");
            for (int i = 0; i < equipTypeFilter.transform.childCount; i++)
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
                    Debug.Log($"subtype{tog.Key - 100} not in {currentTogKey}");
                    tog.isOn = false;
                    obj.SetActive(false);
                }
            }

            equipTypeFilter.gameObject.SetActive(true);

            _scrollRectOriginalPositionY = viewport.position.y;
            _scrollRectOriginalSizeY = viewport.sizeDelta.y;

            equipTypeFilter.transform.position = new Vector3(-3.13f, 3.20f, 240.0f);

            viewport.position = new Vector3(0.48f, 2.80f, 240f);
            viewport.sizeDelta = new Vector2(10f, -158f);
        }
        else
        {
            equipTypeFilter.gameObject.SetActive(false);
            viewport.position = new Vector3(0.48f, 3.22f, 240f);
            viewport.sizeDelta = new Vector2(10f, -98f);
        }

        return false;
    }
}