using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Tile;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Room.Data
{
    [Serializable]
    public class RoomDtoDef : ChunkDtoDef
    {
        [LabelText("房间尺寸,默认为300x300大小")]
        public Vector2Int roomSize = Vector2Int.one * 300;

        [LabelText("房间中的瓦片类型")]
        public TileDtoDef[,] roomTiles;

        public RoomDtoDef()
        {
            
        }

        protected override string GetTypePrefix()
        {
            return "Room";
        }
    }
}