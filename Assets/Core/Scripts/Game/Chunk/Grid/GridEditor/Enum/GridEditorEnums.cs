using System;

namespace Core.Game.Grid.Editor
{
    /// <summary>
    /// 编辑器模式
    /// </summary>
    public enum EditorMode
    {
        None,           // 无模式(查看)
        Structure,      // 结构编辑模式(房间、墙壁等)
        Object,         // 物体放置模式
        Terrain         // 地形编辑模式
    }

    /// <summary>
    /// 结构编辑工具类型
    /// </summary>
    public enum StructureToolType
    {
        None,           // 无工具
        DrawRoom,       // 绘制房间
        DrawWall,       // 绘制墙壁
        EraseWall,      // 擦除墙壁
        PlaceDoor,      // 放置门
        PlaceWindow,    // 放置窗户
        DrawFloor,      // 绘制地板
        DrawCeiling,    // 绘制天花板
        Erase           // 擦除工具
    }

    /// <summary>
    /// 物体编辑工具类型
    /// </summary>
    public enum ObjectToolType
    {
        None,           // 无工具
        Place,          // 放置物体
        Select,         // 选择物体
        Move,           // 移动物体
        Rotate,         // 旋转物体
        Delete          // 删除物体
    }

    /// <summary>
    /// 绘制模式
    /// </summary>
    public enum DrawMode
    {
        Single,         // 单点放置
        Line,           // 线性绘制
        Rectangle,      // 矩形绘制
        Fill            // 填充
    }

    /// <summary>
    /// 编辑器操作类型(用于撤销/重做)
    /// </summary>
    public enum EditorActionType
    {
        PlaceCell,      // 放置单元格
        RemoveCell,     // 移除单元格
        PlaceObject,    // 放置物体
        RemoveObject,   // 移除物体
        MoveObject,     // 移动物体
        RotateObject,   // 旋转物体
        Batch           // 批量操作
    }

    /// <summary>
    /// 捕捉模式
    /// </summary>
    [Flags]
    public enum SnapMode
    {
        None = 0,
        Grid = 1 << 0,      // 网格对齐
        Object = 1 << 1,    // 物体对齐
        Wall = 1 << 2,      // 墙壁对齐
        Center = 1 << 3     // 中心对齐
    }

    /// <summary>
    /// 视图模式
    /// </summary>
    public enum ViewMode
    {
        Perspective,    // 透视视图
        Top,            // 俯视图
        Front,          // 前视图
        Side            // 侧视图
    }
}