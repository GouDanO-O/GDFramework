using System;
using System.Collections.Generic;
using GDFrameworkExtend.Data;
using Sirenix.OdinInspector;

namespace Game.World
{
    [Serializable]
    public class RoomDto : ConfigData
    {
        [LabelText("房间ID==>地图_区块_房间身份ID(房间名字必须唯一)")]
        public string roomId;
        
        [LabelText("房间名")]
        public string roomName;
        
        [LabelText("房间里面持有的所有节点")]
        public List<NodeDto> nodeDtoList;
        
        [LabelText("可选：多个入口，支持非线性关卡")]
        public List<string> entryNodeIds = new();
    }
    
    public class Room
    {
        public RoomDto RoomData;
        
        
    }
}