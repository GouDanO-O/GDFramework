using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data.Interface;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Data
{
    /// <summary>
    /// 容器临时数据基类(可包含子级)
    /// </summary>
    [Serializable]
    public class ChunkContainerTemporaryData : ChunkTemporaryData, IChunkContainerTemporaryData
    {
        [LabelText("子级实例ID列表")]
        public List<string> ChildInstanceIds { get; set; } = new List<string>();
        
        [LabelText("当前激活的子级")]
        public string ActiveChildInstanceId { get; set; }

        public ChunkContainerTemporaryData() : base()
        {
        }

        public ChunkContainerTemporaryData(string defId) : base(defId)
        {
        }
    }
}