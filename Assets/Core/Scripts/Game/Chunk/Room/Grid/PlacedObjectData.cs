using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Room.Grid
{
 /// <summary>
    /// 放置在地块上的物品数据
    /// </summary>
    [Serializable]
    public class PlacedObjectData
    {
        [LabelText("实例ID")]
        [ReadOnly]
        [JsonProperty]
        public string InstanceId;

        [LabelText("物品定义ID")]
        [JsonProperty]
        public string ObjectDefId;

        [LabelText("物品类别")]
        [JsonProperty]
        public ObjectCategory Category;

        [LabelText("基准位置")]
        [PropertyTooltip("物品左下角所在的地块坐标")]
        [JsonProperty]
        public TilePosition BasePosition;

        [LabelText("楼层")]
        [JsonProperty]
        public int FloorLevel = 0;

        [LabelText("旋转")]
        [JsonProperty]
        public ObjectRotation Rotation = ObjectRotation.Deg0;

        [LabelText("尺寸")]
        [JsonProperty]
        public ObjectSize Size = ObjectSize.One;

        [LabelText("占用的地块")]
        [ReadOnly]
        [JsonProperty]
        public List<string> OccupiedTileKeys = new List<string>();

        [LabelText("放置时间")]
        [JsonProperty]
        public DateTime PlacedTime;

        [LabelText("自定义数据")]
        [JsonProperty]
        public Dictionary<string, string> CustomData = new Dictionary<string, string>();

        /// <summary>
        /// 获取旋转后的实际尺寸
        /// </summary>
        [JsonIgnore]
        public ObjectSize ActualSize => Size.GetRotatedSize(Rotation);

        /// <summary>
        /// 获取旋转角度（度）
        /// </summary>
        [JsonIgnore]
        public float RotationDegrees => (float)Rotation;

        /// <summary>
        /// 获取旋转四元数
        /// </summary>
        [JsonIgnore]
        public Quaternion RotationQuaternion => Quaternion.Euler(0, RotationDegrees, 0);

        public PlacedObjectData()
        {
            InstanceId = GenerateInstanceId();
            PlacedTime = DateTime.Now;
        }

        public PlacedObjectData(string objectDefId, TilePosition basePosition, ObjectSize size, ObjectRotation rotation = ObjectRotation.Deg0)
        {
            InstanceId = GenerateInstanceId();
            ObjectDefId = objectDefId;
            BasePosition = basePosition;
            Size = size;
            Rotation = rotation;
            PlacedTime = DateTime.Now;
        }

        /// <summary>
        /// 生成实例ID
        /// </summary>
        private static string GenerateInstanceId()
        {
            return $"OBJ_{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
        }

        /// <summary>
        /// 获取物品占用的所有地块位置
        /// </summary>
        public List<TilePosition> GetOccupiedPositions()
        {
            var positions = new List<TilePosition>();
            var actualSize = ActualSize;

            for (int x = 0; x < actualSize.Width; x++)
            {
                for (int z = 0; z < actualSize.Depth; z++)
                {
                    positions.Add(new TilePosition(BasePosition.X + x, BasePosition.Z + z));
                }
            }

            return positions;
        }

        /// <summary>
        /// 更新占用的地块Keys
        /// </summary>
        public void UpdateOccupiedTileKeys()
        {
            OccupiedTileKeys.Clear();
            foreach (var pos in GetOccupiedPositions())
            {
                OccupiedTileKeys.Add(pos.ToKey());
            }
        }

        /// <summary>
        /// 检查是否占用指定位置
        /// </summary>
        public bool OccupiesPosition(TilePosition position)
        {
            return OccupiedTileKeys.Contains(position.ToKey());
        }

        /// <summary>
        /// 获取世界坐标位置（中心点）
        /// </summary>
        public Vector3 GetWorldPosition(float tileSize = 1f, float baseHeight = 0f)
        {
            var actualSize = ActualSize;
            float centerX = (BasePosition.X + actualSize.Width * 0.5f) * tileSize;
            float centerZ = (BasePosition.Z + actualSize.Depth * 0.5f) * tileSize;
            float y = baseHeight + FloorLevel * 3f; // 假设每层楼高3米
            
            return new Vector3(centerX, y, centerZ);
        }

        /// <summary>
        /// 旋转物品
        /// </summary>
        public void Rotate(bool clockwise = true)
        {
            int currentDeg = (int)Rotation;
            currentDeg += clockwise ? 90 : -90;
            
            if (currentDeg >= 360) currentDeg -= 360;
            if (currentDeg < 0) currentDeg += 360;
            
            Rotation = (ObjectRotation)currentDeg;
            UpdateOccupiedTileKeys();
        }

        /// <summary>
        /// 设置自定义数据
        /// </summary>
        public void SetCustomData(string key, string value)
        {
            CustomData[key] = value;
        }

        /// <summary>
        /// 获取自定义数据
        /// </summary>
        public string GetCustomData(string key, string defaultValue = null)
        {
            return CustomData.TryGetValue(key, out var value) ? value : defaultValue;
        }

        /// <summary>
        /// 克隆物品数据（生成新的实例ID）
        /// </summary>
        public PlacedObjectData Clone()
        {
            var clone = new PlacedObjectData
            {
                ObjectDefId = ObjectDefId,
                Category = Category,
                BasePosition = BasePosition,
                FloorLevel = FloorLevel,
                Rotation = Rotation,
                Size = Size,
                CustomData = new Dictionary<string, string>(CustomData)
            };
            clone.UpdateOccupiedTileKeys();
            return clone;
        }

        public override string ToString()
        {
            return $"Object[{InstanceId}] Def:{ObjectDefId} Pos:{BasePosition} Size:{ActualSize} Rot:{Rotation}";
        }
    }
}