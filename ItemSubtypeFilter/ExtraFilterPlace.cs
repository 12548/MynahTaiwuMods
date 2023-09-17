using UnityEngine;

namespace ItemSubtypeFilter;

/// <summary>
/// 指示额外过滤器的UI坐标位置
/// </summary>
public class ExtraFilterPlace
{ 
    /// <summary>
    /// 新视口位置
    /// </summary>
    public Vector3 NewViewportPos;
    
    /// <summary>
    /// 新视口大小
    /// </summary>
    public Vector2 NewViewportSize;
    
    /// <summary>
    /// 新过滤器位置
    /// </summary>
    public Vector3 SecondFilterPos;
    
    /// <summary>
    /// 原视口位置
    /// </summary>
    public Vector3? OriginalViewportPos;
    
    /// <summary>
    /// 原视口大小
    /// </summary>
    public Vector2? OriginalViewportSize;
    
    /// <summary>
    /// 过滤器是否占多行
    /// </summary>
    public bool DoubleLine;

    /// <summary>
    /// 额外的过滤器位置
    /// </summary>
    public Vector3? ExtraFilterPos;

    public ExtraFilterPlace(Vector3 newViewportPos, Vector2 newViewportSize, Vector3 secondFilterPos,
        Vector3? originalViewportPos = null, Vector2? originalViewportSize = null, bool doubleLine = false, Vector3? extraFilterPos = null)
    {
        NewViewportPos = newViewportPos;
        NewViewportSize = newViewportSize;
        SecondFilterPos = secondFilterPos;
        OriginalViewportPos = originalViewportPos;
        OriginalViewportSize = originalViewportSize;
        DoubleLine = doubleLine;
        ExtraFilterPos = extraFilterPos;
    }
}