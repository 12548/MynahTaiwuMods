using MynahBaseModBase;
using TaiwuModdingLib.Core.Plugin;

namespace MynahMoreInfoBackend;

[PluginConfig("MynahMoreInfo", "myna12548", "1")]
public class ModEntry : TaiwuRemakeHarmonyPlugin
{
    [ModSetting("人物浮窗显示可请教技能", description: "在详细人物浮窗中显示可请教的技艺")]
    public static bool ShowLearnableSkill = true;

    public static string StaticModIdStr;
    public override void Initialize()
    { 
        base.Initialize();
        StaticModIdStr = ModIdStr;
    }
}