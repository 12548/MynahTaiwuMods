using GameData.Domains.Global;
using HarmonyLib;

namespace TaiwuSavingFolder;

[HarmonyPatch]
public class GlobalDomainPatch
{
    [HarmonyPostfix, HarmonyPatch(typeof(GlobalDomain), "LeaveWorld")]
    public static void LeaveWorldPostfix()
    {
        
    }
}