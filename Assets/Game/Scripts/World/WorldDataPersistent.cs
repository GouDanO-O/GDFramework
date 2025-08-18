using System;
using System.Collections.Generic;
using System.IO;
using GDFramework.Utility;
using GDFrameworkExtend.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.World
{
    [Serializable]
    public class WorldDataPersistent : ConfigData
    {
        [LabelText("初始区块ID(玩家第一次进入世界所处的区块ID)")]
        public string initialAreaBlockId;
        
        [LabelText("区块数据列表")]
        public List<AreaBlockData> areaBlockDatas = new List<AreaBlockData>();

        [HideInInspector]
        public List<string> areaBlockIds = new List<string>();

        public override void SaveConfigData(string path)
        {
            string willSavePath = path;
            
            string dirPath = Path.GetDirectoryName(willSavePath);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            areaBlockIds.Clear();
            for (int i = 0; i < areaBlockDatas.Count; i++)
            {
                string curId = areaBlockDatas[i].areaBlockDataPersistent.configId;
                if (areaBlockIds.Contains(curId))
                {
                    LogMonoUtility.AddErrorLog("重复的房间ID");
                }
                else
                {
                    areaBlockIds.Add(curId);
                }
            }

            object curData = this;
            willSavePath += curData.GetType()+".json";
            
            // 保存完整的WorldData对象
            string json = JsonUtility.ToJson(this, true);
            File.WriteAllText(willSavePath, json);
            Debug.Log("保存完整世界数据成功");
        }
    }
}