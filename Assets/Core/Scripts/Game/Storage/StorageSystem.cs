using System;
using System.Collections.Generic;
using Core.Game.Chunk.Universe.Data;
using Core.Game.Storage.Data;
using GDFrameworkCore;
using Sirenix.OdinInspector;

namespace Core.Game.Storage
{
    /// <summary>
    /// 存储系统
    /// </summary>
    public class StorageSystem : AbstractSystem
    {
        /// <summary>
        /// 任何存档Key
        /// </summary>
        private const string DefaultSlotDataKeyName = "SlotDataKey_";
        
        /// <summary>
        /// 最大存档数量
        /// </summary>
        private const int MaxSlotCount = 6;

        /// <summary>
        /// 所有的存档数据
        /// </summary>
        private Dictionary<int, SaveSlotData> _slotDataDict = new Dictionary<int, SaveSlotData>();

        /// <summary>
        /// 当前存档索引
        /// </summary>
        private int _curSlotIndex = -1;

        /// <summary>
        /// 当前存档槽位
        /// </summary>
        private SaveSlotData _curSlotData;
        
        protected override void OnInit()
        {
            
        }

        #region 读

        /// <summary>
        /// 获取所有存档槽位信息
        /// </summary>
        /// <returns></returns>
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
        /// 判断当前游戏是否是全新的游戏
        /// 即是否有任何已游玩的存档
        /// </summary>
        /// <returns></returns>
        public bool HasAnySlot()
        {
            if (ES3.KeyExists(DefaultSlotDataKeyName))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 判断当前游戏是否有指定的存档槽位的存档
        /// </summary>
        /// <returns></returns>
        public bool HasAssignSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= MaxSlotCount)
            {
                UnityEngine.Debug.LogError($"无效的存档槽位: {slotIndex}");
                return false;
            }
            
            if (ES3.KeyExists(DefaultSlotDataKeyName+slotIndex))
            {
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// 获取指定槽位数据
        /// </summary>
        public SaveSlotData GetSlotData()
        {
           return GetSlotData(0);
        }
        
        /// <summary>
        /// 获取指定槽位数据
        /// </summary>
        public SaveSlotData GetSlotData(int slotIndex)
        {
            if (_slotDataDict.ContainsKey(slotIndex))
            {
                return _slotDataDict[slotIndex];
            }

            SaveSlotData slotData;
            if (HasAnySlot())
            {
                if (HasAssignSlot(slotIndex))
                {
                    string key = DefaultSlotDataKeyName + slotIndex;
                    slotData = ES3.Load<SaveSlotData>(key);
                }
                else
                {
                    slotData = new SaveSlotData(slotIndex);
                }
            }
            else
            {
                slotData = new SaveSlotData(slotIndex);
            }
            
            _slotDataDict[slotIndex] = slotData;
            return slotData;
        }

        /// <summary>
        /// 获取当前槽位的宇宙数据
        /// </summary>
        /// <returns></returns>
        public UniverseTemporaryData GetUniverseData()
        {
            UniverseTemporaryData curUniverseTemporaryDataData = new UniverseTemporaryData();
            if (_curSlotData == null)
            {
                GetSlotData();
            }

            if (_curSlotData.UniverseId == string.Empty)
            {
                
            }

            return curUniverseTemporaryDataData;
        }
        
        
        
        #endregion

        #region 存

        /// <summary>
        /// 更新当前选择的存档槽位
        /// </summary>
        /// <param name="slotIndex"></param>
        public void UpdateCurSelectingSlotIndex(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= MaxSlotCount)
            {
                UnityEngine.Debug.LogError($"无效的存档槽位: {slotIndex}");
                return;
            }
            _curSlotIndex = slotIndex;
            _curSlotData = GetSlotData(slotIndex);
        }
        
        /// <summary>
        /// 更新当前选择的存档槽位的名称
        /// </summary>
        /// <param name="slotIndex"></param>
        public void UpdateCurSelectingSlotName(int slotIndex,string name)
        {
            if (slotIndex < 0 || slotIndex >= MaxSlotCount)
            {
                UnityEngine.Debug.LogError($"无效的存档槽位: {slotIndex}");
                return;
            }

            GetSlotData(slotIndex).SlotName = name;
        }

        #endregion
    }
}