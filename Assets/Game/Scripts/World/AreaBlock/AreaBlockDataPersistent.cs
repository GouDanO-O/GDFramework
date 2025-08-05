using System;
using System.Collections.Generic;
using GDFrameworkExtend.Data;
using Sirenix.OdinInspector;

namespace Game.World
{
    [Serializable]
    public class AreaBlockDataPersistent : ConfigData
    {
        [LabelText("区块名称")]
        public string areaBlockName;

        [LabelText("区块ID")]
        public string areaBlockId;

        [LabelText("区块描述")]
        public string areaBlockDes;
        
        [LabelText("区块里面的房间")]
        public List<RoomData> roomDatas = new List<RoomData>();
        
    }
}