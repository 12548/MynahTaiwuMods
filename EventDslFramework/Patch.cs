using Config.EventConfig;
using GameData.Domains.Mod;
using GameData.Domains.TaiwuEvent;
using HarmonyLib;

namespace EventDslFramework;

[HarmonyPatch]
public class Patch
{
    /// <summary>
    /// 在其他事件加载完毕之后加载mod事件。
    /// </summary>
    [HarmonyPatch(typeof(ModDomain), nameof(ModDomain.LoadAllEventPackages))]
    [HarmonyPostfix]
    public static void LoadAllEventPackagesPostfix()
    {
        var package = new DslEventPackage();
        for (int index = 0; index < TaiwuEventDomain._managerArray.Length; ++index)
            TaiwuEventDomain._managerArray[index]?.HandleEventPackage(package);
        TaiwuEventDomain._packagesList.Add(package);
    }
}