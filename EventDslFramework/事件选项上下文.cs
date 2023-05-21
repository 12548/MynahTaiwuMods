using Config.EventConfig;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.TaiwuEvent.EventHelper;

namespace EventDslFramework;

public class 事件选项上下文
{
    public delegate void 事件点击触发(DSL事件 evt, EventArgBox argBox);
    public delegate string 事件点击触发并跳转(DSL事件 evt, EventArgBox argBox);
    public delegate 事件引用 事件点击触发并自定义跳转(DSL事件 evt, EventArgBox argBox);
    
    public delegate string 内容替换方法(DSL事件 evt, EventArgBox argBox);
    public delegate bool 条件检查方法(DSL事件 evt, EventArgBox argBox);
    
    private TaiwuEventOption _option = new();

    private string _guid;
    private 事件创建上下文 _event;
    private bool _customJump;
    private 事件引用? _jumpTo;
    private 事件点击触发? _triggerBeforeJump;

    public List<事件引用> OtherEventsToAddTo = new();

    public 事件选项上下文(事件创建上下文 事件, string 选项文本 = "")
    {
        _event = 事件;
        _guid = Guid.NewGuid().ToString();
        _option.SetContent(选项文本);
        _option.OptionKey = $"DSL_Option_{_guid}";
    }

    public 事件选项上下文 作为ESC选项()
    {
        _event.GetEventItem().EscOptionKey = _option.OptionKey;
        return this;
    }

    public 事件选项上下文 选择时跳转到(事件引用 事件)
    {
        _jumpTo = 事件;
        return this;
    }
    
    public 事件创建上下文 选择时跳转到新事件(string 事件内容)
    {
        var 事件创建上下文 = 事件框架.创建事件(事件内容);
        _jumpTo = 事件创建上下文;
        事件创建上下文.ParentOption = this;
        事件创建上下文.ParentEvent = _event;
        
        return 事件创建上下文;
    }
    
    public 事件选项上下文 选择时跳转到简单事件(string 事件内容, string 选项内容)
    {
        var 事件创建上下文 = 事件框架.创建简单事件(事件内容, 选项内容);
        事件创建上下文.ParentOption = this;
        事件创建上下文.ParentEvent = _event;
        _jumpTo = 事件创建上下文;
        return this;
    }
    
    public 事件选项上下文 选择时触发(事件点击触发 触发)
    {
        _triggerBeforeJump = 触发;
        return this;
    }
    public 事件选项上下文 选择时触发(事件点击触发并跳转 触发)
    {
        _option.OnOptionSelect = () => 触发.Invoke(_event.GetEventItem(), _option.ArgBox);
        _customJump = true;
        return this;
    }
    public 事件选项上下文 选择时触发(事件点击触发并自定义跳转 触发)
    {
        _option.OnOptionSelect = () => 触发.Invoke(_event.GetEventItem(), _option.ArgBox).获取GUID();
        _customJump = true;
        return this;
    }
    
    public 事件选项上下文 选择时跳转到(string GUID)
    {
        _jumpTo = new GUID事件引用(GUID);
        return this;
    }
    
    public 事件选项上下文 并加入到其他事件(string GUID)
    {
        OtherEventsToAddTo.Add(new GUID事件引用(GUID));
        return this;
    }
    
    public 事件选项上下文 并加入到其他事件(事件引用 事件)
    {
        OtherEventsToAddTo.Add(事件);
        return this;
    }
    
    public 事件选项上下文 其立场为(立场 立场)
    {
        _option.Behavior = (sbyte)立场;
        return this;
    }
    
    public 事件选项上下文 其默认状态为(事件选项状态 状态)
    {
        _option.DefaultState = (sbyte)状态;
        return this;
    }
    
    public 事件选项上下文 自定义内容替换(内容替换方法 自定义内容替换)
    {
        _option.GetReplacedContent = () => 自定义内容替换.Invoke(_event.GetEventItem(), _option.ArgBox);;
        return this;
    }
    
    public 事件选项上下文 自定义选项显示条件(条件检查方法 条件检查方法)
    {
        _option.OnOptionVisibleCheck = () => 条件检查方法.Invoke(_event.GetEventItem(), _option.ArgBox);;
        return this;
    }
    public 事件选项上下文 自定义选项可用条件(条件检查方法 条件检查方法)
    {
        _option.OnOptionAvailableCheck = () => 条件检查方法.Invoke(_event.GetEventItem(), _option.ArgBox);;
        return this;
    }

    public 事件选项上下文 还带有选项(string 选项文本 = "")
    {
        return _event.带有选项(选项文本);
    }

    public TaiwuEventOption BuildOption()
    {
        var option = _option;
        if (!_customJump)
        {
            var jumpTarget = _jumpTo?.获取GUID() ?? string.Empty;
            _option.OnOptionSelect = () =>
            {
                _triggerBeforeJump?.Invoke(_event.GetEventItem(), _option.ArgBox);
                return jumpTarget;
            };
        }

        option.GetReplacedContent ??= () => string.Empty;
        option.OnOptionVisibleCheck ??= () => true;
        option.OnOptionAvailableCheck ??= () => true;

        option.GetExtraFormatLanguageKeys = () => default; // 多语言用的,目前无用

        
        foreach (var eventRef in OtherEventsToAddTo)
        {
            EventHelper.AddOptionToEvent(eventRef.获取GUID(), _event.获取GUID(), _option.OptionKey);
        }
        
        return option;
    }
}
