using Sirenix.OdinInspector;

namespace Game.World
{
    [LabelText("节点状态枚举")]
    public enum ENodeState
    {
        [LabelText("隐藏中")]
        Hidden,
        [LabelText("锁定中")]
        Locked,
        [LabelText("可触发")]
        Triggerable, 
        [LabelText("已触发")]
        Triggered,
    }
}