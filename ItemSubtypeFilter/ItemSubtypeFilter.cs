using MynahBaseModBase;
using TaiwuModdingLib.Core.Plugin;

namespace ItemSubtypeFilter;

[PluginConfig("ItemSubtypeFilter", "myna12548", "")]
public class ModEntry : TaiwuRemakeHarmonyPlugin
{
    [ModSetting(displayName: "用于行囊")] public static readonly bool UseInInventory = true;

    [ModSetting(displayName: "用于仓库")] public static readonly bool UseInWarehouse = true;

    [ModSetting(displayName: "用于商店")] public static readonly bool UseInShop = true;
    
    [ModSetting(displayName: "用于茶马帮")] public static readonly bool UseInTeaHorse = true;
    
    [ModSetting(displayName: "用于事件", description: "事件界面选择物品，如偷窃等")] public static readonly bool UseInEvent = true;
    
    [ModSetting(displayName: "书籍高级筛选")] public static readonly bool BookThirdFilter = true;

    public override void OnModSettingUpdate()
    {
        base.OnModSettingUpdate();
        MynahBaseModFrontend.MynahBaseModFrontend.OnModSettingUpdate(this);
    }
}
