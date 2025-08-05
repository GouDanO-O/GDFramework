using System;
using System.Collections.Generic;
using GDFrameworkExtend.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.World
{
    [Serializable]
    public class RoomData
    {
        [LabelText("房间固定数据")]
        public RoomDataPersistent roomDataPersistent;
        
        [LabelText("房间对局数据"),ReadOnly]
        public RoomDataTemporary roomDataTemporary;
    }
}