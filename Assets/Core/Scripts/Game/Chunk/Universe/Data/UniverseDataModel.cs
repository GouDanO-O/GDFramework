using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Storage;
using GDFrameworkCore;
using UnityEngine;

namespace Core.Game.Chunk.Universe.Data
{
    public class UniverseDataModel : ChunkDataModel, ICanGetSystem
    {
        /// <summary>
        /// 所有宇宙配置列表 (从资源加载)
        /// </summary>
        private List<UniverseDtoDef> _universeDtoDefList = new List<UniverseDtoDef>();
        
        /// <summary>
        /// DefId -> DtoDef 快速查找
        /// </summary>
        private Dictionary<string, UniverseDtoDef> _defIdToDefDict = new Dictionary<string, UniverseDtoDef>();
        
        /// <summary>
        /// 运行时激活的宇宙数据 (DefId -> Data)
        /// 每个存档只会有一个激活的宇宙
        /// </summary>
        private Dictionary<string, UniverseData> _activeUniverseDataDict = new Dictionary<string, UniverseData>();
        
        /// <summary>
        /// 当前宇宙数据
        /// </summary>
        private UniverseData _currentUniverseData;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        public override void InitializeDataModel()
        {
            // 数据模型初始化
        }

        #region 配置管理

        /// <summary>
        /// 添加 Def 配置 (启动时从资源加载)
        /// </summary>
        public void AddDtoDef(UniverseDtoDef dtoDef)
        {
            if (dtoDef == null)
            {
                Debug.LogError("无法添加空的 UniverseDtoDef");
                return;
            }

            if (_defIdToDefDict.ContainsKey(dtoDef.DefId))
            {
                Debug.LogWarning($"UniverseDtoDef 已存在,跳过: {dtoDef.DefId}");
                return;
            }

            _universeDtoDefList.Add(dtoDef);
            _defIdToDefDict[dtoDef.DefId] = dtoDef;

            Debug.Log($"添加宇宙配置: {dtoDef.DefName} (DefId: {dtoDef.DefId})");
        }

        /// <summary>
        /// 移除配置
        /// </summary>
        public void RemoveDtoDef(string defId)
        {
            if (_defIdToDefDict.TryGetValue(defId, out var def))
            {
                _universeDtoDefList.Remove(def);
                _defIdToDefDict.Remove(defId);
                
                Debug.Log($"移除宇宙配置: {def.DefName} ({defId})");
            }
        }

        /// <summary>
        /// 获取所有配置
        /// </summary>
        public List<UniverseDtoDef> GetAllUniverseDefs()
        {
            return new List<UniverseDtoDef>(_universeDtoDefList);
        }

        /// <summary>
        /// 根据 DefId 获取配置
        /// </summary>
        public UniverseDtoDef GetDefById(string defId)
        {
            return _defIdToDefDict.TryGetValue(defId, out var def) ? def : null;
        }

        #endregion

        #region 运行时数据管理

        /// <summary>
        /// 获取当前宇宙数据
        /// </summary>
        public UniverseData GetCurrentUniverseData()
        {
            if (_currentUniverseData == null)
            {
                LoadOrCreateDefaultUniverse();
            }
            return _currentUniverseData;
        }

        /// <summary>
        /// 根据 DefId 获取或创建宇宙数据
        /// </summary>
        public UniverseData GetOrCreateUniverseData(string defId)
        {
            // 先从激活列表查找
            if (_activeUniverseDataDict.TryGetValue(defId, out var data))
            {
                return data;
            }

            // 获取配置
            var def = GetDefById(defId);
            if (def == null)
            {
                Debug.LogError($"找不到宇宙配置: {defId}");
                return null;
            }

            // 创建运行时数据
            var universeData = new UniverseData();
            universeData.InitChunkData(def);

            _activeUniverseDataDict[defId] = universeData;

            Debug.Log($"创建/加载宇宙数据: {def.DefName} (DefId: {defId})");

            return universeData;
        }

        /// <summary>
        /// 设置当前宇宙
        /// </summary>
        public void SetCurrentUniverse(string defId)
        {
            _currentUniverseData = GetOrCreateUniverseData(defId);
            
            if (_currentUniverseData != null)
            {
                Debug.Log($"切换当前宇宙: {_currentUniverseData.UniverseDef.DefName}");
            }
        }

        /// <summary>
        /// 加载或创建默认宇宙
        /// </summary>
        private void LoadOrCreateDefaultUniverse()
        {
            var storageSystem = this.GetSystem<StorageSystem>();
            var currentSlot = storageSystem?.GetCurrentSlotData();

            if (currentSlot != null && !string.IsNullOrEmpty(currentSlot.UniverseId))
            {
                // 从存档加载宇宙
                _currentUniverseData = GetOrCreateUniverseData(currentSlot.UniverseId);
            }
            else
            {
                // 创建新宇宙
                if (_universeDtoDefList.Count > 0)
                {
                    var defaultDef = _universeDtoDefList[0];
                    _currentUniverseData = GetOrCreateUniverseData(defaultDef.DefId);
                    
                    // 更新存档槽位
                    if (currentSlot != null)
                    {
                        currentSlot.UniverseId = defaultDef.DefId;
                        storageSystem.SaveSlotData(currentSlot.SlotIndex, currentSlot);
                    }
                }
                else
                {
                    Debug.LogError("没有可用的宇宙配置!");
                }
            }
        }

        /// <summary>
        /// 保存所有激活宇宙的临时数据
        /// </summary>
        public void SaveAllTemporaryData()
        {
            foreach (var universeData in _activeUniverseDataDict.Values)
            {
                universeData.SaveTemporaryData();
            }
            Debug.Log($"保存了 {_activeUniverseDataDict.Count} 个宇宙的临时数据");
        }

        /// <summary>
        /// 清除运行时数据 (切换存档时调用)
        /// </summary>
        public void ClearRuntimeData()
        {
            _activeUniverseDataDict.Clear();
            _currentUniverseData = null;
            Debug.Log("清除宇宙运行时数据");
        }

        #endregion
    }
}