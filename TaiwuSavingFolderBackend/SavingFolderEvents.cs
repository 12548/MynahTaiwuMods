using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace TaiwuSavingFolder;

[SuppressMessage("ReSharper", "EventNeverSubscribedTo.Global")]
public static class SavingFolderEvents
{
    /// <summary>
    /// 离开世界（回到主菜单）时触发此事件。此时应当关闭文件夹中的所有文件，以便删除。
    /// </summary>
    public static event EventHandler? LeaveWorldEvent;
    
    /// <summary>
    /// 进入世界时触发此事件。此时可以开始使用文件夹了。
    /// </summary>
    public static event EventHandler? EnterWorldEvent;
    /// <summary>
    /// 保存（备份）世界时触发此事件。此时应当暂时停止占用写文件，避免备份出错。
    /// </summary>
    public static event EventHandler? SaveWorldEvent;

    public static void RaiseLeaveWorldEvent()
    {
        LeaveWorldEvent?.Invoke(null, EventArgs.Empty);
    }
    
    public static void RaiseEnterWorldEvent()
    {
        EnterWorldEvent?.Invoke(null, EventArgs.Empty);
    }
    
    public static void SaveEnterWorldEvent()
    {
        SaveWorldEvent?.Invoke(null, EventArgs.Empty);
    }
}