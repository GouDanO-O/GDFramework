using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Substance.Data;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Room.Data
{
    [Serializable,JsonObject]
    public class RoomTemporaryData  : ChunkTemporaryData
    {
        [LabelText("瓦片地图数据")]
        public Dictionary<string, TileData> TileMap;
        
        [LabelText("放置的物体列表")]
        public List<PlaceableObjectData> PlacedObjects;
        
        [LabelText("光照级别(0-10)")]
        [Range(0, 10)]
        public int LightLevel = 5;

        public RoomTemporaryData() : base()
        {
            TileMap = new Dictionary<string, TileData>();
            PlacedObjects = new List<PlaceableObjectData>();
            LightLevel = 5;
        }

        public RoomTemporaryData(string defId) : base(defId)
        {
            TileMap = new Dictionary<string, TileData>();
            PlacedObjects = new List<PlaceableObjectData>();
            LightLevel = 5;
        }

        /// <summary>
        /// 获取瓦片键
        /// </summary>
        public static string GetTileKey(int x, int y)
        {
            return $"{x}_{y}";
        }

        /// <summary>
        /// 获取瓦片键
        /// </summary>
        public static string GetTileKey(Vector2Int pos)
        {
            return GetTileKey(pos.x, pos.y);
        }
    }
}