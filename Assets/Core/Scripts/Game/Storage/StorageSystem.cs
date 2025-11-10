using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Game.Chunk.Data.Interface;
using Core.Game.Chunk.Substance.Interface;
using Core.Game.Storage.Data;
using GDFrameworkCore;
using GDFrameworkExtend.LogKit;
using Newtonsoft.Json;
using UnityEngine;

namespace Core.Game.Storage
{
    /// <summary>
    /// 存储系统 - 两层架构
    /// 游戏配置层 (Editor路径，打包后只读) + Mod配置层 (可读写)
    /// </summary>
    public class StorageSystem : AbstractSystem
    {
        #region 常量定义
        
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
        /// 游戏配置路径 (编辑器开发路径)
        /// 打包后通过 YooAsset 加载，运行时不可写
        /// </summary>
        private static string GameConfigPath => Path.Combine(Application.dataPath, "Core/Res/Configs/ChunkData");

        /// <summary>
        /// Mod 配置路径 (运行时可读写)
        /// </summary>
        private static string ModConfigPath => Path.Combine(Application.persistentDataPath, "Mods", "ChunkData");

        #endregion

        #region 私有字段

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

        /// <summary>
        /// Def配置缓存
        /// Key: DefId, Value: Def对象
        /// </summary>
        private Dictionary<string, IChunkDtoDef> _defCache = new Dictionary<string, IChunkDtoDef>();

        /// <summary>
        /// Def 被修改标记
        /// 用于跟踪哪些 Def 需要保存
        /// </summary>
        private HashSet<string> _modifiedDefs = new HashSet<string>();

        #endregion
        
        protected override void OnInit()
        {
            // 确保 Mod 配置目录存在
            EnsureModDirectoryExists();
        }

        #region 存档槽位管理

        public List<SaveSlotData> GetAllSlots()
        {
            List<SaveSlotData> slots = new List<SaveSlotData>();
            for (int i = 0; i < MaxSlotCount; i++)
            {
                slots.Add(GetSlotData(i));
            }
            return slots;
        }

        public bool HasAnySlot()
        {
            for (int i = 0; i < MaxSlotCount; i++)
            {
                if (HasSlot(i)) return true;
            }
            return false;
        }

        public bool HasSlot(int slotIndex)
        {
            if (!IsValidSlotIndex(slotIndex)) return false;
            string key = GetSlotDataKey(slotIndex);
            return ES3.KeyExists(key);
        }

        public SaveSlotData GetSlotData(int slotIndex)
        {
            if (!IsValidSlotIndex(slotIndex))
            {
                LogKit.Error($"无效的存档槽位: {slotIndex}");
                return new SaveSlotData(slotIndex);
            }

            if (_slotDataDict.ContainsKey(slotIndex))
                return _slotDataDict[slotIndex];

            SaveSlotData slotData;
            string key = GetSlotDataKey(slotIndex);
            
            if (ES3.KeyExists(key))
                slotData = ES3.Load<SaveSlotData>(key);
            else
                slotData = new SaveSlotData(slotIndex);
            
            _slotDataDict[slotIndex] = slotData;
            return slotData;
        }

        public void SaveSlotData(int slotIndex, SaveSlotData slotData)
        {
            if (!IsValidSlotIndex(slotIndex))
            {
                LogKit.Error($"无效的存档槽位: {slotIndex}");
                return;
            }

            string key = GetSlotDataKey(slotIndex);
            ES3.Save(key, slotData);
            _slotDataDict[slotIndex] = slotData;
            
            LogKit.Log($"保存存档槽位 {slotIndex}: {slotData.SlotName}");
        }

        public void DeleteSlot(int slotIndex)
        {
            if (!IsValidSlotIndex(slotIndex))
            {
                LogKit.Error($"无效的存档槽位: {slotIndex}");
                return;
            }

            string key = GetSlotDataKey(slotIndex);
            if (ES3.KeyExists(key))
            {
                var slotData = GetSlotData(slotIndex);
                DeleteSlotTemporaryData(slotData.UniverseId);
                ES3.DeleteKey(key);
                _slotDataDict.Remove(slotIndex);
                LogKit.Log($"删除存档槽位 {slotIndex}");
            }
        }

