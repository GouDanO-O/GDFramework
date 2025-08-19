using System;
using System.Collections.Generic;
using System.IO;
using Game.Models.Resource;
using GDFramework.Utility;
using GDFrameworkCore;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.World
{
    [Serializable,JsonObject]
    public class WorldDataModel : AbstractModel
    {
        public string configId;
        
        public string PersistentDataPath = "Assets/Game/Res/Configs/WorldData";
        
        [LabelText("世界固定数据")]
        public WorldDataPersistent worldDataPersistent;
        
        [LabelText("世界对局数据")]
        public WorldDataTemporary worldDataTemporary;

        [LabelText("世界画布数据")]
        public WorldCanvasDataPersistent worldCanvasDataPersistent;
        
        /// <summary>
        /// 所有区块数据
        /// </summary>
        private Dictionary<string,AreaBlockData> _areaBlockDataDict = new Dictionary<string, AreaBlockData>();
        
        /// <summary>
        /// 当前区块房间数据
        /// </summary>
        private Dictionary<string,RoomData> _curRoomDataDict = new Dictionary<string, RoomData>();
        
        /// <summary>
        /// 当前房间所有节点数据
        /// </summary>
        private Dictionary<string,NodeData>  _curNodeDataDict = new Dictionary<string, NodeData>();
        
        private WorldDataUtility _worldDataUtility;

        protected override void OnInit()
        {
            _worldDataUtility = this.GetUtility<WorldDataUtility>();
            
        }

        public void GetWorldData()
        {
#if UNITY_EDITOR
            _worldDataUtility = new WorldDataUtility();
#endif
            _worldDataUtility.LoadCompleteWorldData(this);
        }

        public void SaveWorldData()
        {
#if UNITY_EDITOR
            _worldDataUtility = new WorldDataUtility();
#endif
            _worldDataUtility.SaveCompleteWorldData(this);
        }

        public AreaBlockData GetCurrentAreaBlockData(string areaBlockId)
        {
            if (worldDataPersistent == null)
            {
                LogMonoUtility.AddErrorLog("世界固定数据为空");
                return null;
            }

            if (worldDataPersistent.areaBlockDatas.Count == 0)
            {
                LogMonoUtility.AddErrorLog("世界中的区块数据为空");
                return null;
            }

            if (_areaBlockDataDict.Count == 0)
            {
                LogMonoUtility.AddErrorLog("区块字典为空");
                return null;
            }
            
            if (_areaBlockDataDict.ContainsKey(areaBlockId))
            {
                return _areaBlockDataDict[areaBlockId];
            }

            LogMonoUtility.AddErrorLog("区块字典未包含该ID");
            return null;
        }
        
        /// <summary>
        /// 更换区块
        /// </summary>
        public void ChangeAreaBlock()
        {
            
        }
        
        public void SaveConfigData()
        {
            if (configId == "")
            {
                configId = "default";
            }
            worldDataPersistent.areaBlockIds.Clear();
            string worldataPath = PersistentDataPath + "/" + configId;
            for (int i = 0; i < worldDataPersistent.areaBlockDatas.Count; i++)
            {
                AreaBlockData areaBlockData = worldDataPersistent.areaBlockDatas[i];
                string curId = areaBlockData.configId;
                if (worldDataPersistent.areaBlockIds.Contains(curId))
                {
                    LogMonoUtility.AddErrorLog("重复的房间ID");
                }
                else
                {
                    areaBlockData.SaveConfigData(worldataPath);
                    worldDataPersistent.areaBlockIds.Add(curId);
                }
            }

            string willSavePath = PersistentDataPath;
            
            string dirPath = Path.GetDirectoryName(willSavePath);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            
            willSavePath += "/"+this.configId+".json";
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(willSavePath, json);
            LogMonoUtility.AddLog($"保存{willSavePath}数据成功");
        }
    }
}