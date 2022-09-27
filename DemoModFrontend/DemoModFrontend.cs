using MynahBaseModBase;
using TaiwuModdingLib.Core.Plugin;

namespace DemoModFrontend
{
    [PluginConfig("DemoMod", "myna12548", "123")]
    public class ModEntry: TaiwuRemakeHarmonyPlugin
    {
        [ModSetting("布尔A", description: "这是一个简单的布尔值")]
        public bool a = true;

        [ModSetting("字符串B")] public static string b = "asdf";

        [SliderModSetting("滑动C", minValue: 3, maxValue: 30, stepSize: 1)]
        public static int c = 7;
        
        [DropDownModSetting("下拉d", options: new []{"选项J", "选项L"})]
        public static int d = 1;
    }
}