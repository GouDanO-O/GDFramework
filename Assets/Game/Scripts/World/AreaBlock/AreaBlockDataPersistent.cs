using System;
using System.Collections.Generic;
using GDFramework.Utility;
using GDFrameworkExtend.Data;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.World
{
    [Serializable,JsonObject]
    public class AreaBlockDataPersistent
    {
        [Title("玩家第一次进入区块所处的房间ID,如果为空,则默认取索引第一位"),LabelText("初始房间ID")]
        public string initialAreaBlockId;

        [Title("当玩家进入又离开区块时,是否需要缓存当前所处房间ID,如果不缓存,则每次进入都进入初始房间,否则进入历史房间"),LabelText("是否缓存当前房间id")]
        public bool willCacheLocateRoomIdWhenPlayerEntersAndLeavesAreaBlock;
        
        [LabelText("区块里面的房间"),JsonIgnore,
         ValidateInput("CheckConfigId", "配置ID不能重复", InfoMessageType.Error)]
        public List<RoomDto> roomDatas = new List<RoomDto>();
        
        [LabelText("区块所拥有的房间ID"),ReadOnly]
        public List<string> roomIds  = new List<string>();
        
        private bool CheckConfigId()
        {
            var idSet = new HashSet<string>();
            foreach (var data in roomDatas)
            {
                if (!idSet.Add(data.configId)) // 如果添加失败（已存在），返回true
                    return false;
            }
            return true;
        }
    }
}