using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Room;
using Core.Game.Chunk.Room.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Region.Data
{
    [Serializable]
    public class RegionDtoDef : ChunkDtoDef
    {
        [Title("玩家第一次进入区块所处的房间ID,如果为空,则默认取索引第一位"),LabelText("初始房间ID"),ReadOnly]
        public string initialPlayerLocateRoomId;

        [LabelText("第一次进入区块展示的房间"),ReadOnly]
        public List<string> initialShowingRoomIdList = new List<string>();
        
        [LabelText("区块拥有的所有房间ID"),ReadOnly]
        public List<string> roomIdList  = new List<string>();

        public RegionDtoDef()
        {
            
        }
        
        protected override string GetTypePrefix()
        {
            return "Region";
        }
    }
}