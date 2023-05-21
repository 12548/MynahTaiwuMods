using Config.EventConfig;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.TaiwuEvent.Enum;

namespace EventDslFramework;

public class DSL事件: TaiwuEventItem
{
    public delegate void 进入离开触发(DSL事件 evt, EventArgBox argBox);
    public delegate string 内容替换方法(DSL事件 evt, EventArgBox argBox);
    public delegate bool 条件检查方法(DSL事件 evt, EventArgBox argBox);

    public 进入离开触发? 进入事件触发器;
    public 进入离开触发? 离开事件触发器;
    public 内容替换方法? 自定义内容替换;
    public 条件检查方法? 自定义条件检查;

    public DSL事件()
    {
        Guid = Guid.NewGuid();
        EventType = EEventType.ModEvent;
        EventAudio = ""; // 音频目前无效
        
        IsHeadEvent = false;
        EventGroup = "MynaDslEvents";
        ForceSingle = false; // 不可重复触发 目前无效
        TriggerType = EventTrigger.None;
        EventSortingOrder = 500;
        MainRoleKey = "RoleTaiwu";
        TargetRoleKey = "";
        EventBackground = "";
        MaskControl = 0;
        MaskTweenTime = 0.0f;
        EscOptionKey = "";
    }

    public override bool OnCheckEventCondition()
    {
        return 自定义条件检查?.Invoke(this, ArgBox) ?? true;
    }

    public override void OnEventEnter()
    {
        进入事件触发器?.Invoke(this, ArgBox);
    }

    public override void OnEventExit()
    {
        离开事件触发器?.Invoke(this, ArgBox);
    }

    public override string GetReplacedContentString()
    {
        return 自定义内容替换?.Invoke(this, ArgBox) ?? string.Empty;
    }
}