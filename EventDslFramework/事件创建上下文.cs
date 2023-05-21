using Config.EventConfig;

namespace EventDslFramework;

public class 事件创建上下文 : 事件引用
{
    private DSL事件 _eventItem = new();
    private string _name;
    private List<事件选项上下文> _options = new();

    public 事件创建上下文? ParentEvent;
    public 事件选项上下文? ParentOption;

    public DSL事件 BuildEventItem()
    {
        _eventItem.EventOptions = _options.Select(it => it.BuildOption()).ToArray();
        return _eventItem;
    }

    public 事件创建上下文(string 事件文本 = "")
    {
        _name = "事件" + _eventItem.Guid;
        其文本为(事件文本);
    }

    public 事件创建上下文 其文本为(string 事件文本)
    {
        _eventItem.SetLanguage(new[] { 事件文本 });
        return this;
    }

    public 事件创建上下文 其主角色为(string key)
    {
        _eventItem.MainRoleKey = key;
        return this;
    }

    public 事件创建上下文 其目标角色为(string key)
    {
        _eventItem.TargetRoleKey = key;
        return this;
    }

    public 事件创建上下文 其触发条件为(触发条件 条件)
    {
        _eventItem.TriggerType = (short)条件;
        _eventItem.IsHeadEvent = 条件 != 触发条件.无;
        return this;
    }
    
    public 事件创建上下文 控制遮罩(控制遮罩类型 类型, float 渐变时间)
    {
        _eventItem.MaskControl = (sbyte)类型;
        _eventItem.MaskTweenTime = 渐变时间;
        return this;
    }

    public 事件创建上下文 其事件排序为(short 事件排序)
    {
        _eventItem.EventSortingOrder = 事件排序;
        return this;
    }
    
    public 事件创建上下文 其事件背景图为(string 事件背景图名)
    {
        _eventItem.EventBackground = 事件背景图名;
        return this;
    }
    
    public 事件创建上下文 进入事件时触发(DSL事件.进入离开触发 进入事件触发)
    {
        _eventItem.进入事件触发器 = 进入事件触发;
        return this;
    }
    
    public 事件创建上下文 离开事件时触发(DSL事件.进入离开触发 离开事件触发)
    {
        _eventItem.离开事件触发器 = 离开事件触发;
        return this;
    }
    public 事件创建上下文 自定义内容替换(DSL事件.内容替换方法 自定义内容替换)
    {
        _eventItem.自定义内容替换 = 自定义内容替换;
        return this;
    }
    public 事件创建上下文 自定义条件检查(DSL事件.条件检查方法 自定义条件检查)
    {
        _eventItem.自定义条件检查 = 自定义条件检查;
        return this;
    }

    public string 获取GUID()
    {
        return _eventItem.Guid.ToString();
    }

    public DSL事件 GetEventItem()
    {
        return _eventItem;
    }

    public 事件选项上下文 带有选项(string 选项文本 = "")
    {
        var 事件选项上下文 = new 事件选项上下文(this, 选项文本);
        _options.Add(事件选项上下文);
        return 事件选项上下文;
    }

    public 事件选项上下文 另外_上级选项()
    {
        if (ParentOption == null) throw new Exception("不存在上级选项!");
        return ParentOption;
    }
    
    public 事件创建上下文 另外_上级事件()
    {
        if (ParentEvent == null) throw new Exception("不存在上级选项!");
        return ParentEvent;
    }
}