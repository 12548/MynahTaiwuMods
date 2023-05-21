using System.Diagnostics;
using EventDslFramework;
using GameData.Utilities;
using TaiwuModdingLib.Core.Plugin;

namespace EventDslDemoMod;

[PluginConfig("EventDslDemoMod", "myna12548", "1")]
public class ModEntry: TaiwuRemakePlugin
{
    public override void Initialize()
    {
        var 再见 = 事件框架.创建简单事件("再见!", "Goodbye!");

        事件框架.创建事件("你好!")
            .其主角色为("RoleTaiwu")
            .其触发条件为(触发条件.太吾进行了一次地格移动)
            .带有选项("你也好!")
                .选择时触发((evt, box) => { AdaptableLog.Info("你也好好！"); })
                .选择时跳转到(再见)
            .还带有选项("你不好!");

        事件框架.创建事件()
            .带有选项("互动主体增加额外选项啦!")
                .选择时跳转到简单事件("你选择了新选项!", "好的!")
                .并加入到其他事件("567d1caf-8b28-4dbf-8cbe-e746e8ac8cfd");
    }

    public override void Dispose()
    {
    }
}