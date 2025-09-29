using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Game.World.Interface;
using GDFramework.Utility;
using GDFrameworkExtend.Data;
using GDFrameworkExtend.JsonKit;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Game.World
{
    public class AreaBlockData : IData
    {
        public string UniqueId { get; set; }     
        
        [LabelText("当前区块的固定数据")]
        private AreaBlockDto areaBlockDto;

        [LabelText("当前区块的临时数据")]
        private AreaBlockDtoTemporary areaBlockDtoTemporary;
        
        [LabelText("当前区块持有的房间数据")]
        private Dictionary<string,RoomData> curHoldingRoomDtoDict = new Dictionary<string, RoomData>();
    }
}