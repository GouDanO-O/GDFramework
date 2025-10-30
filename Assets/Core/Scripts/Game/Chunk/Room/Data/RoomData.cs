using System;
using System.Collections.Generic;
using System.Linq;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Data.Interface;
using Core.Game.Chunk.Substance.Data;
using Core.Game.Chunk.Tile;
using UnityEngine;

namespace Core.Game.Chunk.Room.Data
{
    /// <summary>
    /// 房间
    /// 房间里面存有所持有的所有格子块
    /// 包括格子块上的放置的物体
    /// </summary>
    public class RoomData : ChunkData
    {
        public RoomDtoDef RoomDef => DtoDef as RoomDtoDef;
        public RoomTemporaryData RoomTempData => TemporaryData as RoomTemporaryData;

        // Entity配置缓存
        private Dictionary<string, EntityDtoDef> _entityDefCache = new Dictionary<string, EntityDtoDef>();

        protected override IChunkTemporaryData CreateTemporaryData(string defId)
        {
            var tempData = new RoomTemporaryData(defId);

            // 第一次创建: 从固定数据复制到临时数据
            InitializeFromDef(tempData);
            return tempData;
        }

        protected override IChunkTemporaryData LoadTemporaryDataFromES3(string instanceId)
        {
            // 之后加载: 直接读取临时数据,不再看固定数据
            return ES3.Load<RoomTemporaryData>(instanceId);
        }

        /// <summary>
        /// 第一次创建时:从固定数据初始化临时数据
        /// </summary>
        private void InitializeFromDef(RoomTemporaryData tempData)
        {
            if (RoomDef == null)
                return;

            // 1. 复制瓦片布局
            foreach (var tileDef in RoomDef.InitialTiles)
            {
                string key = GetTileKey(tileDef.Position);
                tempData.Tiles[key] = new TileData()
                {
                    Position = tileDef.Position,
                    TileType = tileDef.TileType,
                    IsWalkable = tileDef.IsWalkable,
                };
            }

            // 如果没有定义瓦片,生成默认的Ground瓦片
            if (RoomDef.InitialTiles.Count == 0)
            {
                for (int x = 0; x < RoomDef.GridSize.x; x++)
                {
                    for (int y = 0; y < RoomDef.GridSize.y; y++)
                    {
                        string key = GetTileKey(x, y);
                        tempData.Tiles[key] = new TileData()
                        {
                            Position = new Vector2Int(x, y),
                            TileType = ETileType.Ground,
                            IsWalkable = true,
                        };
                    }
                }
            }

            // 2. 复制实体布局
            foreach (var entityDef in RoomDef.InitialEntities)
            {
                // 为每个实体生成唯一ID
                string entityInstanceId = GenerateEntityInstanceId();

                // 加载实体配置
                var entityDefData = GetOrLoadEntityDef(entityDef.DefId);
                if (entityDefData == null)
                {
                    Debug.LogWarning($"找不到实体配置: {entityDef.DefId}");
                    continue;
                }

                tempData.Entities[entityInstanceId] = new EntityData()
                {
                    EntityTempData =
                    {
                        InstanceId = entityInstanceId,
                        DefId = entityDef.DefId,
                        Position = entityDef.Position,
                        RotationType = entityDef.RotationType,
                        Health = entityDefData.InitialHealth,
                        MaxHealth = entityDefData.InitialMaxHealth,
                        IsDestroyed = false,
                        CreateTime = DateTime.Now,
                        LastModifyTime = DateTime.Now
                    }
 
                };
            }
        }

        private string GenerateEntityInstanceId()
        {
            return $"ENT_{Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper()}";
        }

        private string GetTileKey(int x, int y) => $"{x}_{y}";
        private string GetTileKey(Vector2Int pos) => $"{pos.x}_{pos.y}";

        // ==================== 瓦片操作 ====================

        /// <summary>
        /// 获取瓦片
        /// </summary>
        public TileData GetTile(Vector2Int pos)
        {
            return RoomTempData.Tiles.TryGetValue(GetTileKey(pos), out var tile) ? tile : null;
        }

        public List<TileData> GetAllTiles()
        {
            return new List<TileData>(RoomTempData.Tiles.Values);
        }

        /// <summary>
        /// 修改瓦片类型
        /// </summary>
        public void SetTileType(Vector2Int pos, ETileType tileType)
        {
            var tile = GetTile(pos);
            if (tile != null)
            {
                tile.TileType = tileType;
                tile.IsWalkable = tileType == ETileType.Ground;
                SaveTemporaryData();
            }
        }

        /// <summary>
        /// 设置瓦片可行走性
        /// </summary>
        public void SetTileWalkable(Vector2Int pos, bool walkable)
        {
            var tile = GetTile(pos);
            if (tile != null)
            {
                tile.IsWalkable = walkable;
                SaveTemporaryData();
            }
        }

        /// <summary>
        /// 探索瓦片
        /// </summary>
        public void ExploreTile(Vector2Int pos)
        {
            var tile = GetTile(pos);
            if (tile != null)
            {
                SaveTemporaryData();
            }
        }

        // ==================== 实体操作 ====================

        /// <summary>
        /// 获取实体
        /// </summary>
        public EntityData GetEntity(string entityInstanceId)
        {
            return RoomTempData.Entities.TryGetValue(entityInstanceId, out var entity) ? entity : null;
        }

        /// <summary>
        /// 获取指定位置的实体
        /// </summary>
        public EntityData GetEntityAt(Vector2Int pos)
        {
            foreach (var entity in RoomTempData.Entities.Values)
            {
                if (entity.EntityTempData.Position == pos && !entity.EntityTempData.IsDestroyed)
                    return entity;
            }

            return null;
        }

