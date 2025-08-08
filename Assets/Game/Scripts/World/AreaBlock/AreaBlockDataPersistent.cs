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
        
        [LabelText("初始房间ID(玩家第一次进入区块所处的房间ID\n如果为空,则默认取索引第一位)")]
        public string initialAreaBlockId;

        [LabelText("当玩家进入又离开区块时,是否需要缓存当前所处房间ID\n如果不缓存,则每次进入都进入初始房间,否则进入历史房间")]
        public bool willCacheLocateRoomIdWhenPlayerEntersAndLeavesAreaBlock = false;
        
        [LabelText("区块里面的房间")]
        public List<RoomData> roomDatas = new List<RoomData>();
        
    }
}