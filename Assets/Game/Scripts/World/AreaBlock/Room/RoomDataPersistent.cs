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
    public class RoomDataPersistent : PersistentData
    {
        [LabelText("房间里面拥有的互动节点"),JsonIgnore]
        public List<NodeData> nodeDatas = new List<NodeData>();
        
        [LabelText("房间里面拥有的节点ID"),ReadOnly]
        public List<string> nodeIds = new List<string>();
        

    }
}