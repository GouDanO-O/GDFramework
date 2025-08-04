using System;
using System.Collections.Generic;
using GDFrameworkExtend.Data;
using Sirenix.OdinInspector;

namespace Game.World
{
    [Serializable]
    public class AreaBlockDataPersistent : ConfigData
    {
        public string areaBlockName;

        public string areaBlockId;
        
        public List<RoomData> roomDatas = new List<RoomData>();
        
    }
}