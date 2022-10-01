using System;
using System.Collections.Generic;
using Config.EventConfig;
using GameData.Domains.TaiwuEvent.Enum;

namespace MynahModConfigGenerator.EventViewerTest;

public class EventDumpInfo
{
    public short EventSortingOrder;
    public EEventType EventType;
    public bool ForceSingle;
    public string Guid;
    public bool IsHeadEvent;
    public string MainRoleKey;
    public string TargetRoleKey;
    public short TriggerType;
    public string ClassName;
    public List<string> jumps = new();

    public EventDumpInfo(TaiwuEventItem item)
    {
        EventSortingOrder = item.EventSortingOrder;
        EventType = item.EventType;
        ForceSingle = item.ForceSingle;
        Guid = item.Guid.ToString();
        IsHeadEvent = item.IsHeadEvent;
        MainRoleKey = item.MainRoleKey;
        TargetRoleKey = item.TargetRoleKey;
        TriggerType = item.TriggerType;
        ClassName = item.GetType().Name;
        
        foreach (var op in item.EventOptions)
        {
            var next = String.Empty;
            try
            {
                next = op.OnOptionSelect();
            }
            catch (Exception _)
            {
                // ignored
            }
            
            jumps.Add(next);
        }
    }
}