using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Tile;
using GDFrameworkExtend.Data;
using Newtonsoft.Json;
using UnityEngine;

namespace Core.Game.Chunk.Room.Data
{
    [Serializable,JsonObject]
    public class RoomTemporaryData  : ChunkTemporaryData
    {
        /// <summary>
        /// 房间中所有实体的InstanceId列表
        /// 实体通过这个列表管理,而不是通过瓦片
        /// </summary>
        public List<string> EntityInstanceIds = new List<string>();
        public RoomTemporaryData() : base() { }
        public RoomTemporaryData(string defId) : base(defId) { }
    }
}