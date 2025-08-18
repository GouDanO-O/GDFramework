using System;
using System.Collections.Generic;
using GDFrameworkExtend.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.World
{
    [Serializable]
    public class RoomDataPersistent : ConfigData
    {
        [LabelText("房间里面拥有的互动节点")]
        public List<NodeData> nodeDatas = new List<NodeData>();
        

    }
}