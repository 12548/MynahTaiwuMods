using MynahBaseModBase;
using TaiwuModdingLib.Core.Plugin;

namespace ItemSubtypeFilter;

[PluginConfig("ItemSubtypeFilter", "myna12548", "")]
public class ModEntry : TaiwuRemakeHarmonyPlugin
{
    [ModSetting(displayName: "用于行囊")] public static readonly bool UseInInventory = true;

    [ModSetting(displayName: "用于仓库")] public static readonly bool UseInWarehouse = true;

    [ModSetting(displayName: "用于商店")] public static readonly bool UseInShop = true;

    public override void OnModSettingUpdate()
    {
        base.OnModSettingUpdate();
        MynahBaseModFrontend.MynahBaseModFrontend.OnModSettingUpdate(this);
    }
}
