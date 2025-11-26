namespace Core.Game.Chunk.Room.Grid
{
    /// <summary>
    /// 地块标记
    /// 用于标记地块的特殊属性
    /// </summary>
    [System.Flags]
    public enum TileFlags
    {
        None = 0,
        
        /// <summary>
        /// 可行走
        /// </summary>
        Walkable = 1 << 0,
        
        /// <summary>
        /// 可放置物品
        /// </summary>
        Placeable = 1 << 1,
        
        /// <summary>
        /// 可游泳
        /// </summary>
        Swimmable = 1 << 2,
        
        /// <summary>
        /// 会造成伤害
        /// </summary>
        Damaging = 1 << 3,
        
        /// <summary>
        /// 会减速
        /// </summary>
        SlowDown = 1 << 4,
        
        /// <summary>
        /// 滑行（冰面）
        /// </summary>
        Slippery = 1 << 5,
        
        /// <summary>
        /// 已锁定（不可编辑）
        /// </summary>
        Locked = 1 << 6
    }
}