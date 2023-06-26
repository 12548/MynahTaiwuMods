using System.Runtime.CompilerServices;
using HarmonyLib;
using UnityEngine;

namespace ItemSubtypeFilter;

/// <summary>
/// 修习物品排序
/// </summary>
[HarmonyPatch]
public static class CombatSkillSortAndFilterPatch
{
    [HarmonyPatch(typeof(CombatSkillSortAndFilter), "Init")]
    [HarmonyPostfix]
    public static void InitPostfix(CombatSkillSortAndFilter __instance)
    {
        if (ModEntry.PracticeCombatSkillSort)
        {
            var tr = __instance.transform;
            var parentName = tr.parent.parent.parent.parent.name;
            if (parentName != "Practice") return;
            var sortTypeHolder = tr.Find("SortTypeHolder");
            sortTypeHolder.gameObject.SetActive(true);
            var position = sortTypeHolder.localPosition;
            position.y = 39f;
            sortTypeHolder.localPosition = position;
        }
    }
}