        public List<EntityData> GetAllEntities()
        {
            return new List<EntityData>(RoomTempData.Entities.Values);
        }

        /// <summary>
        /// 获取活跃的实体(未被摧毁)
        /// </summary>
        public List<EntityData> GetActiveEntities()
        {
            return RoomTempData.Entities.Values
                .Where(e => !e.EntityTempData.IsDestroyed)
                .ToList();
        }

        /// <summary>
        /// 玩家生成新实体(放置建筑等)
        /// </summary>
        public string SpawnEntity(Vector2Int pos, string entityDefId,
            EEntityRotationType rotation = EEntityRotationType.Up)
        {
            var tile = GetTile(pos);
            if (tile == null || !tile.IsWalkable)
            {
                Debug.LogWarning($"无法在此位置生成实体: {pos}");
                return null;
            }

            if (GetEntityAt(pos) != null)
            {
                Debug.LogWarning($"位置已有实体: {pos}");
                return null;
            }

            var entityDefData = GetOrLoadEntityDef(entityDefId);
            if (entityDefData == null)
            {
                Debug.LogError($"找不到实体配置: {entityDefId}");
                return null;
            }

            string entityInstanceId = GenerateEntityInstanceId();

            RoomTempData.Entities[entityInstanceId] = new EntityData()
            {
                EntityTempData =
                {
                    InstanceId = entityInstanceId,
                    DefId = entityDefId,
                    Position = pos,
                    RotationType = rotation,
                    Health = entityDefData.InitialHealth,
                    MaxHealth = entityDefData.InitialMaxHealth,
                    IsDestroyed = false,
                    CreateTime = DateTime.Now,
                    LastModifyTime = DateTime.Now
                }

            };

            SaveTemporaryData();
            return entityInstanceId;
        }

        /// <summary>
        /// 移除实体(砍树、拾取等)
        /// </summary>
        public bool RemoveEntity(string entityInstanceId)
        {
            if (!RoomTempData.Entities.ContainsKey(entityInstanceId))
                return false;

            // 直接从字典中移除
            RoomTempData.Entities.Remove(entityInstanceId);

            SaveTemporaryData();
            return true;
        }

        /// <summary>
        /// 对实体造成伤害
        /// </summary>
        public bool DamageEntity(string entityInstanceId, int damage)
        {
            var entity = GetEntity(entityInstanceId);
            if (entity == null || entity.EntityTempData.IsDestroyed)
                return false;

            entity.EntityTempData.Health -= damage;
            entity.EntityTempData.LastModifyTime = DateTime.Now;

            if (entity.EntityTempData.Health <= 0)
            {
                entity.EntityTempData.Health = 0;
                entity.EntityTempData.IsDestroyed = true;
            }

            SaveTemporaryData();
            return true;
        }

        /// <summary>
        /// 修复实体
        /// </summary>
        public bool RepairEntity(string entityInstanceId, int amount)
        {
            var entity = GetEntity(entityInstanceId);
            if (entity == null || entity.EntityTempData.IsDestroyed)
                return false;

            entity.EntityTempData.Health = Mathf.Min(entity.EntityTempData.Health + amount, entity.EntityTempData.MaxHealth);
            entity.EntityTempData.LastModifyTime = DateTime.Now;

            SaveTemporaryData();
            return true;
        }

        /// <summary>
        /// 移动实体
        /// </summary>
        public bool MoveEntity(string entityInstanceId, Vector2Int newPos)
        {
            var entity = GetEntity(entityInstanceId);
            if (entity == null)
                return false;

            var newTile = GetTile(newPos);
            if (newTile == null || !newTile.IsWalkable)
                return false;

            if (GetEntityAt(newPos) != null)
                return false;

            entity.EntityTempData.Position = newPos;
            entity.EntityTempData.LastModifyTime = DateTime.Now;

            SaveTemporaryData();
            return true;
        }

        /// <summary>
        /// 旋转实体
        /// </summary>
        public bool RotateEntity(string entityInstanceId, EEntityRotationType newRotation)
        {
            var entity = GetEntity(entityInstanceId);
            if (entity == null)
                return false;

            entity.EntityTempData.RotationType = newRotation;
            entity.EntityTempData.LastModifyTime = DateTime.Now;

            SaveTemporaryData();
            return true;
        }

        /// <summary>
        /// 获取实体配置
        /// </summary>
        public EntityDtoDef GetEntityDef(string entityInstanceId)
        {
            var entity = GetEntity(entityInstanceId);
            if (entity == null)
                return null;

            return GetOrLoadEntityDef(entity.EntityTempData.DefId);
        }

        private EntityDtoDef GetOrLoadEntityDef(string entityDefId)
        {
            if (_entityDefCache.TryGetValue(entityDefId, out var cached))
                return cached;

            var def = ChunkDtoDef.LoadDefFromJson<EntityDtoDef>(entityDefId);
            if (def != null)
            {
                _entityDefCache[entityDefId] = def;
            }

            return def;
        }

        // ==================== 房间状态 ====================

        public void SetCleared(bool cleared)
        {
            RoomTempData.IsCleared = cleared;
            SaveTemporaryData();
        }

        public void SetLocked(bool locked)
        {
            RoomTempData.IsLocked = locked;
            SaveTemporaryData();
        }

        public void SetDiscovered(bool discovered)
        {
            RoomTempData.IsDiscovered = discovered;
            SaveTemporaryData();
        }
    }
}