using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Tile;
using GDFrameworkExtend.Data;
using Newtonsoft.Json;

namespace Core.Game.Chunk.Room.Data
{
    [Serializable,JsonObject]
    public class RoomTemporaryData  : ChunkTemporaryData
    {
        public TileData[,] tileData;
    }
}