using MynahBaseModBase;
using TaiwuModdingLib.Core.Plugin;

namespace MynahMoreInfoBackend;

[PluginConfig("MynahMoreInfo", "myna12548", "1")]
public class ModEntry : TaiwuRemakeHarmonyPlugin
{
    // [ModSetting("人物浮窗显示可请教技能", description: "在详细人物浮窗中显示可请教的技艺")]
    // public static bool ShowLearnableSkill = true;

    [SliderModSetting("显示人物持有物品数", 
        minValue: 0, 
        maxValue: 15,
        defaultValue: 3,
        description: "在详细人物浮窗中显示人物持有的此数量的最高品级物品，为0则不显示")]
    public static int ShowNpcGoodItemsCount;
    
    [SliderModSetting("显示人物擅长功法数", 
        minValue: 0, 
        maxValue: 15,
        defaultValue: 3,
        description: "在详细人物浮窗中显示人物会的此数量的最高品级功法，为0则不显示")]
    public static int ShowNpcGoodBattleSkillsCount;

    public static string StaticModIdStr;
    public override void Initialize()
    { 
        base.Initialize();
        StaticModIdStr = ModIdStr;
    }

    public override void OnModSettingUpdate()
    {
        base.OnModSettingUpdate();
        MynahBaseModBackend.MynahBaseModBackend.OnModSettingUpdate(this);
    }
}