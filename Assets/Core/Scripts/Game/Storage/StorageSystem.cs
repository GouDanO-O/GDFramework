using System;
using System.Collections.Generic;
using System.Linq;
using Core.Game.Chunk.Data.Interface;
using Core.Game.Chunk.Substance.Interface;
using Core.Game.Chunk.Universe.Data;
using Core.Game.Storage.Data;
using GDFrameworkCore;
using UnityEngine;

namespace Core.Game.Storage
{
    /// <summary>
    /// 存储系统 (简化版)
    /// 使用 DefId 作为唯一标识
    /// </summary>
    public class StorageSystem : AbstractSystem
    {
        /// <summary>
        /// 存档槽位Key前缀
        /// </summary>
        private const string SlotDataKeyPrefix = "SlotData_";
        
        /// <summary>
        /// 临时数据Key前缀
        /// </summary>
        private const string TempDataKeyPrefix = "TempData_";
        
        /// <summary>
        /// 最大存档数量
        /// </summary>
        private const int MaxSlotCount = 6;

        /// <summary>
        /// 所有的存档槽位数据 (缓存)
        /// </summary>
        private Dictionary<int, SaveSlotData> _slotDataDict = new Dictionary<int, SaveSlotData>();

        /// <summary>
        /// 当前存档槽位索引
        /// </summary>
        private int _currentSlotIndex = -1;

        /// <summary>
        /// 当前存档槽位数据
        /// </summary>
        private SaveSlotData _currentSlotData;
        
        protected override void OnInit()
        {
            // 初始化时可以加载默认存档
        }

        #region 存档槽位管理

        /// <summary>
        /// 获取所有存档槽位信息
        /// </summary>
        public List<SaveSlotData> GetAllSlots()
        {
            List<SaveSlotData> slots = new List<SaveSlotData>();
            for (int i = 0; i < MaxSlotCount; i++)
            {
                slots.Add(GetSlotData(i));
            }
            return slots;
        }

