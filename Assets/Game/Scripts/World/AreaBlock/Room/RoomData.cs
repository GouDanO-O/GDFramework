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
    public class RoomData : ConfigData
    {
        [LabelText("房间固定数据")]
        public RoomDataPersistent roomDataPersistent;
        
        [LabelText("房间对局数据"),ReadOnly]
        public RoomDataTemporary roomDataTemporary;
        
        /// <summary>
        /// 当前区块的数据字典
        /// </summary>
        private Dictionary<string,NodeData> _roomDataDict = new Dictionary<string,NodeData>();
        
        public void SaveConfigData(string path)
        {
            roomDataPersistent.nodeIds.Clear();
            
            if (configId == "")
            {
                configId = "default";
            }
            string roomPath = path+ "/"+configId;
            
            for (int i = 0; i < roomDataPersistent.nodeDatas.Count; i++)
            {
                NodeData nodeData = roomDataPersistent.nodeDatas[i];
                string curId = nodeData.configId;
                if (roomDataPersistent.nodeIds.Contains(curId))
                {
                    LogMonoUtility.AddErrorLog("重复的节点ID");
                }
                else
                {
                    nodeData.SaveConfigData(roomPath);
                    roomDataPersistent.nodeIds.Add(curId);
                }
            }
            base.SaveConfigData(path);
        }
    }
}