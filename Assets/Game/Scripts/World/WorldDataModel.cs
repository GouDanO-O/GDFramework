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
using UnityEditor;
using UnityEngine;

namespace Game.World
{
    public interface IDto
    {
        public void LoadData()
        {
        }

        public void SaveData()
        {
        }

        public void SaveData(string directory, JsonSerializerSettings settings)
        {
        }

        public void SaveData(string worldRootDir, string directory, JsonSerializerSettings settings)
        {
        }
    }

    // 首先定义一个接口，用于支持父级引用和自动刷新
    public interface IHierarchicalDto : IDto
    {
        string GetParentDtoId();
        void SetParent(IHierarchicalDto parent);
        void RefreshDtoId();
        void RefreshChildrenDtoIds();
        void AutoRefreshHierarchy();
    }

    [Serializable, JsonObject]
    public class WorldDto : IHierarchicalDto
    {
        [LabelText("配置名称")] public string configName;

        [LabelText("配置ID(当前配置的ID,同一层必须唯一)"),
         OnValueChanged("OnWorldIdChange"),
         ValidateInput("ValidateConfigId", "配置ID不能包含下划线!", InfoMessageType.Error)]
        public string configId;

        [LabelText("用于存储和读取时使用的id(会进行层级拼接,防止重复)"), DisableInEditorMode]
        public string dtoId;

        [LabelText("配置描述")] public string configDes;

        [JsonIgnore, HideInInspector] private IHierarchicalDto parent = null;

        /// <summary>
        /// 固定数据路径（只读）
        /// </summary>
        public string PersistentDataPath
        {
            get
            {
#if UNITY_EDITOR
                return "Assets/Game/Res/Configs/WorldData";
#endif
                return Path.Combine(Application.persistentDataPath, configName);
            }
        }

        /// <summary>
        /// 临时数据路径（可写入）
        /// </summary>
        [JsonIgnore]
        public string TemporaryDataPath
        {
            get { return Path.Combine(Application.persistentDataPath, "WorldData"); }
        }

        [LabelText("世界固定数据")] public WorldDataPersistent worldDataPersistent;
        [LabelText("世界对局数据")] public WorldDataTemporary worldDataTemporary;
        [LabelText("世界画布数据")] public WorldCanvasDataPersistent worldCanvasDataPersistent;

        // 添加一个属性包装器来监听列表变化
        private List<AreaBlockDto> _cachedAreaBlocks = new List<AreaBlockDto>();

        // 实现 IHierarchicalDto 接口
        public string GetParentDtoId()
        {
            return null; // World 是顶级，没有父级
        }

        public void SetParent(IHierarchicalDto parent)
        {
            this.parent = parent; // World 通常没有父级，但保留接口一致性
        }

        public void RefreshDtoId()
        {
            this.dtoId = this.configId;
        }

        public void RefreshChildrenDtoIds()
        {
            if (worldDataPersistent?.areaBlockDatas != null)
            {
                foreach (var area in worldDataPersistent.areaBlockDatas)
                {
                    area.SetParent(this);
                    area.RefreshDtoId();
                    area.RefreshChildrenDtoIds();
                }
            }
        }

        public void AutoRefreshHierarchy()
        {
            RefreshDtoId();
            RefreshChildrenDtoIds();

            // 监听列表变化
            CheckForListChanges();
        }

        // 检查列表变化并自动建立父子关系
        private void CheckForListChanges()
        {
            if (worldDataPersistent?.areaBlockDatas == null) return;

            // 检查是否有新增或移除的区块
            var currentBlocks = worldDataPersistent.areaBlockDatas;
            var newBlocks = currentBlocks.Except(_cachedAreaBlocks).ToList();
            var removedBlocks = _cachedAreaBlocks.Except(currentBlocks).ToList();

            // 为新增的区块设置父级关系
            foreach (var newBlock in newBlocks)
            {
                newBlock.SetParent(this);
                newBlock.AutoRefreshHierarchy();
            }

            // 更新缓存
            _cachedAreaBlocks = new List<AreaBlockDto>(currentBlocks);
        }

        // Unity的OnValidate在Inspector值改变时自动调用
#if UNITY_EDITOR
        [OnInspectorInit]
        private void OnInspectorInit()
        {
            AutoRefreshHierarchy();
        }

