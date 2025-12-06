using System;
using System.IO;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Room.Grid
{
    /// <summary>
    /// 房间存档数据
    /// </summary>
    [Serializable]
    public class RoomSaveData
    {
        /// <summary>
        /// 存档版本
        /// </summary>
        [JsonProperty("version")]
        public string Version = "1.0";

        /// <summary>
        /// 存档名称
        /// </summary>
        [JsonProperty("saveName")]
        public string SaveName;

        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonProperty("createTime")]
        public DateTime CreateTime;

        /// <summary>
        /// 最后修改时间
        /// </summary>
        [JsonProperty("lastModifiedTime")]
        public DateTime LastModifiedTime;

        /// <summary>
        /// 网格配置
        /// </summary>
        [JsonProperty("config")]
        public RoomGridConfig Config;

        /// <summary>
        /// 网格数据（序列化后的RoomGrid）
        /// </summary>
        [JsonProperty("gridData")]
        public string GridData;

        /// <summary>
        /// 缩略图（Base64编码）
        /// </summary>
        [JsonProperty("thumbnail")]
        public string ThumbnailBase64;

        /// <summary>
        /// 自定义元数据
        /// </summary>
        [JsonProperty("metadata")]
        public System.Collections.Generic.Dictionary<string, string> Metadata = 
            new System.Collections.Generic.Dictionary<string, string>();
    }

    /// <summary>
    /// 房间网格存档系统
    /// 负责保存和加载房间数据
    /// </summary>
    public class RoomGridSaveSystem
    {
        #region 单例

        private static RoomGridSaveSystem _instance;
        public static RoomGridSaveSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RoomGridSaveSystem();
                }
                return _instance;
            }
        }

        #endregion

        #region 属性

        /// <summary>
        /// 存档目录
        /// </summary>
        public string SaveDirectory { get; private set; }

        /// <summary>
        /// 存档文件扩展名
        /// </summary>
        public const string FILE_EXTENSION = ".roomsave";

        /// <summary>
        /// 自动保存间隔（秒）
        /// </summary>
        public float AutoSaveInterval { get; set; } = 300f; // 5分钟

        /// <summary>
        /// 是否启用自动保存
        /// </summary>
        public bool AutoSaveEnabled { get; set; } = true;

        #endregion

        #region 事件

        /// <summary>
        /// 保存完成事件
        /// </summary>
        public event Action<string, bool> OnSaveCompleted;

        /// <summary>
        /// 加载完成事件
        /// </summary>
        public event Action<RoomSaveData, bool> OnLoadCompleted;

        #endregion

        #region 初始化

        private RoomGridSaveSystem()
        {
            // 设置默认存档目录
            SaveDirectory = Path.Combine(Application.persistentDataPath, "RoomSaves");
            
            // 确保目录存在
            if (!Directory.Exists(SaveDirectory))
            {
                Directory.CreateDirectory(SaveDirectory);
            }
        }

        /// <summary>
        /// 设置存档目录
        /// </summary>
        public void SetSaveDirectory(string path)
        {
            SaveDirectory = path;
            if (!Directory.Exists(SaveDirectory))
            {
                Directory.CreateDirectory(SaveDirectory);
            }
        }

        #endregion

        #region 保存

        /// <summary>
        /// 保存房间网格
        /// </summary>
        public bool Save(RoomGrid grid, string saveName, Texture2D thumbnail = null)
        {
            if (grid == null)
            {
                Debug.LogError("[RoomSave] Grid为空，无法保存");
                return false;
            }

            try
            {
                // 创建存档数据
                var saveData = new RoomSaveData
                {
                    Version = "1.0",
                    SaveName = saveName,
                    CreateTime = DateTime.Now,
                    LastModifiedTime = DateTime.Now,
                    Config = grid.Config,
                    GridData = SerializeGrid(grid)
                };

                // 保存缩略图
                if (thumbnail != null)
                {
                    saveData.ThumbnailBase64 = Convert.ToBase64String(thumbnail.EncodeToPNG());
                }

                // 序列化并保存到文件
                string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
                string filePath = GetSaveFilePath(saveName);
                File.WriteAllText(filePath, json);

                Debug.Log($"[RoomSave] 保存成功: {filePath}");
                OnSaveCompleted?.Invoke(saveName, true);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[RoomSave] 保存失败: {e.Message}");
                OnSaveCompleted?.Invoke(saveName, false);
                return false;
            }
        }

        /// <summary>
        /// 保存到指定路径
        /// </summary>
        public bool SaveToPath(RoomGrid grid, string fullPath, Texture2D thumbnail = null)
        {
            if (grid == null)
            {
                Debug.LogError("[RoomSave] Grid为空，无法保存");
                return false;
            }

            try
            {
                var saveData = new RoomSaveData
                {
                    Version = "1.0",
                    SaveName = Path.GetFileNameWithoutExtension(fullPath),
                    CreateTime = DateTime.Now,
                    LastModifiedTime = DateTime.Now,
                    Config = grid.Config,
                    GridData = SerializeGrid(grid)
                };

                if (thumbnail != null)
                {
                    saveData.ThumbnailBase64 = Convert.ToBase64String(thumbnail.EncodeToPNG());
                }

                string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
                File.WriteAllText(fullPath, json);

                Debug.Log($"[RoomSave] 保存成功: {fullPath}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[RoomSave] 保存失败: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// 快速保存（使用默认名称）
        /// </summary>
        public bool QuickSave(RoomGrid grid)
        {
            string saveName = $"QuickSave_{DateTime.Now:yyyyMMdd_HHmmss}";
            return Save(grid, saveName);
        }

        /// <summary>
        /// 自动保存
        /// </summary>
        public bool AutoSave(RoomGrid grid)
        {
            return Save(grid, "AutoSave");
        }

        #endregion

        #region 加载

        /// <summary>
        /// 加载房间存档
        /// </summary>
        public RoomSaveData Load(string saveName)
        {
            string filePath = GetSaveFilePath(saveName);
            return LoadFromPath(filePath);
        }

        /// <summary>
        /// 从指定路径加载
        /// </summary>
        public RoomSaveData LoadFromPath(string fullPath)
        {
            if (!File.Exists(fullPath))
            {
                Debug.LogError($"[RoomSave] 存档文件不存在: {fullPath}");
                OnLoadCompleted?.Invoke(null, false);
                return null;
            }

            try
            {
                string json = File.ReadAllText(fullPath);
                var saveData = JsonConvert.DeserializeObject<RoomSaveData>(json);
                
                Debug.Log($"[RoomSave] 加载成功: {fullPath}");
                OnLoadCompleted?.Invoke(saveData, true);
                return saveData;
            }
            catch (Exception e)
            {
                Debug.LogError($"[RoomSave] 加载失败: {e.Message}");
                OnLoadCompleted?.Invoke(null, false);
                return null;
            }
        }

        /// <summary>
        /// 从存档数据恢复网格
        /// </summary>
        public RoomGrid RestoreGrid(RoomSaveData saveData)
        {
            if (saveData == null || string.IsNullOrEmpty(saveData.GridData))
            {
                Debug.LogError("[RoomSave] 存档数据无效");
                return null;
            }

            try
            {
                return DeserializeGrid(saveData.GridData);
            }
            catch (Exception e)
            {
                Debug.LogError($"[RoomSave] 恢复网格失败: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// 加载缩略图
        /// </summary>
        public Texture2D LoadThumbnail(RoomSaveData saveData)
        {
            if (saveData == null || string.IsNullOrEmpty(saveData.ThumbnailBase64))
            {
                return null;
            }

            try
            {
                byte[] bytes = Convert.FromBase64String(saveData.ThumbnailBase64);
                var texture = new Texture2D(2, 2);
                texture.LoadImage(bytes);
                return texture;
            }
            catch (Exception e)
            {
                Debug.LogError($"[RoomSave] 加载缩略图失败: {e.Message}");
                return null;
            }
        }

        #endregion

        #region 存档管理

        /// <summary>
        /// 获取所有存档
        /// </summary>
        public RoomSaveData[] GetAllSaves()
        {
            var saves = new System.Collections.Generic.List<RoomSaveData>();

            if (!Directory.Exists(SaveDirectory))
            {
                return saves.ToArray();
            }

            var files = Directory.GetFiles(SaveDirectory, $"*{FILE_EXTENSION}");
            foreach (var file in files)
            {
                try
                {
                    string json = File.ReadAllText(file);
                    var saveData = JsonConvert.DeserializeObject<RoomSaveData>(json);
                    if (saveData != null)
                    {
                        saves.Add(saveData);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[RoomSave] 读取存档失败: {file}, {e.Message}");
                }
            }

            // 按最后修改时间排序
            saves.Sort((a, b) => b.LastModifiedTime.CompareTo(a.LastModifiedTime));
            return saves.ToArray();
        }

        /// <summary>
        /// 获取存档信息（不加载完整数据）
        /// </summary>
        public RoomSaveData GetSaveInfo(string saveName)
        {
            string filePath = GetSaveFilePath(saveName);
            if (!File.Exists(filePath))
            {
                return null;
            }

            try
            {
                string json = File.ReadAllText(filePath);
                // 只反序列化基本信息，不包含GridData
                var saveData = JsonConvert.DeserializeObject<RoomSaveData>(json);
                saveData.GridData = null; // 清除大数据以节省内存
                return saveData;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 删除存档
        /// </summary>
        public bool Delete(string saveName)
        {
            string filePath = GetSaveFilePath(saveName);
            
            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"[RoomSave] 存档不存在: {saveName}");
                return false;
            }

            try
            {
                File.Delete(filePath);
                Debug.Log($"[RoomSave] 删除成功: {saveName}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[RoomSave] 删除失败: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// 检查存档是否存在
        /// </summary>
        public bool Exists(string saveName)
        {
            return File.Exists(GetSaveFilePath(saveName));
        }

        /// <summary>
        /// 重命名存档
        /// </summary>
        public bool Rename(string oldName, string newName)
        {
            string oldPath = GetSaveFilePath(oldName);
            string newPath = GetSaveFilePath(newName);

            if (!File.Exists(oldPath))
            {
                Debug.LogError($"[RoomSave] 原存档不存在: {oldName}");
                return false;
            }

            if (File.Exists(newPath))
            {
                Debug.LogError($"[RoomSave] 目标存档已存在: {newName}");
                return false;
            }

            try
            {
                // 读取并更新存档
                var saveData = LoadFromPath(oldPath);
                saveData.SaveName = newName;
                saveData.LastModifiedTime = DateTime.Now;

                // 保存到新位置
                string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
                File.WriteAllText(newPath, json);

                // 删除旧文件
                File.Delete(oldPath);

                Debug.Log($"[RoomSave] 重命名成功: {oldName} -> {newName}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[RoomSave] 重命名失败: {e.Message}");
                return false;
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取存档文件完整路径
        /// </summary>
        public string GetSaveFilePath(string saveName)
        {
            // 清理文件名中的非法字符
            string cleanName = string.Join("_", saveName.Split(Path.GetInvalidFileNameChars()));
            return Path.Combine(SaveDirectory, cleanName + FILE_EXTENSION);
        }

        /// <summary>
        /// 序列化网格
        /// </summary>
        private string SerializeGrid(RoomGrid grid)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None
            };
            return JsonConvert.SerializeObject(grid, settings);
        }

        /// <summary>
        /// 反序列化网格
        /// </summary>
        private RoomGrid DeserializeGrid(string json)
        {
            return JsonConvert.DeserializeObject<RoomGrid>(json);
        }

        /// <summary>
        /// 导出为JSON字符串
        /// </summary>
        public string ExportToJson(RoomGrid grid, bool formatted = true)
        {
            if (grid == null) return null;
            
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = formatted ? Formatting.Indented : Formatting.None
            };
            return JsonConvert.SerializeObject(grid, settings);
        }

        /// <summary>
        /// 从JSON字符串导入
        /// </summary>
        public RoomGrid ImportFromJson(string json)
        {
            if (string.IsNullOrEmpty(json)) return null;
            return JsonConvert.DeserializeObject<RoomGrid>(json);
        }

        #endregion
    }
}