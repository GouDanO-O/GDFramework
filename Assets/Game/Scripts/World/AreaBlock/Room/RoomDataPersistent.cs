using System;
using System.Collections.Generic;
using GDFramework.Utility;
using GDFrameworkExtend.Data;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.World
{
    [Serializable]
    public class RoomDataPersistent
    {
        [LabelText("房间里面拥有的互动节点"),JsonIgnore]
        public List<NodeDto> nodeDatas;
        
        [LabelText("房间里面拥有的节点ID"),ReadOnly]
        public List<string> nodeIds;
        

    }
}