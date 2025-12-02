using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Room.Grid
{
    /// <summary>
    /// 房间网格
    /// 管理房间内所有地块和放置物品
    /// </summary>
    [Serializable]
    public class RoomGrid
    {
        #region 事件

        /// <summary>
        /// 地块变化事件
        /// </summary>
        public event Action<TilePosition, TileData> OnTileChanged;

        /// <summary>
        /// 物品放置事件
        /// </summary>
        public event Action<PlacedObjectData> OnObjectPlaced;

        /// <summary>
        /// 物品移除事件
        /// </summary>
        public event Action<PlacedObjectData> OnObjectRemoved;

        /// <summary>
        /// 物品移动事件
        /// </summary>
        public event Action<PlacedObjectData, TilePosition, TilePosition> OnObjectMoved;

        /// <summary>
        /// 楼层切换事件
        /// </summary>
        public event Action<int, int> OnFloorChanged;

        #endregion

        #region 属性

        [LabelText("网格配置")]
        [JsonProperty]
        public RoomGridConfig Config { get; private set; }

        [LabelText("当前楼层")]
        [JsonProperty]
        public int CurrentFloor { get; private set; }

        [LabelText("楼层数据")]
        [JsonProperty]
        public List<FloorData> Floors { get; private set; } = new List<FloorData>();

        [LabelText("是否已初始化")]
        [JsonIgnore]
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// 当前楼层数据
        /// </summary>
        [JsonIgnore]
        public FloorData CurrentFloorData => Floors.Count > CurrentFloor ? Floors[CurrentFloor] : null;

        #endregion

        #region 楼层数据访问

        /// <summary>
        /// 获取指定楼层数据
        /// </summary>
        public FloorData GetFloorData(int floor)
        {
            if (floor < 0 || floor >= Floors.Count)
                return null;
            return Floors[floor];
        }
        
        #endregion

        #region 构造和初始化

        public RoomGrid()
        {
            Config = new RoomGridConfig();
        }

        public RoomGrid(RoomGridConfig config)
        {
            Config = config ?? new RoomGridConfig();
        }

        /// <summary>
        /// 初始化网格
        /// </summary>
        public void Initialize()
        {
            if (IsInitialized) return;

            // 初始化楼层
            Floors.Clear();
            for (int i = 0; i < Config.FloorCount; i++)
            {
                var floor = new FloorData(i);
                Floors.Add(floor);

                // 如果启用自动填充，填充默认地块
                if (Config.AutoFill)
                {
                    FillFloor(i, Config.DefaultTileType);
                }
            }

            CurrentFloor = 0;
            IsInitialized = true;

            Debug.Log($"[RoomGrid] 初始化完成: {Config}");
        }

        /// <summary>
        /// 从已有数据恢复
        /// </summary>
        public void RestoreFromData(RoomGridConfig config, List<FloorData> floors)
        {
            Config = config;
            Floors = floors ?? new List<FloorData>();
            CurrentFloor = 0;
            IsInitialized = true;

            Debug.Log($"[RoomGrid] 数据恢复完成: {Config}, 楼层数: {Floors.Count}");
        }

        #endregion

        #region 地块操作

        /// <summary>
        /// 获取地块
        /// </summary>
        public TileData GetTile(TilePosition position, int floor = -1)
        {
            if (floor < 0) floor = CurrentFloor;
            if (!Config.IsInBounds(position) || !Config.IsValidFloor(floor))
                return null;

            var floorData = Floors[floor];
            var key = position.ToKey();

            return floorData.Tiles.TryGetValue(key, out var tile) ? tile : null;
        }

        /// <summary>
        /// 获取地块（世界坐标）
        /// </summary>
        public TileData GetTileFromWorld(Vector3 worldPos, int floor = -1)
        {
            var tilePos = Config.WorldToTile(worldPos);
            return GetTile(tilePos, floor);
        }

        /// <summary>
        /// 设置地块
        /// </summary>
        public bool SetTile(TilePosition position, TileType type, int floor = -1)
        {
            if (floor < 0) floor = CurrentFloor;
            if (!Config.IsInBounds(position) || !Config.IsValidFloor(floor))
                return false;

            var floorData = Floors[floor];
            var key = position.ToKey();

            if (floorData.Tiles.TryGetValue(key, out var existingTile))
            {
                if (existingTile.IsLocked) return false;
                existingTile.SetType(type);
            }
            else
            {
                var newTile = new TileData(position, type);
                floorData.Tiles[key] = newTile;
                existingTile = newTile;
            }

            OnTileChanged?.Invoke(position, existingTile);
            return true;
        }

        /// <summary>
        /// 设置地块（包含高度）
        /// </summary>
        public bool SetTile(TilePosition position, TileType type, int heightLevel, int floor = -1)
        {
            if (floor < 0) floor = CurrentFloor;
            if (!Config.IsInBounds(position) || !Config.IsValidFloor(floor))
                return false;

            var floorData = Floors[floor];
            var key = position.ToKey();

            TileData tile;
            if (floorData.Tiles.TryGetValue(key, out tile))
            {
                if (tile.IsLocked) return false;
                tile.SetType(type);
                tile.HeightLevel = heightLevel;
            }
            else
            {
                tile = new TileData(position, type) { HeightLevel = heightLevel };
                floorData.Tiles[key] = tile;
            }

            OnTileChanged?.Invoke(position, tile);
            return true;
        }

        /// <summary>
        /// 删除地块
        /// </summary>
        public bool RemoveTile(TilePosition position, int floor = -1)
        {
            if (floor < 0) floor = CurrentFloor;
            if (!Config.IsValidFloor(floor))
                return false;

            var floorData = Floors[floor];
            var key = position.ToKey();

            if (floorData.Tiles.TryGetValue(key, out var tile))
            {
                if (tile.IsLocked) return false;

                // 如果有物品，先移除物品
                if (tile.HasPlacedObject)
                {
                    RemoveObject(tile.PlacedObjectId, floor);
                }

                floorData.Tiles.Remove(key);
                OnTileChanged?.Invoke(position, null);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 填充区域
        /// </summary>
        public void FillArea(TilePosition start, TilePosition end, TileType type, int floor = -1)
        {
            if (floor < 0) floor = CurrentFloor;

            int minX = Mathf.Min(start.X, end.X);
            int maxX = Mathf.Max(start.X, end.X);
            int minZ = Mathf.Min(start.Z, end.Z);
            int maxZ = Mathf.Max(start.Z, end.Z);

            for (int x = minX; x <= maxX; x++)
            {
                for (int z = minZ; z <= maxZ; z++)
                {
                    SetTile(new TilePosition(x, z), type, floor);
                }
            }
        }

        /// <summary>
        /// 填充整个楼层
        /// </summary>
        public void FillFloor(int floor, TileType type)
        {
            if (!Config.IsValidFloor(floor)) return;

            var floorData = Floors[floor];
            floorData.Tiles.Clear();

            for (int x = 0; x < Config.Width; x++)
            {
                for (int z = 0; z < Config.Depth; z++)
                {
                    var pos = new TilePosition(x, z);
                    var tile = new TileData(pos, type);
                    floorData.Tiles[pos.ToKey()] = tile;
                }
            }

            Debug.Log($"[RoomGrid] 填充楼层 {floor}: {Config.Width}x{Config.Depth} = {floorData.Tiles.Count} 地块");
        }

        /// <summary>
        /// 清空楼层
        /// </summary>
        public void ClearFloor(int floor)
        {
            if (!Config.IsValidFloor(floor)) return;

            var floorData = Floors[floor];

            // 先清除所有物品
            var objectIds = floorData.PlacedObjects.Keys.ToList();
            foreach (var id in objectIds)
            {
                RemoveObject(id, floor);
            }

            // 清除所有地块
            floorData.Tiles.Clear();
        }

        /// <summary>
        /// 洪水填充（油漆桶）
        /// </summary>
        public void FloodFill(TilePosition startPos, TileType newType, int floor = -1)
        {
            if (floor < 0) floor = CurrentFloor;
            if (!Config.IsInBounds(startPos)) return;

            var startTile = GetTile(startPos, floor);
            var originalType = startTile?.Type ?? TileType.None;

            if (originalType == newType) return;

            var queue = new Queue<TilePosition>();
            var visited = new HashSet<string>();

            queue.Enqueue(startPos);
            visited.Add(startPos.ToKey());

            while (queue.Count > 0)
            {
                var pos = queue.Dequeue();
                var tile = GetTile(pos, floor);
                var tileType = tile?.Type ?? TileType.None;

                if (tileType != originalType) continue;

                SetTile(pos, newType, floor);

                foreach (var neighbor in pos.GetNeighbors())
                {
                    if (!Config.IsInBounds(neighbor)) continue;
                    if (visited.Contains(neighbor.ToKey())) continue;

                    visited.Add(neighbor.ToKey());
                    queue.Enqueue(neighbor);
                }
            }
        }

        #endregion

        #region 物品操作

        /// <summary>
        /// 检查是否可以放置物品
        /// </summary>
        public bool CanPlaceObject(TilePosition basePosition, ObjectSize size, ObjectRotation rotation, int floor = -1)
        {
            if (floor < 0) floor = CurrentFloor;

            var actualSize = size.GetRotatedSize(rotation);

            for (int x = 0; x < actualSize.Width; x++)
            {
                for (int z = 0; z < actualSize.Depth; z++)
                {
                    var checkPos = new TilePosition(basePosition.X + x, basePosition.Z + z);

                    if (!Config.IsInBounds(checkPos))
                        return false;

                    var tile = GetTile(checkPos, floor);
                    if (tile == null || !tile.IsPlaceable)
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 放置物品
        /// </summary>
        public PlacedObjectData PlaceObject(string objectDefId, TilePosition basePosition, ObjectSize size,
            ObjectRotation rotation = ObjectRotation.Deg0, ObjectCategory category = ObjectCategory.Furniture,
            int floor = -1)
        {
            if (floor < 0) floor = CurrentFloor;

            if (!CanPlaceObject(basePosition, size, rotation, floor))
            {
                Debug.LogWarning($"[RoomGrid] 无法放置物品: {objectDefId} at {basePosition}");
                return null;
            }

            var placedObject = new PlacedObjectData(objectDefId, basePosition, size, rotation)
            {
                Category = category,
                FloorLevel = floor
            };
            placedObject.UpdateOccupiedTileKeys();

            var floorData = Floors[floor];
            floorData.PlacedObjects[placedObject.InstanceId] = placedObject;

            // 更新地块的占用状态
            foreach (var pos in placedObject.GetOccupiedPositions())
            {
                var tile = GetTile(pos, floor);
                tile?.TryPlaceObject(placedObject.InstanceId);
            }

            OnObjectPlaced?.Invoke(placedObject);
            Debug.Log($"[RoomGrid] 放置物品: {placedObject}");

            return placedObject;
        }

        /// <summary>
        /// 移除物品
        /// </summary>
        public bool RemoveObject(string instanceId, int floor = -1)
        {
            if (floor < 0) floor = CurrentFloor;
            if (!Config.IsValidFloor(floor)) return false;

            var floorData = Floors[floor];

            if (!floorData.PlacedObjects.TryGetValue(instanceId, out var placedObject))
                return false;

            // 释放地块占用
            foreach (var pos in placedObject.GetOccupiedPositions())
            {
                var tile = GetTile(pos, floor);
                tile?.RemoveObject();
            }

            floorData.PlacedObjects.Remove(instanceId);
            OnObjectRemoved?.Invoke(placedObject);

            Debug.Log($"[RoomGrid] 移除物品: {instanceId}");
            return true;
        }

        /// <summary>
        /// 移动物品
        /// </summary>
        public bool MoveObject(string instanceId, TilePosition newPosition, int floor = -1)
        {
            if (floor < 0) floor = CurrentFloor;
            if (!Config.IsValidFloor(floor)) return false;

            var floorData = Floors[floor];

            if (!floorData.PlacedObjects.TryGetValue(instanceId, out var placedObject))
                return false;

            var oldPosition = placedObject.BasePosition;

            // 先释放原位置
            foreach (var pos in placedObject.GetOccupiedPositions())
            {
                var tile = GetTile(pos, floor);
                tile?.RemoveObject();
            }

            // 检查新位置是否可用
            if (!CanPlaceObject(newPosition, placedObject.Size, placedObject.Rotation, floor))
            {
                // 恢复原位置
                foreach (var pos in placedObject.GetOccupiedPositions())
                {
                    var tile = GetTile(pos, floor);
                    tile?.TryPlaceObject(instanceId);
                }

                return false;
            }

            // 更新位置
            placedObject.BasePosition = newPosition;
            placedObject.UpdateOccupiedTileKeys();

            // 占用新位置
            foreach (var pos in placedObject.GetOccupiedPositions())
            {
                var tile = GetTile(pos, floor);
                tile?.TryPlaceObject(instanceId);
            }

            OnObjectMoved?.Invoke(placedObject, oldPosition, newPosition);
            return true;
        }

        /// <summary>
        /// 旋转物品
        /// </summary>
        public bool RotateObject(string instanceId, bool clockwise = true, int floor = -1)
        {
            if (floor < 0) floor = CurrentFloor;
            if (!Config.IsValidFloor(floor)) return false;

            var floorData = Floors[floor];

            if (!floorData.PlacedObjects.TryGetValue(instanceId, out var placedObject))
                return false;

            // 先释放原位置
            foreach (var pos in placedObject.GetOccupiedPositions())
            {
                var tile = GetTile(pos, floor);
                tile?.RemoveObject();
            }

            // 旋转
            var oldRotation = placedObject.Rotation;
            placedObject.Rotate(clockwise);

            // 检查新方向是否可用
            if (!CanPlaceObject(placedObject.BasePosition, placedObject.Size, placedObject.Rotation, floor))
            {
                // 恢复原方向
                placedObject.Rotation = oldRotation;
                placedObject.UpdateOccupiedTileKeys();

                // 恢复占用
                foreach (var pos in placedObject.GetOccupiedPositions())
                {
                    var tile = GetTile(pos, floor);
                    tile?.TryPlaceObject(instanceId);
                }

                return false;
            }

            // 占用新位置
            foreach (var pos in placedObject.GetOccupiedPositions())
            {
                var tile = GetTile(pos, floor);
                tile?.TryPlaceObject(instanceId);
            }

            return true;
        }

        /// <summary>
        /// 获取物品
        /// </summary>
        public PlacedObjectData GetObject(string instanceId, int floor = -1)
        {
            if (floor < 0) floor = CurrentFloor;
            if (!Config.IsValidFloor(floor)) return null;

            return Floors[floor].PlacedObjects.TryGetValue(instanceId, out var obj) ? obj : null;
        }

        /// <summary>
        /// 获取位置上的物品
        /// </summary>
        public PlacedObjectData GetObjectAtPosition(TilePosition position, int floor = -1)
        {
            if (floor < 0) floor = CurrentFloor;

            var tile = GetTile(position, floor);
            if (tile == null || !tile.HasPlacedObject) return null;

            return GetObject(tile.PlacedObjectId, floor);
        }

        /// <summary>
        /// 获取楼层所有物品
        /// </summary>
        public List<PlacedObjectData> GetAllObjects(int floor = -1)
        {
            if (floor < 0) floor = CurrentFloor;
            if (!Config.IsValidFloor(floor)) return new List<PlacedObjectData>();

            return Floors[floor].PlacedObjects.Values.ToList();
        }

        #endregion

        #region 楼层操作

        /// <summary>
        /// 切换楼层
        /// </summary>
        public bool SwitchFloor(int floor)
        {
            if (!Config.IsValidFloor(floor) || floor == CurrentFloor)
                return false;

            var oldFloor = CurrentFloor;
            CurrentFloor = floor;

            OnFloorChanged?.Invoke(oldFloor, floor);
            Debug.Log($"[RoomGrid] 切换楼层: {oldFloor + 1}F -> {floor + 1}F");

            return true;
        }

        /// <summary>
        /// 上一层
        /// </summary>
        public bool GoUpFloor()
        {
            return SwitchFloor(CurrentFloor + 1);
        }

        /// <summary>
        /// 下一层
        /// </summary>
        public bool GoDownFloor()
        {
            return SwitchFloor(CurrentFloor - 1);
        }

        #endregion

        #region 查询方法

        /// <summary>
        /// 获取指定类型的所有地块
        /// </summary>
        public List<TileData> GetTilesByType(TileType type, int floor = -1)
        {
            if (floor < 0) floor = CurrentFloor;
            if (!Config.IsValidFloor(floor)) return new List<TileData>();

            return Floors[floor].Tiles.Values.Where(t => t.Type == type).ToList();
        }

        /// <summary>
        /// 获取可行走的地块
        /// </summary>
        public List<TileData> GetWalkableTiles(int floor = -1)
        {
            if (floor < 0) floor = CurrentFloor;
            if (!Config.IsValidFloor(floor)) return new List<TileData>();

            return Floors[floor].Tiles.Values.Where(t => t.IsWalkable).ToList();
        }

        /// <summary>
        /// 获取可放置的地块
        /// </summary>
        public List<TileData> GetPlaceableTiles(int floor = -1)
        {
            if (floor < 0) floor = CurrentFloor;
            if (!Config.IsValidFloor(floor)) return new List<TileData>();

            return Floors[floor].Tiles.Values.Where(t => t.IsPlaceable).ToList();
        }

        /// <summary>
        /// 获取统计信息
        /// </summary>
        public RoomGridStatistics GetStatistics(int floor = -1)
        {
            if (floor < 0) floor = CurrentFloor;
            if (!Config.IsValidFloor(floor)) return new RoomGridStatistics();

            var floorData = Floors[floor];
            return new RoomGridStatistics
            {
                TotalTiles = floorData.Tiles.Count,
                WalkableTiles = floorData.Tiles.Values.Count(t => t.IsWalkable),
                PlaceableTiles = floorData.Tiles.Values.Count(t => t.IsPlaceable),
                OccupiedTiles = floorData.Tiles.Values.Count(t => t.HasPlacedObject),
                PlacedObjects = floorData.PlacedObjects.Count
            };
        }
        
        

        #endregion
    }
}