using System;
using Core.Game.Chunk.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Node.Data
{
    [CreateAssetMenu(fileName = "NodeDto", menuName = "Core/NodeDto")]
    public class NodeDto : ChunkDto
    {
        [LabelText("节点数据")]
        public NodeDtoDef nodeDtoDef;

        protected override void ChangingDtoData()
        {
            base.ChangingDtoData();
            
        }

        public override ChunkDtoDef CreateRuntimeDef()
        {
            return nodeDtoDef.Clone();
        }
    }
}