        public void SetCurrentSlot(int slotIndex)
        {
            if (!IsValidSlotIndex(slotIndex))
            {
                LogKit.Error($"无效的存档槽位: {slotIndex}");
                return;
            }
            
            _currentSlotIndex = slotIndex;
            _currentSlotData = GetSlotData(slotIndex);
            LogKit.Log($"切换到存档槽位 {slotIndex}: {_currentSlotData.SlotName}");
        }

        public SaveSlotData GetCurrentSlotData()
        {
            if (_currentSlotData == null && _currentSlotIndex >= 0)
                _currentSlotData = GetSlotData(_currentSlotIndex);
            return _currentSlotData;
        }

        #endregion

        #region Def存储 - 核心实现

        /// <summary>
        /// 保存 Def 配置
        /// 编辑器模式：保存到 GameConfig 路径
        /// 运行时模式：保存到 Mod 路径
        /// </summary>
        public void SaveDef(IChunkDtoDef dtoDef)
        {
            if (dtoDef == null)
            {
                LogKit.Error("Def 为空，无法保存");
                return;
            }

            // 验证数据
            if (!dtoDef.Validate(out string error))
            {
                LogKit.Error($"Def 验证失败: {error}");
                return;
            }

            try
            {
#if UNITY_EDITOR
                // 编辑器模式：保存到游戏配置路径
                SaveDefToGameConfig(dtoDef);
                LogKit.Log($"<color=green>✓ 保存 Def 到游戏配置: {dtoDef.DefId} ({dtoDef.DefName})</color>");
#else
                // 运行时模式：保存到 Mod 路径
                SaveDefToMod(dtoDef);
                LogKit.Log($"<color=green>✓ 保存 Def 到 Mod: {dtoDef.DefId} ({dtoDef.DefName})</color>");
#endif

                // 更新缓存
                _defCache[dtoDef.DefId] = dtoDef;
                _modifiedDefs.Remove(dtoDef.DefId); // 保存后清除修改标记
            }
            catch (Exception e)
            {
                LogKit.Error($"保存 Def 失败: {dtoDef.DefId}, 错误: {e.Message}\n{e.StackTrace}");
            }
        }

        /// <summary>
        /// 删除 Def 配置
        /// </summary>
        public void DeleteDef(IChunkDtoDef dtoDef)
        {
            if (dtoDef == null)
            {
                LogKit.Error("Def 为空，无法删除");
                return;
            }

            try
            {
#if UNITY_EDITOR
                // 编辑器模式：从游戏配置路径删除
                DeleteDefFromGameConfig(dtoDef);
                LogKit.Log($"<color=yellow>✗ 删除游戏配置 Def: {dtoDef.DefId}</color>");
#else
                // 运行时模式：只能从 Mod 路径删除
                // 游戏配置是只读的，无法删除
                if (ExistsDefInMod(dtoDef.DefId))
                {
                    DeleteDefFromMod(dtoDef);
                    LogKit.Log($"<color=yellow>✗ 删除 Mod Def: {dtoDef.DefId}</color>");
                }
                else
                {
                    LogKit.Warning($"无法删除游戏配置 Def: {dtoDef.DefId}，运行时游戏配置是只读的");
                    return;
                }
#endif

                // 从缓存移除
                _defCache.Remove(dtoDef.DefId);
                _modifiedDefs.Remove(dtoDef.DefId);
            }
            catch (Exception e)
            {
                LogKit.Error($"删除 Def 失败: {dtoDef.DefId}, 错误: {e.Message}");
            }
        }

