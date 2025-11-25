using System;
using System.Collections.Generic;
using System.Linq;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Data.Interface;
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
        public new RoomDtoDef DtoDef => base.DtoDef as RoomDtoDef;
        public new RoomTemporaryData TemporaryData => base.TemporaryData as RoomTemporaryData;

        protected override IChunkTemporaryData CreateNewTemporaryData()
        {
            return new RoomTemporaryData(DefId);
        }

        protected override Type GetTemporaryDataType()
        {
            return typeof(RoomTemporaryData);
        }
        
    }
}