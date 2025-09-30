using Core.Game.Chunk.Data;
using Core.Game.Chunk.Node.Action;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Node
{
    [CreateAssetMenu(fileName = "NodeDto", menuName = "Core/NodeDto")]
    public class NodeDto : ChunkDto
    {
        [LabelText("节点触发时会发生的效果")]
        public ActionTriggerData actionTriggerData;
    }
}