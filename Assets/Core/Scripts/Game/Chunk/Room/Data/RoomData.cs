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
        protected override IChunkTemporaryData CreateNewTemporaryData()
        {
            throw new NotImplementedException();
        }

        protected override Type GetTemporaryDataType()
        {
            throw new NotImplementedException();
        }
    }
}