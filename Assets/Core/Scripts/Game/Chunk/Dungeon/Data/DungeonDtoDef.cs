using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data;
using GDFrameworkExtend.JsonKit;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Dungeon.Data
{
    [Serializable,JsonObject]
    public class DungeonDtoDef : ChunkDtoDef
    {
        [JsonConverter(typeof(Vector2Converter))]
        public Vector2 InitialSpawnedPosition;
        
        [Title("玩家第一次进入区块所处的房间ID,如果为空,则默认取索引第一位"),LabelText("初始房间ID"),ReadOnly]
        public string InitialPlayerLocateRoomId;

        [LabelText("第一次进入副本展示的房间"),ReadOnly]
        public List<string> InitialShowingRoomIdList = new List<string>();
        
        [LabelText("副本拥有的所有房间ID"),ReadOnly]
        public List<string> RoomIdList  = new List<string>();
        
        public override string GetTypePrefix()
        {
            return "Dungeon";
        }
    }
}