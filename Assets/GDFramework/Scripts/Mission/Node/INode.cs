namespace GDFramework.Mission
{
    /// <summary>
    /// 任务节点叶
    /// </summary>
    public interface INode
    {
        /// <summary>
        /// 激活任务节点
        /// </summary>
        public void ActivateMissionNode();
        
        /// <summary>
        /// 任务节点完成
        /// </summary>
        public void CompleteMissionNode();
        
        /// <summary>
        /// 检查任务节点条件
        /// </summary>
        /// <returns></returns>
        public bool CheckMissionNodeCondition();
        
        /// <summary>
        /// 更新任务进度
        /// </summary>
        public void UpdataMissionNodeProgress();
        
        /// <summary>
        /// 取消任务
        /// </summary>
        public void UnActivateMissionNode();
    }
}