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

        public RoomDataPersistent roomDataPersistent;
        
        public RoomDataTemporary roomDataTemporary;
    }
}