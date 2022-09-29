using MynahBaseModBase;
using TaiwuModdingLib.Core.Plugin;

namespace ItemSubtypeFilter;

[PluginConfig("", "myna12548", "")]
public class ModEntry : TaiwuRemakeHarmonyPlugin
{
    [ModSetting(displayName: "用于行囊")]
    public static bool UseInInventory = true;
    
    [ModSetting(displayName: "用于仓库")]
    public static bool UseInWarehouse = true;
    
    [ModSetting(displayName: "用于商店")]
    public static bool UseInShop = true;
}