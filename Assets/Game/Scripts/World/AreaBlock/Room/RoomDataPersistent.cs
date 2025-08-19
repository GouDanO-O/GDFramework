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
    public class RoomDataPersistent : ConfigData
    {
        [LabelText("房间里面拥有的互动节点"),JsonIgnore]
        public List<NodeData> nodeDatas = new List<NodeData>();
        
        [LabelText("房间里面拥有的节点ID"),ReadOnly]
        public List<string> nodeIds = new List<string>();
        
        public void SaveConfigData(string roomPath,string nodePath)
        {
            nodeIds.Clear();
            for (int i = 0; i < nodeDatas.Count; i++)
            {
                NodeDataPersistent nodeDataPersistent = nodeDatas[i].nodeDataPersistent;
                string curId = nodeDataPersistent.configId;
                if (nodeIds.Contains(curId))
                {
                    LogMonoUtility.AddErrorLog("重复的节点ID");
                }
                else
                {
                    nodeDataPersistent.SaveConfigData(nodePath);
                    nodeIds.Add(curId);
                }
            }
            this.SaveConfigData(roomPath);
        }
    }
}