using System;
using System.Collections.Generic;
using System.IO;
using GDFramework.Utility;
using GDFrameworkExtend.Data;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.World
{
    [Serializable,JsonObject]
    public class WorldDataPersistent : PersistentData
    {
        [LabelText("初始区块ID(玩家第一次进入世界所处的区块ID)")]
        public string initialAreaBlockId;
        
        [LabelText("区块数据列表"),JsonIgnore]
        public List<AreaBlockData> areaBlockDatas = new List<AreaBlockData>();

        [LabelText("当前世界拥有的区块ID"),ReadOnly]
        public List<string> areaBlockIds = new List<string>();
        

    }
}