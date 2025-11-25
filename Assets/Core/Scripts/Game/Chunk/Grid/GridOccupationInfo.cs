using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace Core.Game.Grid
{
    /// <summary>
    /// 格子占用信息
    /// </summary>
    [Serializable]
    public class GridOccupationInfo
    {
        [LabelText("物体ID")]
        [TableColumnWidth(150)]
        [JsonProperty]
        public string ObjectId;
        
        [LabelText("占用格子")]
        [TableColumnWidth(300)]
        [JsonProperty]
        public List<SerializableGridPosition> Positions;
        
        [LabelText("格子数量")]
        [TableColumnWidth(80)]
        [ReadOnly]
        [JsonProperty]
        public int CellCount;
        
        [LabelText("占用时间")]
        [TableColumnWidth(150)]
        [JsonProperty]
        public DateTime OccupiedTime;

        [Button("显示详情", ButtonSizes.Small)]
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 0)]
        private void ShowDetails()
        {
            string posStr = string.Join(", ", Positions.Select(p => p.ToString()));
            UnityEngine.Debug.Log($"Object: {ObjectId}\nPositions: {posStr}\nTime: {OccupiedTime}");
        }

        public override string ToString()
        {
            return $"{ObjectId} ({CellCount} cells) at {OccupiedTime:HH:mm:ss}";
        }
    }
}