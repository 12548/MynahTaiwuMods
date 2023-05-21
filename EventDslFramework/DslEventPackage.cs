using Config.EventConfig;

namespace EventDslFramework;

public class DslEventPackage: EventPackage
{
    public DslEventPackage()
    {
        this.NameSpace = "Taiwu";
        this.Author = "Myna12548";
        this.Group = "MynaDslEvents";
        List<TaiwuEventItem> taiwuEventItemList = new List<TaiwuEventItem>();
        foreach (var 事件创建上下文 in 事件框架.EventContextList)
        {
            taiwuEventItemList.Add(事件创建上下文.BuildEventItem());
        }
        this.EventList = taiwuEventItemList;
    }
}
