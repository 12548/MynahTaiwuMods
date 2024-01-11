using System;
using System.Collections.Generic;
using Config.EventConfig;
using GameData.Domains.TaiwuEvent.Enum;

namespace TaiwuEventDumper;

public class EventDumpInfo
{
    public string GroupName;
    public TweData Twe;
    public EventData Lang;
    public List<List<string>> PossibleNexts = new();
    public string ClassName;

    public EventDumpInfo(string groupName, TweData twe, EventData lang, string className)
    {
        this.GroupName = groupName;
        this.Twe = twe;
        this.Lang = lang;
        ClassName = className;
    }
}