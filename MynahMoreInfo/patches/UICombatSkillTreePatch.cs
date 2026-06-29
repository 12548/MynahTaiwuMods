using Game.Views.CombatSkillTree;
using HarmonyLib;

namespace MynahMoreInfo.patches;

[HarmonyPatch]
public static class UICombatSkillTreePatch
{
    /// <summary>
    /// 显示不传之秘
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch(typeof(CombatSkillTreeSkillItem), "Set")]
    public static void PreFixSetSkillItem(ref bool isVisible)
    {
        if (ModEntry.ShowNonPublicSkill)
        {
            isVisible = true;
        }
    }
}