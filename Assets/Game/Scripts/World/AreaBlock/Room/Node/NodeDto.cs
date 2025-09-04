using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.World
{
    [CreateAssetMenu(fileName = "NodeDto", menuName = "Game/NodeDto")]
    public class NodeDto : Dto
    {
        [LabelText("节点触发时会发生的效果")]
        public ActionTriggerData actionTriggerData;
    }
}