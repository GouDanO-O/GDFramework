using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Substance.Data;
using Core.Game.Chunk.Substance.Interface;
using Core.Game.Chunk.Tile;
using GDFrameworkExtend.JsonKit;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Room.Data
{
    [Serializable,JsonObject]
    public class RoomDtoDef : ChunkDtoDef
    {
        [LabelText("房间尺寸,默认为300x300大小")]
        [JsonConverter(typeof(Vector2IntConverter))]
        public Vector2Int GridSize = Vector2Int.one * 300;

        [LabelText("初始瓦片布局")]
        [InfoBox("定义房间的初始瓦片布局")]
        public TileData[,] InitialTiles;
        
        [LabelText("初始实体")]
        public List<TileEntityData> InitialEntities = new List<TileEntityData>();

        public override string GetTypePrefix()
        {
            return "Room";
        }

        public override void GenerateDefId()
        {
            base.GenerateDefId();
            this.InitialTiles = new TileData[GridSize.x, GridSize.y];
        }

        public void AddEntityToTile(Vector2Int tileIndex, IEntityDtoDef entityDtoDef)
        {
            
        }
    }
}