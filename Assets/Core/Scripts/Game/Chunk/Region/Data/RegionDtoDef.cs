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
        [Title("玩家第一次进入区块所处的房间ID,如果为空,则默认取索引第一位"),LabelText("初始房间ID")]
        public string initialRoomId;
        
#if UNITY_EDITOR
        [LabelText("(编辑期引用)初始房间"), Tooltip("仅编辑期辅助,构建/运行期请使用 initialRoomId")]
        public RoomDto initialRoomDtoRef;
#endif

        [Title("当玩家进入又离开区块时,是否需要缓存当前所处房间ID,如果不缓存,则每次进入都进入初始房间,否则进入历史房间"),LabelText("是否缓存当前房间id")]
        public bool willCacheLocateRoomIdWhenPlayerEntersAndLeavesAreaBlock;
        
        [LabelText("区块里面的房间")]
        public List<RoomDto> roomDatas = new List<RoomDto>();
        
        [LabelText("区块拥有的所有房间ID"),ReadOnly]
        public List<string> roomIds  = new List<string>();

        public RegionDtoDef(string parentChunkId, string thisChunkId) : base(parentChunkId, thisChunkId)
        {
            
        }
    }
}