        /// <summary>
        /// 加载 Def 配置
        /// 优先级: Mod 路径 > 游戏配置路径
        /// </summary>
        public T LoadDef<T>(string defId) where T : class, IChunkDtoDef
        {
            if (string.IsNullOrEmpty(defId))
            {
                LogKit.Warning("DefId 为空，无法加载");
                return null;
            }

            // 优先从缓存获取
            if (_defCache.TryGetValue(defId, out var cachedDef))
            {
                return cachedDef as T;
            }

            try
            {
                T def = null;

                // 优先级1: Mod 路径（可覆盖游戏配置）
                def = LoadDefFromMod<T>(defId);
                if (def != null)
                {
                    LogKit.Log($"<color=cyan>从 Mod 加载 Def: {defId}</color>");
                }
                else
                {
                    // 优先级2: 游戏配置路径
#if UNITY_EDITOR
                    def = LoadDefFromGameConfig<T>(defId);
                    if (def != null)
                    {
                        LogKit.Log($"<color=cyan>从游戏配置加载 Def: {defId}</color>");
                    }
#else
                    // 运行时游戏配置应该由 LaunchResourcesLoader 通过 YooAsset 加载
                    // 这里不应该直接访问，而是从 DataModel 的缓存中获取
                    LogKit.Warning($"Def {defId} 不在 Mod 中，且未从游戏配置加载（应由 LaunchResourcesLoader 加载）");
#endif
                }

                if (def != null)
                {
                    // 更新缓存
                    _defCache[defId] = def;
                }

                return def;
            }
            catch (Exception e)
            {
                LogKit.Error($"加载 Def 失败: {defId}, 错误: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// 检查 Def 是否存在
        /// </summary>
        public bool ExistsDef(string defId)
        {
            if (string.IsNullOrEmpty(defId))
                return false;

            // 检查缓存
            if (_defCache.ContainsKey(defId))
                return true;

            // 检查 Mod 路径
            if (ExistsDefInMod(defId))
                return true;

#if UNITY_EDITOR
            // 编辑器：检查游戏配置路径
            if (ExistsDefInGameConfig(defId))
                return true;
#endif

            return false;
        }

        /// <summary>
        /// 标记 Def 为已修改
        /// </summary>
        public void MarkDefAsModified(string defId)
        {
            if (_defCache.ContainsKey(defId))
            {
                _modifiedDefs.Add(defId);
            }
        }

        /// <summary>
        /// 获取所有被修改的 Def
        /// </summary>
        public List<IChunkDtoDef> GetModifiedDefs()
        {
            return _modifiedDefs
                .Where(defId => _defCache.ContainsKey(defId))
                .Select(defId => _defCache[defId])
                .ToList();
        }

        /// <summary>
        /// 保存所有被修改的 Def
        /// </summary>
        public void SaveAllModifiedDefs()
        {
            var modifiedDefs = GetModifiedDefs();
            foreach (var def in modifiedDefs)
            {
                SaveDef(def);
            }
            LogKit.Log($"<color=green>批量保存: {modifiedDefs.Count} 个 Def</color>");
        }

        /// <summary>
        /// 清空 Def 缓存
        /// </summary>
        public void ClearDefCache()
        {
            _defCache.Clear();
            _modifiedDefs.Clear();
            LogKit.Log("已清空 Def 缓存");
        }

        /// <summary>
        /// 检查 Def 是否在 Mod 中（可能覆盖了游戏配置）
        /// </summary>
        public bool IsModOverride(string defId)
        {
            return ExistsDefInMod(defId);
        }

        #endregion

        #region Def文件操作 - 游戏配置路径

#if UNITY_EDITOR
        /// <summary>
        /// 保存 Def 到游戏配置路径
        /// </summary>
        private void SaveDefToGameConfig(IChunkDtoDef dtoDef)
        {
            string filePath = GetGameConfigDefFilePath(dtoDef.DefId);
            SaveDefToFilePath(dtoDef, filePath);
        }

        /// <summary>
        /// 从游戏配置路径加载 Def
        /// </summary>
        private T LoadDefFromGameConfig<T>(string defId) where T : class, IChunkDtoDef
        {
            string filePath = GetGameConfigDefFilePath(defId);
            return LoadDefFromFilePath<T>(filePath);
        }

        /// <summary>
        /// 从游戏配置路径删除 Def
        /// </summary>
        private void DeleteDefFromGameConfig(IChunkDtoDef dtoDef)
        {
            string filePath = GetGameConfigDefFilePath(dtoDef.DefId);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                LogKit.Log($"删除游戏配置文件: {filePath}");
            }
        }

        /// <summary>
        /// 检查 Def 是否存在于游戏配置路径
        /// </summary>
        private bool ExistsDefInGameConfig(string defId)
        {
            string filePath = GetGameConfigDefFilePath(defId);
            return File.Exists(filePath);
        }

        /// <summary>
        /// 获取游戏配置路径中的 Def 文件路径
        /// </summary>
        private string GetGameConfigDefFilePath(string defId)
        {
            string typePrefix = ExtractTypePrefix(defId);
            string typePath = Path.Combine(GameConfigPath, typePrefix);
            return Path.Combine(typePath, $"{defId}.json");
        }
#endif

        #endregion

        #region Def文件操作 - Mod 路径

        /// <summary>
        /// 保存 Def 到 Mod 路径
        /// </summary>
        private void SaveDefToMod(IChunkDtoDef dtoDef)
        {
            string filePath = GetModDefFilePath(dtoDef.DefId);
            SaveDefToFilePath(dtoDef, filePath);
        }

        /// <summary>
        /// 从 Mod 路径加载 Def
        /// </summary>
        private T LoadDefFromMod<T>(string defId) where T : class, IChunkDtoDef
        {
            string filePath = GetModDefFilePath(defId);
            return LoadDefFromFilePath<T>(filePath);
        }

        /// <summary>
        /// 从 Mod 路径删除 Def
        /// </summary>
        private void DeleteDefFromMod(IChunkDtoDef dtoDef)
        {
            string filePath = GetModDefFilePath(dtoDef.DefId);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                LogKit.Log($"删除 Mod 文件: {filePath}");
            }
        }

        /// <summary>
        /// 检查 Def 是否存在于 Mod 路径
        /// </summary>
        private bool ExistsDefInMod(string defId)
        {
            string filePath = GetModDefFilePath(defId);
            return File.Exists(filePath);
        }

        /// <summary>
        /// 获取 Mod 路径中的 Def 文件路径
        /// </summary>
        private string GetModDefFilePath(string defId)
        {
            string typePrefix = ExtractTypePrefix(defId);
            string typePath = Path.Combine(ModConfigPath, typePrefix);
            return Path.Combine(typePath, $"{defId}.json");
        }

        #endregion

        #region Def文件操作 - 通用方法

        /// <summary>
        /// 保存 Def 到指定文件路径
        /// </summary>
        private void SaveDefToFilePath(IChunkDtoDef dtoDef, string filePath)
        {
            // 确保目录存在
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // 序列化为 JSON
            string json = JsonConvert.SerializeObject(dtoDef, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            // 写入文件
            File.WriteAllText(filePath, json);
            LogKit.Log($"保存到文件: {filePath}");
        }

        /// <summary>
        /// 从文件路径加载 Def
        /// </summary>
        private T LoadDefFromFilePath<T>(string filePath) where T : class, IChunkDtoDef
        {
            if (!File.Exists(filePath))
                return null;

            try
            {
                string json = File.ReadAllText(filePath);
                T def = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                return def;
            }
            catch (Exception e)
            {
                LogKit.Error($"加载文件失败: {filePath}, 错误: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// 从 DefId 提取类型前缀
        /// 格式: Universe_DEF_12345678 -> Universe
        /// </summary>
        private string ExtractTypePrefix(string defId)
        {
            if (string.IsNullOrEmpty(defId))
                return "Unknown";

            int index = defId.IndexOf("_DEF_");
            if (index > 0)
                return defId.Substring(0, index);

            return "Unknown";
        }

        /// <summary>
        /// 确保 Mod 配置目录存在
        /// </summary>
        private void EnsureModDirectoryExists()
        {
            if (!Directory.Exists(ModConfigPath))
            {
                Directory.CreateDirectory(ModConfigPath);
                LogKit.Log($"创建 Mod 配置目录: {ModConfigPath}");
            }
        }

        #endregion

        #region 临时数据管理

        public void SaveChunkTemporaryData(string defId, IChunkTemporaryData tempData)
        {
            if (string.IsNullOrEmpty(defId))
            {
                LogKit.Error("DefId 为空,无法保存临时数据");
                return;
            }

            if (tempData == null)
            {
                LogKit.Error($"临时数据为空,无法保存: {defId}");
                return;
            }

            tempData.LastModifyTime = DateTime.Now;
            tempData.DefId = defId;

            string key = GetTempDataKey(defId);
            ES3.Save(key, tempData);
            
            LogKit.Log($"<color=cyan>保存临时数据: {defId}</color>");
        }

        public IChunkTemporaryData LoadChunkTemporaryData(string defId, Type type)
        {
            if (string.IsNullOrEmpty(defId))
            {
                LogKit.Warning("DefId 为空,无法加载临时数据");
                return null;
            }

            string key = GetTempDataKey(defId);
            
            if (ES3.KeyExists(key))
            {
                try
                {
                    var tempData = ES3.Load(key, type) as IChunkTemporaryData;
                    LogKit.Log($"<color=cyan>加载临时数据: {defId}</color>");
                    return tempData;
                }
                catch (Exception e)
                {
                    LogKit.Error($"加载临时数据失败 {defId}: {e.Message}");
                    return null;
                }
            }

            return null;
        }

        public void SaveEntityTemporaryData(string instanceId, IEntityTemporaryData tempData)
        {
            if (string.IsNullOrEmpty(instanceId))
            {
                LogKit.Error("instanceId 为空,无法保存临时数据");
                return;
            }

            if (tempData == null)
            {
                LogKit.Error($"临时数据为空,无法保存: {instanceId}");
                return;
            }

            tempData.LastModifyTime = DateTime.Now;
            tempData.EntityInstanceId = instanceId;

            string key = GetTempDataKey(instanceId);
            ES3.Save(key, tempData);
            
            LogKit.Log($"<color=cyan>保存临时数据: {instanceId}</color>");
        }
        
        public IEntityTemporaryData LoadEntityTemporaryData(string instanceId, Type type)
        {
            if (string.IsNullOrEmpty(instanceId))
            {
                LogKit.Warning("instanceId 为空,无法加载临时数据");
                return null;
            }

            string key = GetTempDataKey(instanceId);
            
            if (ES3.KeyExists(key))
            {
                try
                {
                    var tempData = ES3.Load(key, type) as IEntityTemporaryData;
                    LogKit.Log($"<color=cyan>加载临时数据: {instanceId}</color>");
                    return tempData;
                }
                catch (Exception e)
                {
                    LogKit.Error($"加载临时数据失败 {instanceId}: {e.Message}");
                    return null;
                }
            }

            return null;
        }

        public T LoadTemporaryData<T>(string defId) where T : class, IChunkTemporaryData
        {
            if (string.IsNullOrEmpty(defId))
            {
                LogKit.Warning("DefId 为空,无法加载临时数据");
                return null;
            }

            string key = GetTempDataKey(defId);
            
            if (ES3.KeyExists(key))
            {
                try
                {
                    T tempData = ES3.Load<T>(key);
                    LogKit.Log($"<color=cyan>加载临时数据: {defId}</color>");
                    return tempData;
                }
                catch (Exception e)
                {
                    LogKit.Error($"加载临时数据失败 {defId}: {e.Message}");
                    return null;
                }
            }

            return null;
        }
        
        public void DeleteTemporaryData(string defId)
        {
            if (string.IsNullOrEmpty(defId))
            {
                LogKit.Warning("DefId 为空,无法删除临时数据");
                return;
            }

            string key = GetTempDataKey(defId);
            
            if (ES3.KeyExists(key))
            {
                ES3.DeleteKey(key);
                LogKit.Log($"<color=yellow>删除临时数据: {defId}</color>");
            }
        }

        public bool ExistsTemporaryData(string defId)
        {
            if (string.IsNullOrEmpty(defId))
                return false;

            string key = GetTempDataKey(defId);
            return ES3.KeyExists(key);
        }

        public List<string> GetAllTemporaryDataKeys()
        {
            if (_currentSlotData == null)
                return new List<string>();

            string prefix = GetTempDataKeyPrefix();
            string[] allKeys = ES3.GetKeys();
            
            return allKeys
                .Where(key => key.StartsWith(prefix))
                .Select(key => key.Substring(prefix.Length))
                .ToList();
        }

        private void DeleteSlotTemporaryData(string universeId)
        {
            if (string.IsNullOrEmpty(universeId))
                return;

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
            
            LogKit.Log($"删除槽位 {universeId} 的 {deleteCount} 条临时数据");
        }

        #endregion

        #region 工具方法

        private bool IsValidSlotIndex(int slotIndex)
        {
            return slotIndex >= 0 && slotIndex < MaxSlotCount;
        }

        private string GetSlotDataKey(int slotIndex)
        {
            return $"{SlotDataKeyPrefix}{slotIndex}";
        }

        private string GetTempDataKeyPrefix()
        {
            if (_currentSlotData == null)
                return TempDataKeyPrefix;
            
            return $"{TempDataKeyPrefix}{_currentSlotData.UniverseId}_";
        }

        private string GetTempDataKey(string defId)
        {
            return $"{GetTempDataKeyPrefix()}{defId}";
        }

        #endregion
    }
}