        // 每当Inspector更新时自动刷新
        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            // 检查是否需要刷新层级关系
            if (worldDataPersistent?.areaBlockDatas != null)
            {
                bool needsRefresh = false;

                // 检查列表大小是否变化
                if (_cachedAreaBlocks.Count != worldDataPersistent.areaBlockDatas.Count)
                {
                    needsRefresh = true;
                }
                else
                {
                    // 检查列表内容是否变化
                    for (int i = 0; i < worldDataPersistent.areaBlockDatas.Count; i++)
                    {
                        if (i >= _cachedAreaBlocks.Count ||
                            !ReferenceEquals(worldDataPersistent.areaBlockDatas[i], _cachedAreaBlocks[i]))
                        {
                            needsRefresh = true;
                            break;
                        }
                    }
                }

                if (needsRefresh)
                {
                    EditorApplication.delayCall += () => { AutoRefreshHierarchy(); };
                }
            }
        }
#endif

        public void SaveData()
        {
            // 保存前自动刷新所有ID
            AutoRefreshHierarchy();

            SaveData_Persistent();
            SaveData_Temporary();
        }

        public void SaveData_Persistent()
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(PersistentDataPath))
            {
                LogMonoUtility.AddErrorLog("PersistentDataPath 为空，无法保存模板！");
                return;
            }

            if (string.IsNullOrEmpty(configId))
                configId = "world_default";

            // 自动刷新ID级联
            AutoRefreshHierarchy();

            worldDataPersistent.areaBlockIds ??= new List<string>();
            worldDataPersistent.areaBlockDatas ??= new List<AreaBlockDto>();
            worldDataPersistent.areaBlockIds.Clear();

            string worldRootDir = Path.Combine(PersistentDataPath, configId);
            Directory.CreateDirectory(worldRootDir);

            foreach (var area in worldDataPersistent.areaBlockDatas)
            {
                string aid = string.IsNullOrEmpty(area.configId) ? "area_auto" : area.configId;
                if (worldDataPersistent.areaBlockIds.Contains(aid))
                    LogMonoUtility.AddErrorLog($"重复的区块ID: {aid}");
                else
                    worldDataPersistent.areaBlockIds.Add(aid);

                area.SaveData_Persistent(worldRootDir, JsonSettings.Make());
            }

            var persistentData = new
            {
                configName = this.configName,
                configId = this.configId,
                configDes = this.configDes,
                worldDataPersistent = this.worldDataPersistent
            };

            string worldJsonPath = Path.Combine(PersistentDataPath, $"{configId}.json");
            File.WriteAllText(worldJsonPath, JsonConvert.SerializeObject(persistentData, JsonSettings.Make()));
            LogMonoUtility.AddLog($"保存固定数据 {worldJsonPath} 成功");
#else
        LogMonoUtility.AddWarning("SaveData_Persistent 只能在编辑器下使用，运行时固定数据为只读状态！");
#endif
        }

        public void SaveData_Temporary()
        {
            if (string.IsNullOrEmpty(TemporaryDataPath))
            {
                LogMonoUtility.AddErrorLog("TemporaryDataPath 为空，无法保存临时数据！");
                return;
            }

            if (string.IsNullOrEmpty(configId))
                configId = "world_default";

            try
            {
                var temporaryData = new
                {
                    configName = this.configName,
                    configId = this.configId,
                    configDes = this.configDes,
                    worldDataTemporary = this.worldDataTemporary
                };

                foreach (var area in worldDataPersistent.areaBlockDatas)
                {
                    area.SaveData_Temporary();
                }

                ES3.Save(configId, temporaryData);
            }
            catch (Exception ex)
            {
                LogMonoUtility.AddErrorLog($"保存临时数据失败: {ex.Message}");
            }
        }

        private void OnWorldIdChange()
        {
            AutoRefreshHierarchy();
        }

        // 验证配置ID是否包含下划线
        private bool ValidateConfigId(string id)
        {
            return !string.IsNullOrEmpty(id) && !id.Contains("_");
        }
    }

    [Serializable]
    public class WorldDataModel : AbstractModel
    {
        public WorldDto worldDto;

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
            worldDto.SaveData();
        }
    }
}