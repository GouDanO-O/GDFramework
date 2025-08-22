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
    public class WorldDataPersistent
    {
        [LabelText("初始区块ID(玩家第一次进入世界所处的区块ID)")]
        public string initialAreaBlockId;
        
        [LabelText("区块数据列表"),JsonIgnore,
         ValidateInput("CheckConfigId", "配置ID不能重复", InfoMessageType.Error)]
        public List<AreaBlockDto> areaBlockDatas = new List<AreaBlockDto>();

        [LabelText("当前世界拥有的区块ID"),ReadOnly]
        public List<string> areaBlockIds = new List<string>();

        #region EditorExtend

        private bool CheckConfigId()
        {
            var idSet = new HashSet<string>();
            foreach (var data in areaBlockDatas)
            {
                if (!idSet.Add(data.configId)) // 如果添加失败（已存在），返回true
                    return false;
            }
            return true;
        }

        #endregion
        

    }
}