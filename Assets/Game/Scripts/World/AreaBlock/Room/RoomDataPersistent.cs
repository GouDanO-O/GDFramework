using System;
using System.Collections.Generic;
using GDFramework.Utility;
using GDFrameworkExtend.Data;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.World
{
    [Serializable]
    public class RoomDataPersistent
    {
        [LabelText("房间里面拥有的互动节点"), JsonIgnore,
         ValidateInput("CheckConfigId", "配置ID不能重复", InfoMessageType.Error)]
        public List<NodeDto> nodeDatas = new List<NodeDto>();

        [LabelText("房间里面拥有的节点ID"), ReadOnly] 
        public List<string> nodeIds = new List<string>();

        private bool CheckConfigId()
        {
            var idSet = new HashSet<string>();
            foreach (var data in nodeDatas)
            {
                if (!idSet.Add(data.configId)) // 如果添加失败（已存在），返回true
                    return false;
            }

            return true;
        }
    }
}