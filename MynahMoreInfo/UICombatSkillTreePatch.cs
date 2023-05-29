using HarmonyLib;

namespace MynahMoreInfo;

[HarmonyPatch]
public static class UICombatSkillTreePatch
{
    /// <summary>
    /// 显示不传之秘
    /// </summary>
    /// <param name="visibleSkill"></param>
    [HarmonyPrefix]
    [HarmonyPatch(typeof(UI_CombatSkillTree), "RefreshSkillItem")]
    public static void PreFixRefreshSkillItem(ref bool visibleSkill)
    {
        if (ModEntry.ShowNonPublicSkill)
        {
            visibleSkill = true;
        }
    }
}