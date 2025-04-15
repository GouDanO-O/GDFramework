namespace GDFramework.Mission
{
    /// <summary>
    /// 所有任务图条件的基类
    /// </summary>
    public abstract class BaseCondition : MissionChainObject 
    {
        public abstract bool IsConditionMet { get; }
    }
}