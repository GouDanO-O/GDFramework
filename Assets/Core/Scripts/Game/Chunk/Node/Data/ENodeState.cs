using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Node.Data
{
    /// <summary>
    /// 节点状态枚举
    /// </summary>
    public enum ENodeState
    {
        /// <summary>
        /// 未激活/隐藏
        /// </summary>
        [LabelText("未激活/隐藏")]
        Inactive,
        /// <summary>
        /// 激活但未完成
        /// </summary>
        [LabelText("激活但未完成")]
        Active,
        /// <summary>
        /// 已完成
        /// </summary>
        [LabelText("已完成")]
        Completed,
        /// <summary>
        /// 已锁定
        /// </summary>
        [LabelText("已锁定")]
        Locked,
        /// <summary>
        /// 失败
        /// </summary>
        [LabelText("失败")]
        Failed,
        /// <summary>
        /// 正在进行中
        /// </summary>
        [LabelText("正在进行中")]
        InProgress
    }
}