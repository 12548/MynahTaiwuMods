using GameData.Domains.Organization;
using HarmonyLib;

namespace VillagerIdentities;

[HarmonyPatch]
public class OrganizationDomainPatch
{
    /// <summary>
    /// 跳过无意义的人数检测
    /// </summary>
    /// <returns></returns>
    [HarmonyPatch(typeof(OrganizationDomain), "CheckPrincipalMembersAmount")]
    [HarmonyPrefix]
    static bool CheckPrincipalMembersAmountPrefix()
    {
        return false;
    }
}
