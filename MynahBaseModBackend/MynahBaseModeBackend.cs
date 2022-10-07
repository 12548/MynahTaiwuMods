using System;
using System.Linq;
using System.Reflection;
using GameData.Domains;
using GameData.Domains.Mod;
using MynahBaseModBase;
using TaiwuModdingLib.Core.Plugin;
using TaiwuModdingLib.Core.Utils;

namespace MynahBaseModBackend;

[PluginConfig("MynahBaseModBackend", "myna12548", "0")]
public class DummyModEntry : TaiwuRemakeHarmonyPlugin
{
}

public class MynahBaseModBackend
{
    public static ushort GetDomainIdByName(string name)
    {
        return DomainHelper.DomainName2DomainId[name];
    }

    public static ushort GetMethodIdByName(Type domain, string name)
    {
        return (ushort)domain.GetField(name)!.GetValue(null)!;
    }

    public static void OnModSettingUpdate(TaiwuRemakePlugin plugin)
    {
        var type = plugin.GetType();
        foreach (var field in type.GetFields())
        {
            var setting = field.GetCustomAttribute(typeof(ModSetting), true);
            if (setting != null)
            {
                var modSetting = (ModSetting)setting;
                switch (modSetting.GetModSettingType(field))
                {
                    case "InputField":
                        var strArg = "";
                        DomainManager.Mod.GetSetting(plugin.ModIdStr, modSetting.GetKey(field), ref strArg);
                        field.SetValue(plugin, strArg);
                        break;

                    case "Toggle":
                        var boolArg = false;
                        DomainManager.Mod.GetSetting(plugin.ModIdStr, modSetting.GetKey(field), ref boolArg);
                        field.SetValue(plugin, boolArg);
                        break;

                    default: // ToggleGroup Slider Dropdown 全都是Int
                        var intArg = 0;
                        DomainManager.Mod.GetSetting(plugin.ModIdStr, modSetting.GetKey(field), ref intArg);
                        field.SetValue(plugin, intArg);
                        break;
                }
            }
        }
    }
}