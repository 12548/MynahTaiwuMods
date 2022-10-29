using TaiwuModdingLib.Core.Plugin;

namespace MynahMoreInfoBackend;

[PluginConfig("MynahMoreInfo", "myna12548", "1")]
public class ModEntry : TaiwuRemakeHarmonyPlugin
{
    public static string StaticModIdStr;
    public override void Initialize()
    { 
        base.Initialize();
        StaticModIdStr = ModIdStr;
    }
}