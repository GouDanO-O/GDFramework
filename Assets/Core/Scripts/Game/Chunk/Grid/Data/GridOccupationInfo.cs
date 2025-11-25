using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Grid
{
    /// <summary>
    /// 格子占用信息
    /// </summary>
    [Serializable]
    public class GridOccupationInfo
    {
        [LabelText("物体ID")]
        public string ObjectId;
        
        [LabelText("占用的格子")]
        [TableList]
        public List<SerializableGridPosition> Positions;
        
        [LabelText("占用时间")]
        public DateTime OccupiedTime;
    }
}