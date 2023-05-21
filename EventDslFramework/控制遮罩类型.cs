namespace EventDslFramework;

public enum 控制遮罩类型
{
    不控制 = 0,
    事件显示时渐变到全黑 = 1,
    事件隐藏时从全黑恢复 = 2,
    显示时渐变到全黑且退出时渐变恢复 = 3,
}