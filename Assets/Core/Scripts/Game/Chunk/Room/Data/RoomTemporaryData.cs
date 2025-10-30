using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Substance.Data;
using Core.Game.Chunk.Tile;
using Newtonsoft.Json;

namespace Core.Game.Chunk.Room.Data
{
    [Serializable,JsonObject]
    public class RoomTemporaryData  : ChunkTemporaryData
    {
        /// <summary>
        /// 所有瓦片的运行时状态(完整数据)
        /// Key: "x_y"
        /// </summary>
        public Dictionary<string, TileData> Tiles = new Dictionary<string, TileData>();
        
        /// <summary>
        /// 所有实体的运行时状态(完整数据)
        /// Key: 实体的唯一ID
        /// </summary>
        public Dictionary<string, EntityData> Entities = new Dictionary<string, EntityData>();
        
        /// <summary>
        /// 房间状态
        /// </summary>
        public bool IsCleared = false;
        public bool IsLocked = false;
        public bool IsDiscovered = false;
        
        public RoomTemporaryData() : base() { }
        public RoomTemporaryData(string defId) : base(defId) { }
    }
}