        /// <summary>
        /// 判断是否有任何存档
        /// </summary>
        public bool HasAnySlot()
        {
            for (int i = 0; i < MaxSlotCount; i++)
            {
                if (HasSlot(i))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断指定槽位是否有存档
        /// </summary>
        public bool HasSlot(int slotIndex)
        {
            if (!IsValidSlotIndex(slotIndex))
            {
                return false;
            }
            
            string key = GetSlotDataKey(slotIndex);
            return ES3.KeyExists(key);
        }

        /// <summary>
        /// 获取指定槽位数据
        /// </summary>
        public SaveSlotData GetSlotData(int slotIndex)
        {
            if (!IsValidSlotIndex(slotIndex))
            {
                Debug.LogError($"无效的存档槽位: {slotIndex}");
                return new SaveSlotData(slotIndex);
            }

            // 从缓存获取
            if (_slotDataDict.ContainsKey(slotIndex))
            {
                return _slotDataDict[slotIndex];
            }

            // 从存储加载
            SaveSlotData slotData;
            string key = GetSlotDataKey(slotIndex);
            
            if (ES3.KeyExists(key))
            {
                slotData = ES3.Load<SaveSlotData>(key);
            }
            else
            {
                slotData = new SaveSlotData(slotIndex);
            }
            
            _slotDataDict[slotIndex] = slotData;
            return slotData;
        }

        /// <summary>
        /// 保存槽位数据
        /// </summary>
        public void SaveSlotData(int slotIndex, SaveSlotData slotData)
        {
            if (!IsValidSlotIndex(slotIndex))
            {
                Debug.LogError($"无效的存档槽位: {slotIndex}");
                return;
            }

            string key = GetSlotDataKey(slotIndex);
            ES3.Save(key, slotData);
            
            _slotDataDict[slotIndex] = slotData;
            
            Debug.Log($"保存存档槽位 {slotIndex}: {slotData.SlotName}");
        }

        /// <summary>
        /// 删除指定槽位的存档
        /// </summary>
        public void DeleteSlot(int slotIndex)
        {
            if (!IsValidSlotIndex(slotIndex))
            {
                Debug.LogError($"无效的存档槽位: {slotIndex}");
                return;
            }

            string key = GetSlotDataKey(slotIndex);
            if (ES3.KeyExists(key))
            {
                var slotData = GetSlotData(slotIndex);
                
                // 删除槽位关联的所有临时数据
                DeleteSlotTemporaryData(slotData.UniverseId);
                
                // 删除槽位数据
                ES3.DeleteKey(key);
                _slotDataDict.Remove(slotIndex);
                
                Debug.Log($"删除存档槽位 {slotIndex}");
            }
        }

        /// <summary>
        /// 更新当前选择的存档槽位
        /// </summary>
        public void SetCurrentSlot(int slotIndex)
        {
            if (!IsValidSlotIndex(slotIndex))
            {
                Debug.LogError($"无效的存档槽位: {slotIndex}");
                return;
            }
            
            _currentSlotIndex = slotIndex;
            _currentSlotData = GetSlotData(slotIndex);
            
            Debug.Log($"切换到存档槽位 {slotIndex}: {_currentSlotData.SlotName}");
        }

        /// <summary>
        /// 获取当前槽位
        /// </summary>
        public SaveSlotData GetCurrentSlotData()
        {
            if (_currentSlotData == null && _currentSlotIndex >= 0)
            {
                _currentSlotData = GetSlotData(_currentSlotIndex);
            }
            return _currentSlotData;
        }

        #endregion

        #region Def存储

        public void SaveDef(IChunkDtoDef dtoDef)
        {
            
        }

        public void DeleteDef(IChunkDtoDef dtoDef)
        {
            
        }

        #endregion

        #region 临时区块数据管理

        /// <summary>
        /// 保存临时数据
        /// </summary>
        public void SaveChunkTemporaryData(string defId, IChunkTemporaryData tempData)
        {
            if (string.IsNullOrEmpty(defId))
            {
                Debug.LogError("DefId 为空,无法保存临时数据");
                return;
            }

            if (tempData == null)
            {
                Debug.LogError($"临时数据为空,无法保存: {defId}");
                return;
            }

            // 更新修改时间
            tempData.LastModifyTime = DateTime.Now;
            tempData.DefId = defId;

            // 使用当前槽位的宇宙ID作为前缀,隔离不同存档的数据
            string key = GetTempDataKey(defId);
            ES3.Save(key, tempData);
            
            Debug.Log($"<color=cyan>保存临时数据: {defId}</color>");
        }

        /// <summary>
        /// 加载临时数据 (通用版本)
        /// </summary>
        public IChunkTemporaryData LoadChunkTemporaryData(string defId, Type type)
        {
            if (string.IsNullOrEmpty(defId))
            {
                Debug.LogWarning("DefId 为空,无法加载临时数据");
                return null;
            }

            string key = GetTempDataKey(defId);
            
            if (ES3.KeyExists(key))
            {
                try
                {
                    var tempData = ES3.Load(key, type) as IChunkTemporaryData;
                    Debug.Log($"<color=cyan>加载临时数据: {defId}</color>");
                    return tempData;
                }
                catch (Exception e)
                {
                    Debug.LogError($"加载临时数据失败 {defId}: {e.Message}");
                    return null;
                }
            }

            return null;
        }

        #endregion
        
        #region 临时实体数据管理

        /// <summary>
        /// 保存实体临时数据
        /// </summary>
        public void SaveEntityTemporaryData(string instanceId, IEntityTemporaryData tempData)
        {
            if (string.IsNullOrEmpty(instanceId))
            {
                Debug.LogError("instanceId 为空,无法保存临时数据");
                return;
            }

            if (tempData == null)
            {
                Debug.LogError($"临时数据为空,无法保存: {instanceId}");
                return;
            }

            // 更新修改时间
            tempData.LastModifyTime = DateTime.Now;
            tempData.EntityInstanceId = instanceId;

            // 使用当前槽位的宇宙ID作为前缀,隔离不同存档的数据
            string key = GetTempDataKey(instanceId);
            ES3.Save(key, tempData);
            
            Debug.Log($"<color=cyan>保存临时数据: {instanceId}</color>");
        }
        
        /// <summary>
        /// 加载实体临时数据
        /// </summary>
        public IEntityTemporaryData LoadEntityTemporaryData(string instanceId, Type type)
        {
            if (string.IsNullOrEmpty(instanceId))
            {
                Debug.LogWarning("instanceId 为空,无法加载临时数据");
                return null;
            }

            string key = GetTempDataKey(instanceId);
            
            if (ES3.KeyExists(key))
            {
                try
                {
                    var tempData = ES3.Load(key, type) as IEntityTemporaryData;
                    Debug.Log($"<color=cyan>加载临时数据: {instanceId}</color>");
                    return tempData;
                }
                catch (Exception e)
                {
                    Debug.LogError($"加载临时数据失败 {instanceId}: {e.Message}");
                    return null;
                }
            }

            return null;
        }

        #endregion

        #region 临时数据管理

        /// <summary>
        /// 加载临时数据
        /// </summary>
        public T LoadTemporaryData<T>(string defId) where T : class, IChunkTemporaryData
        {
            if (string.IsNullOrEmpty(defId))
            {
                Debug.LogWarning("DefId 为空,无法加载临时数据");
                return null;
            }

            string key = GetTempDataKey(defId);
            
            if (ES3.KeyExists(key))
            {
                try
                {
                    T tempData = ES3.Load<T>(key);
                    Debug.Log($"<color=cyan>加载临时数据: {defId}</color>");
                    return tempData;
                }
                catch (Exception e)
                {
                    Debug.LogError($"加载临时数据失败 {defId}: {e.Message}");
                    return null;
                }
            }

            return null;
        }
        
        /// <summary>
        /// 删除临时数据
        /// </summary>
        public void DeleteTemporaryData(string defId)
        {
            if (string.IsNullOrEmpty(defId))
            {
                Debug.LogWarning("DefId 为空,无法删除临时数据");
                return;
            }

            string key = GetTempDataKey(defId);
            
            if (ES3.KeyExists(key))
            {
                ES3.DeleteKey(key);
                Debug.Log($"<color=yellow>删除临时数据: {defId}</color>");
            }
        }

        /// <summary>
        /// 检查临时数据是否存在
        /// </summary>
        public bool ExistsTemporaryData(string defId)
        {
            if (string.IsNullOrEmpty(defId))
            {
                return false;
            }

            string key = GetTempDataKey(defId);
            return ES3.KeyExists(key);
        }

        /// <summary>
        /// 获取所有临时数据的DefId列表
        /// </summary>
        public List<string> GetAllTemporaryDataKeys()
        {
            if (_currentSlotData == null)
            {
                return new List<string>();
            }

            string prefix = GetTempDataKeyPrefix();
            string[] allKeys = ES3.GetKeys();
            
            return allKeys
                .Where(key => key.StartsWith(prefix))
                .Select(key => key.Substring(prefix.Length))
                .ToList();
        }

        /// <summary>
        /// 删除槽位相关的所有临时数据
        /// </summary>
        private void DeleteSlotTemporaryData(string universeId)
        {
            if (string.IsNullOrEmpty(universeId))
            {
                return;
            }

            string prefix = $"{TempDataKeyPrefix}{universeId}_";
            string[] allKeys = ES3.GetKeys();
            
            int deleteCount = 0;
            foreach (var key in allKeys)
            {
                if (key.StartsWith(prefix))
                {
                    ES3.DeleteKey(key);
                    deleteCount++;
                }
            }
            
            Debug.Log($"删除槽位 {universeId} 的 {deleteCount} 条临时数据");
        }

        #endregion

        #region 工具方法

        /// <summary>
        /// 验证槽位索引是否有效
        /// </summary>
        private bool IsValidSlotIndex(int slotIndex)
        {
            return slotIndex >= 0 && slotIndex < MaxSlotCount;
        }

        /// <summary>
        /// 获取存档槽位的Key
        /// </summary>
        private string GetSlotDataKey(int slotIndex)
        {
            return $"{SlotDataKeyPrefix}{slotIndex}";
        }

        /// <summary>
        /// 获取临时数据的Key前缀
        /// </summary>
        private string GetTempDataKeyPrefix()
        {
            if (_currentSlotData == null)
            {
                return TempDataKeyPrefix;
            }
            
            // 使用宇宙ID作为前缀,隔离不同存档
            return $"{TempDataKeyPrefix}{_currentSlotData.UniverseId}_";
        }

        /// <summary>
        /// 获取临时数据的完整Key
        /// </summary>
        private string GetTempDataKey(string defId)
        {
            return $"{GetTempDataKeyPrefix()}{defId}";
        }

        #endregion
    }
}