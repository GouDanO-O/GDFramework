using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game.Models.Resource;
using GDFramework.Utility;
using GDFrameworkCore;
using GDFrameworkExtend.JsonKit;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.World
{
    [Serializable,JsonObject]
    public class WorldDataModel : AbstractModel
    {
        public string configId;
        
        /// <summary>
        /// 模板根目录（例如：Assets/Game/Res/Configs/WorldData）
        /// </summary>
        [JsonIgnore]
        public string PersistentDataPath = "Assets/Game/Res/Configs/WorldData";
        
        [LabelText("世界固定数据")]
        public WorldDataPersistent worldDataPersistent;
        
        [LabelText("世界对局数据")]
        public WorldDataTemporary worldDataTemporary;

        [LabelText("世界画布数据")]
        public WorldCanvasDataPersistent worldCanvasDataPersistent;
        
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
        
        public void SaveConfigData()
        {
            if (string.IsNullOrEmpty(PersistentDataPath))
            {
                LogMonoUtility.AddErrorLog("PersistentDataPath 为空，无法保存模板！");
                return;
            }

            if (string.IsNullOrEmpty(configId))
                configId = "world_default";

            // 1) 确保列表存在并去重
            worldDataPersistent ??= new WorldDataPersistent();
            worldDataPersistent.areaBlockIds ??= new List<string>();
            worldDataPersistent.areaBlockDatas ??= new List<AreaBlockData>();
            worldDataPersistent.areaBlockIds.Clear();

            // 2) 世界目录：用于存放 area/room/node 子级
            string worldRootDir = Path.Combine(PersistentDataPath, configId);
            Directory.CreateDirectory(worldRootDir);

            // 3) 逐区块保存（区块 JSON 写在 world 根目录；房间/节点分层进子目录）
            foreach (var area in worldDataPersistent.areaBlockDatas.Where(a => a != null))
            {
                string aid = string.IsNullOrEmpty(area.configId) ? "area_auto" : area.configId;

                if (worldDataPersistent.areaBlockIds.Contains(aid))
                    LogMonoUtility.AddErrorLog($"重复的区块ID: {aid}");
                else
                    worldDataPersistent.areaBlockIds.Add(aid);

                // 让 Area 自己处理房间/节点保存；同时把自己的 JSON 写到 world 根目录
                area.SaveConfigData(worldRootDir, JsonSettings.Make());
            }

            // 4) 最后保存世界根 JSON（写在 PersistentDataPath 根下）
            string worldJsonPath = Path.Combine(PersistentDataPath, $"{configId}.json");
            File.WriteAllText(worldJsonPath, JsonConvert.SerializeObject(this, JsonSettings.Make()));
            LogMonoUtility.AddLog($"保存 {worldJsonPath} 数据成功");
        }
    }
}