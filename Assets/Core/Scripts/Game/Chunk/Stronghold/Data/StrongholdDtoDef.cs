using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Stronghold.Data
{
    public class StrongholdDtoDef : ChunkDtoDef
    {
        [Title("玩家第一次进入区块所处的房间ID,如果为空,则默认取索引第一位"),LabelText("初始房间ID"),ReadOnly]
        public string initialPlayerLocateRoomId;

        [LabelText("第一次进入副本展示的房间"),ReadOnly]
        public List<string> initialShowingRoomIdList = new List<string>();
        
        [LabelText("副本拥有的所有房间ID"),ReadOnly]
        public List<string> roomIdList  = new List<string>();

        public StrongholdDtoDef()
        {
            
        }
        
        protected override string GetTypePrefix()
        {
            return "Stronghold";
        }
    }
}