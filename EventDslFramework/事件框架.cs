namespace EventDslFramework;

public static class 事件框架
{
    public static readonly Dictionary<string, string> EventNameToGuid = new();
    public static List<事件创建上下文> EventContextList = new();

    public static 事件创建上下文 创建事件(string 内容 = "自定义事件文本")
    {
        事件创建上下文 item = new(内容);
        
        EventContextList.Add(item);
        return item;
    }
    
    public static 事件创建上下文 创建简单事件(string 内容 = "自定义事件文本", string 选项 = "(……)")
    {
        事件创建上下文 上下文 = new(内容);
        上下文.带有选项(选项);
        
        EventContextList.Add(上下文);
        return 上下文;
    }
}