using System;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Room.Grid
{
/// <summary>
    /// 单个地块的数据
    /// </summary>
    [Serializable]
    public class TileData
    {
        [LabelText("位置")]
        [JsonProperty]
        public TilePosition Position;

        [LabelText("地块类型")]
        [JsonProperty]
        public TileType Type = TileType.None;

        [LabelText("高度等级")]
        [PropertyTooltip("用于悬崖、台阶等地形，0为地面")]
        [MinValue(0)]
        [JsonProperty]
        public int HeightLevel = 0;

        [LabelText("标记")]
        [JsonProperty]
        public TileFlags Flags = TileFlags.Walkable | TileFlags.Placeable;

        [LabelText("自定义地块ID")]
        [ShowIf("Type", TileType.Custom)]
        [JsonProperty]
        public string CustomTileId;

        [LabelText("放置的物品ID")]
        [ReadOnly]
        [JsonProperty]
        public string PlacedObjectId;

        /// <summary>
        /// 实际高度（米）
        /// </summary>
        [JsonIgnore]
        public float WorldHeight => HeightLevel * 0.5f; // 每级高度0.5米

        /// <summary>
        /// 是否可行走
        /// </summary>
        [JsonIgnore]
        public bool IsWalkable => HasFlag(TileFlags.Walkable) && string.IsNullOrEmpty(PlacedObjectId);

        /// <summary>
        /// 是否可放置物品
        /// </summary>
        [JsonIgnore]
        public bool IsPlaceable => HasFlag(TileFlags.Placeable) && string.IsNullOrEmpty(PlacedObjectId);

        /// <summary>
        /// 是否有物品
        /// </summary>
        [JsonIgnore]
        public bool HasObject => !string.IsNullOrEmpty(PlacedObjectId);

        /// <summary>
        /// 是否已锁定
        /// </summary>
        [JsonIgnore]
        public bool IsLocked => HasFlag(TileFlags.Locked);

        public TileData()
        {
            Position = TilePosition.Zero;
            Type = TileType.None;
            HeightLevel = 0;
            Flags = TileFlags.Walkable | TileFlags.Placeable;
        }

        public TileData(TilePosition position, TileType type = TileType.Grass)
        {
            Position = position;
            Type = type;
            HeightLevel = 0;
            SetDefaultFlags(type);
        }

        /// <summary>
        /// 根据地块类型设置默认标记
        /// </summary>
        public void SetDefaultFlags(TileType type)
        {
            switch (type)
            {
                case TileType.None:
                    Flags = TileFlags.None;
                    break;
                case TileType.Water:
                    Flags = TileFlags.Swimmable;
                    break;
                case TileType.Lava:
                    Flags = TileFlags.Damaging;
                    break;
                case TileType.Ice:
                    Flags = TileFlags.Walkable | TileFlags.Slippery;
                    break;
                case TileType.Sand:
                    Flags = TileFlags.Walkable | TileFlags.Placeable | TileFlags.SlowDown;
                    break;
                default:
                    Flags = TileFlags.Walkable | TileFlags.Placeable;
                    break;
            }
        }

        /// <summary>
        /// 设置地块类型
        /// </summary>
        public void SetType(TileType type, bool updateFlags = true)
        {
            if (IsLocked) return;
            
            Type = type;
            if (updateFlags)
            {
                SetDefaultFlags(type);
            }
        }

        /// <summary>
        /// 检查是否有指定标记
        /// </summary>
        public bool HasFlag(TileFlags flag)
        {
            return (Flags & flag) == flag;
        }

        /// <summary>
        /// 添加标记
        /// </summary>
        public void AddFlag(TileFlags flag)
        {
            Flags |= flag;
        }

        /// <summary>
        /// 移除标记
        /// </summary>
        public void RemoveFlag(TileFlags flag)
        {
            Flags &= ~flag;
        }

        /// <summary>
        /// 放置物品
        /// </summary>
        public bool TryPlaceObject(string objectId)
        {
            if (!IsPlaceable || IsLocked)
                return false;

            PlacedObjectId = objectId;
            return true;
        }

        /// <summary>
        /// 移除物品
        /// </summary>
        public void RemoveObject()
        {
            if (IsLocked) return;
            PlacedObjectId = null;
        }

        /// <summary>
        /// 锁定地块
        /// </summary>
        public void Lock()
        {
            AddFlag(TileFlags.Locked);
        }

        /// <summary>
        /// 解锁地块
        /// </summary>
        public void Unlock()
        {
            RemoveFlag(TileFlags.Locked);
        }

        /// <summary>
        /// 重置地块
        /// </summary>
        public void Reset()
        {
            if (IsLocked) return;
            
            Type = TileType.None;
            HeightLevel = 0;
            Flags = TileFlags.None;
            PlacedObjectId = null;
            CustomTileId = null;
        }

        /// <summary>
        /// 克隆地块数据
        /// </summary>
        public TileData Clone()
        {
            return new TileData
            {
                Position = Position,
                Type = Type,
                HeightLevel = HeightLevel,
                Flags = Flags,
                CustomTileId = CustomTileId,
                PlacedObjectId = PlacedObjectId
            };
        }

        public override string ToString()
        {
            return $"Tile[{Position}] Type:{Type} Height:{HeightLevel} Flags:{Flags}";
        }
    }
}