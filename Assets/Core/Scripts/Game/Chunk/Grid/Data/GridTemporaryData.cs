using System;
using System.Collections.Generic;
using System.Linq;
using Core.Game.Chunk.Data;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace Core.Game.Grid.Data
{
/// <summary>
    /// 网格临时数据
    /// 保存运行时状态,支持序列化和持久化
    /// </summary>
    [Serializable]
    public class GridTemporaryData : ChunkTemporaryData
    {
        [Title("占用信息")]
        [BoxGroup("占用")]
        
        [LabelText("占用的格子信息")]
        [TableList(AlwaysExpanded = true, ShowIndexLabels = true)]
        [JsonProperty]
        public List<GridOccupationInfo> OccupiedCells = new List<GridOccupationInfo>();

        [Title("自定义单元格数据")]
        [BoxGroup("自定义数据")]
        
        [LabelText("单元格自定义数据")]
        [DictionaryDrawerSettings(KeyLabel = "格子坐标", ValueLabel = "JSON数据")]
        [JsonProperty]
        public Dictionary<string, string> CellCustomData = new Dictionary<string, string>();

        [Title("运行时统计")]
        [BoxGroup("统计")]
        
        [LabelText("已放置物体数量")]
        [ReadOnly]
        [ShowInInspector]
        [JsonProperty]
        public int PlacedObjectCount;
        
        [LabelText("占用格子总数")]
        [ReadOnly]
        [ShowInInspector]
        [JsonProperty]
        public int TotalOccupiedCells;
        
        [LabelText("可用格子数量")]
        [ReadOnly]
        [ShowInInspector]
        [JsonProperty]
        public int AvailableCellCount;
        
        [LabelText("最后放置时间")]
        [ReadOnly]
        [ShowInInspector]
        [JsonProperty]
        public DateTime? LastPlacementTime;

        [Title("运行时状态")]
        [BoxGroup("状态")]
        
        [LabelText("是否已初始化")]
        [ReadOnly]
        [ShowInInspector]
        [JsonProperty]
        public bool IsInitialized;
        
        [LabelText("网格版本")]
        [ReadOnly]
        [ShowInInspector]
        [JsonProperty]
        public int GridVersion = 1;

        public GridTemporaryData() : base()
        {
            IsInitialized = false;
        }

        public GridTemporaryData(string defId) : base(defId)
        {
            IsInitialized = false;
        }

        #region 占用管理

        /// <summary>
        /// 添加占用信息
        /// </summary>
        public void AddOccupation(string objectId, List<SerializableGridPosition> positions)
        {
            if (string.IsNullOrEmpty(objectId))
                throw new ArgumentException("objectId cannot be null or empty");

            if (positions == null || positions.Count == 0)
                throw new ArgumentException("positions cannot be null or empty");

            // 检查是否已存在
            if (HasObject(objectId))
            {
                RemoveOccupation(objectId);
            }

            var occupation = new GridOccupationInfo
            {
                ObjectId = objectId,
                Positions = new List<SerializableGridPosition>(positions),
                OccupiedTime = DateTime.Now,
                CellCount = positions.Count
            };
            
            OccupiedCells.Add(occupation);
            PlacedObjectCount++;
            TotalOccupiedCells += positions.Count;
            LastPlacementTime = DateTime.Now;
            LastModifyTime = DateTime.Now;
        }

        /// <summary>
        /// 移除占用信息
        /// </summary>
        public bool RemoveOccupation(string objectId)
        {
            var occupation = OccupiedCells.FirstOrDefault(o => o.ObjectId == objectId);
            if (occupation == null)
                return false;

            OccupiedCells.Remove(occupation);
            PlacedObjectCount = Math.Max(0, PlacedObjectCount - 1);
            TotalOccupiedCells = Math.Max(0, TotalOccupiedCells - occupation.CellCount);
            LastModifyTime = DateTime.Now;
            
            return true;
        }

        /// <summary>
        /// 获取物体占用的位置
        /// </summary>
        public List<SerializableGridPosition> GetObjectPositions(string objectId)
        {
            var occupation = OccupiedCells.FirstOrDefault(o => o.ObjectId == objectId);
            return occupation?.Positions ?? new List<SerializableGridPosition>();
        }

        /// <summary>
        /// 检查物体是否存在
        /// </summary>
        public bool HasObject(string objectId)
        {
            return OccupiedCells.Any(o => o.ObjectId == objectId);
        }

        /// <summary>
        /// 获取所有物体ID
        /// </summary>
        public List<string> GetAllObjectIds()
        {
            return OccupiedCells.Select(o => o.ObjectId).ToList();
        }

        /// <summary>
        /// 清空所有占用
        /// </summary>
        public void ClearAllOccupations()
        {
            OccupiedCells.Clear();
            PlacedObjectCount = 0;
            TotalOccupiedCells = 0;
            LastModifyTime = DateTime.Now;
        }

        #endregion

        #region 自定义数据管理

        /// <summary>
        /// 设置单元格自定义数据
        /// </summary>
        public void SetCellData(GridPosition position, string key, object value)
        {
            string posKey = $"{position.X}_{position.Y}_{position.Z}_{key}";
            string jsonValue = JsonConvert.SerializeObject(value);
            CellCustomData[posKey] = jsonValue;
            LastModifyTime = DateTime.Now;
        }

        /// <summary>
        /// 获取单元格自定义数据
        /// </summary>
        public T GetCellData<T>(GridPosition position, string key, T defaultValue = default)
        {
            string posKey = $"{position.X}_{position.Y}_{position.Z}_{key}";
            if (CellCustomData.TryGetValue(posKey, out string jsonValue))
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(jsonValue);
                }
                catch
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// 移除单元格自定义数据
        /// </summary>
        public bool RemoveCellData(GridPosition position, string key)
        {
            string posKey = $"{position.X}_{position.Y}_{position.Z}_{key}";
            bool removed = CellCustomData.Remove(posKey);
            if (removed)
            {
                LastModifyTime = DateTime.Now;
            }
            return removed;
        }

        /// <summary>
        /// 清空所有自定义数据
        /// </summary>
        public void ClearAllCustomData()
        {
            CellCustomData.Clear();
            LastModifyTime = DateTime.Now;
        }

        #endregion

        #region 统计和工具方法

        /// <summary>
        /// 更新统计信息
        /// </summary>
        public void UpdateStatistics(int totalCells)
        {
            AvailableCellCount = totalCells - TotalOccupiedCells;
        }

        /// <summary>
        /// 标记为已初始化
        /// </summary>
        public void MarkAsInitialized()
        {
            IsInitialized = true;
            LastModifyTime = DateTime.Now;
        }

        /// <summary>
        /// 重置所有数据
        /// </summary>
        public void Reset()
        {
            ClearAllOccupations();
            ClearAllCustomData();
            IsInitialized = false;
            AvailableCellCount = 0;
            LastPlacementTime = null;
            GridVersion++;
            LastModifyTime = DateTime.Now;
        }

        /// <summary>
        /// 获取数据摘要
        /// </summary>
        public string GetSummary()
        {
            return $"Objects: {PlacedObjectCount}, Occupied Cells: {TotalOccupiedCells}, " +
                   $"Available: {AvailableCellCount}, Last Modified: {LastModifyTime:yyyy-MM-dd HH:mm:ss}";
        }

        #endregion
    }
}