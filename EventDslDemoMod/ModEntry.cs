using EventDslFramework;
using GameData.Domains;
using GameData.Domains.TaiwuEvent.EventHelper;
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
            .带有选项("(遣离……)")
                .自定义选项显示条件((evt, box) => EventHelper.IsInGroup(box.GetInt("CharacterId")))
                .选择时触发((evt, box) =>
                {
                    var charId = box.GetInt("CharacterId");
                    DomainManager.Taiwu.LeaveGroup(
                        DomainManager.TaiwuEvent.MainThreadDataContext, 
                        charId,
                        moveToRandomAdjacentBlock: false
                        );
                    DomainManager.TaiwuEvent.OnEvent_LetTeammateLeaveGroup(charId); // 支持触发离队事件
                })
                .选择时跳转到简单事件("遣离成功!", "(……)")
                .并加入到其他事件("567d1caf-8b28-4dbf-8cbe-e746e8ac8cfd");
    }

    public override void Dispose()
    {
    }
}