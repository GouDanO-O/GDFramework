using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Node;
using Core.Game.Chunk.Node.Data;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Room.Data
{
    [Serializable]
    public class RoomDtoDef : ChunkDtoDef
    {
        [LabelText("第一次进入房间展示的节点"), ReadOnly]
        public List<string> initialShowingNodeList = new List<string>();
        
        [LabelText("房间里面拥有的节点ID"), ReadOnly]
        public List<string> nodeIdList = new List<string>();

        public RoomDtoDef()
        {
            
        }

        protected override string GetTypePrefix()
        {
            return "Room";
        }
    }
}