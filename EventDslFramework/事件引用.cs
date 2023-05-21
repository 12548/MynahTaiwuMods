namespace EventDslFramework;

public interface 事件引用
{
    public string 获取GUID();
}

public class GUID事件引用 : 事件引用
{
    private string _guid;

    public GUID事件引用(string guid)
    {
        _guid = guid;
    }
    
    public string 获取GUID()
    {
        return _guid;
    }
}