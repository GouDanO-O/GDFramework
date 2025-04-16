namespace GDFramework.Mission
{
    /// <summary>
    /// 任务进度
    /// </summary>
    public enum EMissionStatus
    {
        Inactive,   // 未激活
        Active,     // 激活
        Completed,  // 完成
        Failed      // 失败
    }
    
    /// <summary>
    /// 任务类型
    /// </summary>
    public enum EMissionType
    {
        Main,       // 主线任务
        Side,       // 支线任务
        Daily,      // 日常任务
        Achievement // 成就
    }
    
    /// <summary>
    /// 任务需求模式
    /// </summary>
    public enum EMissionRequireMode
    {
        All,
        Any
    }
    
    /// <summary>
    /// 任务条件
    /// </summary>
    public enum EMissionConditionType
    {
        KillEnemy,
        CollectItem,
        TalkToNpc,
        ReachLocation,
        Custom
    }
}