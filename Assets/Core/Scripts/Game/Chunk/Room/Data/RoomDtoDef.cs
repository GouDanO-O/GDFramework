using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Tile;
using Core.Game.Chunk.Tile.Entity;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Room.Data
{
    [Serializable,JsonObject]
    public class RoomDtoDef : ChunkDtoDef
    {
        [LabelText("房间尺寸,默认为300x300大小")]
        public Vector2Int GridSize = Vector2Int.one * 300;
        
        [LabelText("初始瓦片布局")]
        [InfoBox("定义房间的初始瓦片布局")]
        public List<TileDefinition> InitialTiles = new List<TileDefinition>();
        
        [LabelText("初始实体")]
        public List<TileEntity> InitialEntities = new List<TileEntity>();

        public override string GetTypePrefix()
        {
            return "Room";
        }
    }
}