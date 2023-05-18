namespace TaiwuEventDumper;

public class TweData
{
    public string EventGuid;
    public long TmEdit = 0;
    public string SaveVersion = "Alpha_1";
    public string EventGroup;
    public bool ForceSingle;
    public string EventType = "ModEvent";
    public string TriggerType; // need todo
    public int EventOrder = 500;
    public string DecideRole = "";
    public string TargetRole = "";
    public string EventTexture = "";
    public bool InternalTexture = true;
    public string TexturePath = "";
    public bool ControlMask;
    public sbyte ControlMaskCode;
    public string MaskTweenTime;
    public string AudioName;
    public string EscOption;
    public TweEventOption[] Options;

    public TweData(string eventGuid, string eventGroup, bool forceSingle, string triggerType, int eventOrder,
        string decideRole, string targetRole, string eventTexture, bool internalTexture, string texturePath,
        bool controlMask, sbyte controlMaskCode, string maskTweenTime, string audioName, string escOption,
        TweEventOption[] options)
    {
        EventGuid = eventGuid;
        EventGroup = eventGroup;
        ForceSingle = forceSingle;
        TriggerType = triggerType;
        EventOrder = eventOrder;
        DecideRole = decideRole;
        TargetRole = targetRole;
        EventTexture = eventTexture;
        InternalTexture = internalTexture;
        TexturePath = texturePath;
        ControlMaskCode = controlMaskCode;
        MaskTweenTime = maskTweenTime;
        AudioName = audioName;
        EscOption = escOption;
        Options = options;
    }
}

public class TweEventOption
{
    public string Guid;
    public string OptionKey;

    public TweEventOption(string guid, string optionKey)
    {
        Guid = guid;
        OptionKey = optionKey;
    }
}