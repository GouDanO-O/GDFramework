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
            return new RoomTemporaryData(defId);
        }
        
        protected override IChunkTemporaryData LoadTemporaryDataFromES3(string instanceId)
        {
            return ES3.Load<RoomTemporaryData>(instanceId);
        }

        protected override void OnInitFromDef(IChunkDtoDef dtoDef)
        {
            base.OnInitFromDef(dtoDef);
            BuildRuntimeState();
        }

        protected override void OnInitFromInstanceId(string instanceId, IChunkDtoDef dtoDef)
        {
            base.OnInitFromInstanceId(instanceId, dtoDef);
            BuildRuntimeState();
        }

        /// <summary>
        /// 构建运行时状态 = 固定数据 + 临时数据
        /// </summary>
        private void BuildRuntimeState()
        {
            // 1. 从固定数据加载初始瓦片
            BuildRuntimeTiles();
            // 2. 从固定数据加载初始实体
            BuildRuntimeEntities();
            // 3. 应用临时数据的变化
            ApplyTemporaryDataChanges();
        }

        /// <summary>
        /// 构建运行时瓦片
        /// </summary>
        private void BuildRuntimeTiles()
        {
            
        }

        /// <summary>
        /// 构建运行时实体(从固定数据)
        /// </summary>
        private void BuildRuntimeEntities()
        {

        }

        /// <summary>
        /// 应用临时数据的变化
        /// </summary>
        private void ApplyTemporaryDataChanges()
        {

        }
        
        
        // ==================== 公共API ====================
        
        private string GetTileKey(int x, int y) => $"{x}_{y}";
        private string GetTileKey(Vector2Int pos) => $"{pos.x}_{pos.y}";
        
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
    }
}