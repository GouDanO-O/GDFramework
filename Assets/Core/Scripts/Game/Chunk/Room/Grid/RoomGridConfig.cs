using System;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Room.Grid
{
    /// <summary>
    /// 房间网格配置
    /// 定义房间的基础网格参数
    /// </summary>
    [Serializable]
    public class RoomGridConfig
    {
        [Title("尺寸设置")]
        
        [LabelText("网格宽度(X轴格数)")]
        [MinValue(5)]
        [MaxValue(200)]
        [JsonProperty]
        public int Width = 50;

        [LabelText("网格深度(Z轴格数)")]
        [MinValue(5)]
        [MaxValue(200)]
        [JsonProperty]
        public int Depth = 50;

        [LabelText("单个地块大小(米)")]
        [MinValue(0.5f)]
        [MaxValue(5f)]
        [JsonProperty]
        public float TileSize = 1f;

        [Title("楼层设置")]
        
        [LabelText("楼层数量")]
        [MinValue(1)]
        [MaxValue(10)]
        [JsonProperty]
        public int FloorCount = 1;

        [LabelText("层高(米)")]
        [MinValue(2f)]
        [MaxValue(10f)]
        [JsonProperty]
        public float FloorHeight = 3f;

        [Title("默认设置")]
        
        [LabelText("默认地块类型")]
        [JsonProperty]
        public TileType DefaultTileType = TileType.Grass;

        [LabelText("自动填充")]
        [PropertyTooltip("创建时是否自动填充所有地块")]
        [JsonProperty]
        public bool AutoFill = true;

        [Title("边界设置")]
        
        [LabelText("启用边界墙")]
        [JsonProperty]
        public bool EnableBoundaryWalls = false;

        [LabelText("边界墙厚度")]
        [ShowIf("EnableBoundaryWalls")]
        [MinValue(1)]
        [JsonProperty]
        public int BoundaryWallThickness = 1;

        /// <summary>
        /// 总格子数
        /// </summary>
        [JsonIgnore]
        public int TotalTileCount => Width * Depth;

        /// <summary>
        /// 世界尺寸(米)
        /// </summary>
        [JsonIgnore]
        public Vector2 WorldSize => new Vector2(Width * TileSize, Depth * TileSize);

        /// <summary>
        /// 中心点世界坐标
        /// </summary>
        [JsonIgnore]
        public Vector3 WorldCenter => new Vector3(Width * TileSize * 0.5f, 0, Depth * TileSize * 0.5f);

        public RoomGridConfig()
        {
            Width = 50;
            Depth = 50;
            TileSize = 1f;
            FloorCount = 1;
            FloorHeight = 3f;
            DefaultTileType = TileType.Grass;
            AutoFill = true;
        }

        public RoomGridConfig(int width, int depth, float tileSize = 1f)
        {
            Width = width;
            Depth = depth;
            TileSize = tileSize;
            FloorCount = 1;
            FloorHeight = 3f;
            DefaultTileType = TileType.Grass;
            AutoFill = true;
        }

        /// <summary>
        /// 检查坐标是否在范围内
        /// </summary>
        public bool IsInBounds(TilePosition position)
        {
            return position.X >= 0 && position.X < Width &&
                   position.Z >= 0 && position.Z < Depth;
        }

        /// <summary>
        /// 检查坐标是否在范围内
        /// </summary>
        public bool IsInBounds(int x, int z)
        {
            return x >= 0 && x < Width && z >= 0 && z < Depth;
        }

        /// <summary>
        /// 检查楼层是否有效
        /// </summary>
        public bool IsValidFloor(int floor)
        {
            return floor >= 0 && floor < FloorCount;
        }

        /// <summary>
        /// 世界坐标转地块坐标
        /// </summary>
        public TilePosition WorldToTile(Vector3 worldPos)
        {
            return new TilePosition(
                Mathf.FloorToInt(worldPos.x / TileSize),
                Mathf.FloorToInt(worldPos.z / TileSize)
            );
        }

        /// <summary>
        /// 地块坐标转世界坐标（中心点）
        /// </summary>
        public Vector3 TileToWorld(TilePosition tilePos, int floor = 0)
        {
            return new Vector3(
                (tilePos.X + 0.5f) * TileSize,
                floor * FloorHeight,
                (tilePos.Z + 0.5f) * TileSize
            );
        }

        /// <summary>
        /// 获取世界边界
        /// </summary>
        public Bounds GetWorldBounds()
        {
            var size = new Vector3(Width * TileSize, FloorCount * FloorHeight, Depth * TileSize);
            var center = size * 0.5f;
            return new Bounds(center, size);
        }

        /// <summary>
        /// 验证配置
        /// </summary>
        public bool Validate(out string error)
        {
            if (Width < 5 || Width > 200)
            {
                error = "网格宽度必须在5-200之间";
                return false;
            }

            if (Depth < 5 || Depth > 200)
            {
                error = "网格深度必须在5-200之间";
                return false;
            }

            if (TileSize < 0.5f || TileSize > 5f)
            {
                error = "地块大小必须在0.5-5之间";
                return false;
            }

            if (FloorCount < 1 || FloorCount > 10)
            {
                error = "楼层数量必须在1-10之间";
                return false;
            }

            error = string.Empty;
            return true;
        }

        /// <summary>
        /// 克隆配置
        /// </summary>
        public RoomGridConfig Clone()
        {
            return new RoomGridConfig
            {
                Width = Width,
                Depth = Depth,
                TileSize = TileSize,
                FloorCount = FloorCount,
                FloorHeight = FloorHeight,
                DefaultTileType = DefaultTileType,
                AutoFill = AutoFill,
                EnableBoundaryWalls = EnableBoundaryWalls,
                BoundaryWallThickness = BoundaryWallThickness
            };
        }

        public override string ToString()
        {
            return $"RoomGridConfig[{Width}x{Depth}] TileSize:{TileSize}m Floors:{FloorCount}";
        }
    }
}