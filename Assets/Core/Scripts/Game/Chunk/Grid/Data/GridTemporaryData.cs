using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Grid
{
    public class GridTemporaryData  : ChunkTemporaryData
    {
         [Title("占用信息")]
        
        [LabelText("占用的格子信息")]
        [TableList]
        public List<GridOccupationInfo> OccupiedCells = new List<GridOccupationInfo>();
        
        [Title("自定义数据")]
        
        [LabelText("单元格自定义数据")]
        [DictionaryDrawerSettings(KeyLabel = "格子坐标", ValueLabel = "数据")]
        public Dictionary<string, string> CellCustomData = new Dictionary<string, string>();
        
        [Title("统计信息")]
        
        [LabelText("已放置物体数量")]
        [ReadOnly]
        public int PlacedObjectCount;
        
        [LabelText("可用格子数量")]
        [ReadOnly]
        public int AvailableCellCount;

        public GridTemporaryData() : base()
        {
        }

        public GridTemporaryData(string defId) : base(defId)
        {
        }

        /// <summary>
        /// 添加占用信息
        /// </summary>
        public void AddOccupation(string objectId, List<SerializableGridPosition> positions)
        {
            var occupation = new GridOccupationInfo
            {
                ObjectId = objectId,
                Positions = positions,
                OccupiedTime = DateTime.Now
            };
            
            OccupiedCells.Add(occupation);
            PlacedObjectCount++;
            LastModifyTime = DateTime.Now;
        }

        /// <summary>
        /// 移除占用信息
        /// </summary>
        public bool RemoveOccupation(string objectId)
        {
            int removed = OccupiedCells.RemoveAll(o => o.ObjectId == objectId);
            if (removed > 0)
            {
                PlacedObjectCount = Math.Max(0, PlacedObjectCount - removed);
                LastModifyTime = DateTime.Now;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取物体占用的位置
        /// </summary>
        public List<SerializableGridPosition> GetObjectPositions(string objectId)
        {
            var occupation = OccupiedCells.Find(o => o.ObjectId == objectId);
            return occupation?.Positions;
        }

        /// <summary>
        /// 清空所有占用
        /// </summary>
        public void ClearAllOccupations()
        {
            OccupiedCells.Clear();
            PlacedObjectCount = 0;
            LastModifyTime = DateTime.Now;
        }
    }
}