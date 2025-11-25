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

        public RoomTemporaryData() : base()
        {

        }

        public RoomTemporaryData(string defId) : base(defId)
        {

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