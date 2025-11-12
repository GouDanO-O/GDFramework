using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Procedure.Models.Resource;
using Core.Game.Procedure.Resource;
using Core.Game.Storage;
using GDFrameworkCore;
using GDFrameworkExtend.LogKit;
using UnityEngine;

namespace Core.Game.Chunk.World.Data
{
    public class WorldDataModel : ChunkDataModel, ICanGetSystem
    {
        /// <summary>
        /// 当前游戏固定世界数据配置列表
        /// </summary>
        private List<WorldDtoDef> _worldDtoDefList = new List<WorldDtoDef>();
        
        /// <summary>
        /// DefId -> DtoDef 的快速查找字典
        /// </summary>
        private Dictionary<string, WorldDtoDef> _defIdToDefDict = new Dictionary<string, WorldDtoDef>();
        
        /// <summary>
        /// 所有世界运行时数据字典 (InstanceId -> Data)
        /// </summary>
        private Dictionary<string, WorldData> _worldDataDict = new Dictionary<string, WorldData>();

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        public override void InitializeDataModel()
        {
            LoadExistingTemporaryData();
        }

        /// <summary>
        /// 获取所有世界配置定义
        /// </summary>
        public List<WorldDtoDef> GetAllWorldDefs()
        {
            return new List<WorldDtoDef>(_worldDtoDefList);
        }

        /// <summary>
        /// 根据 DefId 获取配置
        /// </summary>
        public WorldDtoDef GetDefById(string defId)
        {
            return _defIdToDefDict.TryGetValue(defId, out var def) ? def : null;
        }
        
        /// <summary>
        /// 根据 InstanceId 获取世界数据
        /// </summary>
        public WorldData GetWorldData(string instanceId)
        {
            return _worldDataDict.TryGetValue(instanceId, out var data) ? data : null;
        }

        /// <summary>
        /// 获取所有世界运行时数据
        /// </summary>
        public List<WorldData> GetAllWorlds()
        {
            return new List<WorldData>(_worldDataDict.Values);
        }

        /// <summary>
        /// 创建世界实例
        /// </summary>
        public WorldData CreateWorldInstance(WorldDtoDef def, string instanceId = null)
        {
            if (def == null)
            {
                Debug.LogError("无法创建世界实例: 配置为空");
                return null;
            }

            if (string.IsNullOrEmpty(instanceId))
            {
                instanceId = GenerateInstanceId();
            }

            if (_worldDataDict.ContainsKey(instanceId))
            {
                Debug.LogWarning($"世界实例已存在: {instanceId}");
                return _worldDataDict[instanceId];
            }

            var worldData = new WorldData();
            worldData.InitChunkData(def);

            _worldDataDict[instanceId] = worldData;

            Debug.Log($"创建世界实例: {def.DefName} (InstanceId: {instanceId}, DefId: {def.DefId})");

            return worldData;
        }

        /// <summary>
        /// 删除世界实例
        /// </summary>
        public void DeleteWorldInstance(string instanceId)
        {
            if (_worldDataDict.TryGetValue(instanceId, out var data))
            {
                data.DeleteTemporaryData();
                _worldDataDict.Remove(instanceId);
                
                Debug.Log($"删除世界实例: {instanceId}");
            }
        }

        /// <summary>
        /// 添加 Def 配置(带上下文)
        /// </summary>
        public void AddDtoDef(WorldDtoDef dtoDef)
        {
            if (dtoDef == null)
            {
                LogKit.Error("无法添加空的 WorldDtoDef");
                return;
            }

            if (_defIdToDefDict.ContainsKey(dtoDef.DefId))
            {
                LogKit.Error($"WorldDtoDef 已存在,跳过: {dtoDef.DefId}");
                return;
            }

            _worldDtoDefList.Add(dtoDef);
            _defIdToDefDict[dtoDef.DefId] = dtoDef;
            
            LogKit.Error($"添加配置: {dtoDef.DefName} (DefId: {dtoDef.DefId}");

            TryLoadExistingInstancesForDef(dtoDef);
        }

        /// <summary>
        /// 移除 Def 配置
        /// </summary>
        public void RemoveDtoDef(string defId)
        {
            if (_defIdToDefDict.TryGetValue(defId, out var def))
            {
                _worldDtoDefList.Remove(def);
                _defIdToDefDict.Remove(defId);
                
                LogKit.Log($"移除世界配置: {def.DefName} ({defId})");
            }
        }

        /// <summary>
        /// 尝试为配置加载已存在的临时数据实例
        /// </summary>
        private void TryLoadExistingInstancesForDef(WorldDtoDef def)
        {
            var storageSystem = this.GetSystem<StorageSystem>();
            if (storageSystem == null)
            {
                LogKit.Error("StorageSystem 未初始化,跳过临时数据加载");
                return;
            }

            // var tempDataList = storageSystem.LoadAllTemporaryDataByDefId<WorldTemporaryData>(def.DefId);
            //
            // if (tempDataList != null && tempDataList.Count > 0)
            // {
            //     Debug.Log($"找到 {tempDataList.Count} 个世界临时数据实例 (DefId: {def.DefId})");
            //
            //     foreach (var tempData in tempDataList)
            //     {
            //         var worldData = new WorldData();
            //         worldData.SetDefData(def);
            //         worldData.SetTempData();
            //
            //         _worldDataDict[tempData.InstanceId] = worldData;
            //
            //         Debug.Log($"恢复世界实例: {def.DefName} (InstanceId: {tempData.InstanceId})");
            //     }
            // }
        }

        /// <summary>
        /// 启动时加载所有已存在的临时数据
        /// </summary>
        private void LoadExistingTemporaryData()
        {
            var storageSystem = this.GetSystem<StorageSystem>();
            if (storageSystem == null)
            {
                Debug.LogWarning("StorageSystem 未初始化,跳过临时数据加载");
                return;
            }

            // var allTempData = storageSystem.LoadAllTemporaryData<WorldTemporaryData>();
            //
            // if (allTempData == null || allTempData.Count == 0)
            // {
            //     Debug.Log("没有找到已保存的世界临时数据");
            //     return;
            // }
            //
            // Debug.Log($"找到 {allTempData.Count} 个世界临时数据");
            //
            // foreach (var tempData in allTempData)
            // {
            //     var def = GetDefById(tempData.DefId);
            //     if (def == null)
            //     {
            //         Debug.LogWarning($"找不到对应的配置 (DefId: {tempData.DefId}),跳过实例 {tempData.InstanceId}");
            //         continue;
            //     }
            //
            //     if (!_worldDataDict.ContainsKey(tempData.InstanceId))
            //     {
            //         var worldData = new WorldData();
            //         worldData.SetDefData(def);
            //         worldData.SetTempData();
            //
            //         _worldDataDict[tempData.InstanceId] = worldData;
            //
            //         Debug.Log($"恢复世界实例: {def.DefName} (InstanceId: {tempData.InstanceId})");
            //     }
            // }
        }

        /// <summary>
        /// 保存所有世界实例的临时数据
        /// </summary>
        public void SaveAllTemporaryData()
        {
            foreach (var worldData in _worldDataDict.Values)
            {
                worldData.SaveTemporaryData();
            }
            Debug.Log($"保存了 {_worldDataDict.Count} 个世界实例的临时数据");
        }

        /// <summary>
        /// 生成实例ID
        /// </summary>
        private string GenerateInstanceId()
        {
            return $"WORLD_INST_{System.Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
        }
    }
}