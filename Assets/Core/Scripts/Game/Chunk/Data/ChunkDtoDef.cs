using System;
using System.IO;
using Core.Game.Chunk.Data.Interface;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Data
{
    [Serializable,JsonObject]
    public abstract class ChunkDtoDef : IChunkDtoDef
    {
        [LabelText("配置ID"), ReadOnly]
        [InfoBox("这是配置的唯一标识,多个实例可以共享同一个配置")]
        public string DefId { get; protected set; }

        [LabelText("配置名称")]
        public string DefName { get; set; }
        
        [LabelText("配置描述")]
        public string DefDescription { get; set; }

        /// <summary>
        /// 编辑器内配置路径(打包前)
        /// </summary>
        protected virtual string EditorDefPath => "Assets/Core/Res/Config/ChunkData";
        
        /// <summary>
        /// 运行时配置路径(打包后,使用持久化路径)
        /// </summary>
        protected virtual string RuntimeDefPath => Path.Combine(Application.persistentDataPath, "ChunkData");

        public ChunkDtoDef()
        {
            GenerateDefId();
        }

        private void GenerateDefId()
        {
            string typePrefix = GetTypePrefix();
            string uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            DefId = $"{typePrefix}_DEF_{uniqueId}";
        }
        
        public abstract string GetTypePrefix();

        /// <summary>
        /// 获取文件名
        /// </summary>
        protected virtual string GetFileName()
        {
            return $"{DefId}.json";
        }

        /// <summary>
        /// 获取完整路径
        /// </summary>
        protected virtual string GetFullPath()
        {
#if UNITY_EDITOR
            // 编辑器内使用Assets路径
            string folderPath = Path.Combine(EditorDefPath, GetTypePrefix());
            return Path.Combine(folderPath, GetFileName());
#else
            // 运行时使用持久化路径
            string folderPath = Path.Combine(RuntimeDefPath, GetTypePrefix());
            return Path.Combine(folderPath, GetFileName());
#endif
        }

        /// <summary>
        /// 深拷贝
        /// </summary>
        public virtual IChunkDtoDef Clone()
        {
            string json = JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return JsonConvert.DeserializeObject<ChunkDtoDef>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
        }
        
        public virtual bool Validate(out string error)
        {
            if (string.IsNullOrEmpty(DefId))
            {
                error = "配置ID不能为空";
                return false;
            }
            error = string.Empty;
            return true;
        }

        /// <summary>
        /// 保存配置到JSON
        /// </summary>
        public virtual void SaveThisDef()
        {
            try
            {
                // 验证数据
                if (!Validate(out string error))
                {
                    Debug.LogError($"数据验证失败: {error}");
                    return;
                }

                string fullPath = GetFullPath();
                string directory = Path.GetDirectoryName(fullPath);

                // 确保目录存在
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // 序列化为JSON
                string json = JsonConvert.SerializeObject(this, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Formatting = Formatting.Indented,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                // 写入文件
                File.WriteAllText(fullPath, json, System.Text.Encoding.UTF8);

#if UNITY_EDITOR
                // 编辑器模式下刷新资源
                UnityEditor.AssetDatabase.Refresh();
#endif

                OnDefSaved(fullPath);
                Debug.Log($"配置已保存: {fullPath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"保存配置失败: {e.Message}\n{e.StackTrace}");
            }
        }

        /// <summary>
        /// 从JSON加载配置
        /// </summary>
        public static T LoadDefFromJson<T>(string defId) where T : ChunkDtoDef
        {
            try
            {
                // 先尝试从运行时路径加载(优先级更高,用于运行时修改的配置)
                string runtimePath = GetRuntimePath<T>(defId);
                if (File.Exists(runtimePath))
                {
                    return LoadFromPath<T>(runtimePath);
                }

#if UNITY_EDITOR
                // 编辑器模式下尝试从Assets路径加载
                string editorPath = GetEditorPath<T>(defId);
                if (File.Exists(editorPath))
                {
                    return LoadFromPath<T>(editorPath);
                }
#else
                // 打包后尝试从StreamingAssets加载(首次启动时的初始配置)
                string streamingPath = GetStreamingAssetsPath<T>(defId);
                if (File.Exists(streamingPath))
                {
                    var def = LoadFromPath<T>(streamingPath);
                    // 复制到持久化路径供后续修改
                    CopyToRuntimePath(streamingPath, runtimePath);
                    return def;
                }
#endif

                Debug.LogWarning($"找不到配置文件: {defId}");
                return null;
            }
            catch (Exception e)
            {
                Debug.LogError($"加载配置失败 {defId}: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// 从指定路径加载
        /// </summary>
        private static T LoadFromPath<T>(string path) where T : ChunkDtoDef
        {
            string json = File.ReadAllText(path, System.Text.Encoding.UTF8);
            return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
        }

        /// <summary>
        /// 获取编辑器路径
        /// </summary>
        private static string GetEditorPath<T>(string defId) where T : ChunkDtoDef
        {
            var temp = Activator.CreateInstance<T>();
            string typePrefix = temp.GetTypePrefix();
            return Path.Combine("Assets/Core/Resources/ChunkData", typePrefix, $"{defId}.json");
        }

        /// <summary>
        /// 获取运行时路径
        /// </summary>
        private static string GetRuntimePath<T>(string defId) where T : ChunkDtoDef
        {
            var temp = Activator.CreateInstance<T>();
            string typePrefix = temp.GetTypePrefix();
            return Path.Combine(Application.persistentDataPath, "ChunkData", typePrefix, $"{defId}.json");
        }

        /// <summary>
        /// 获取StreamingAssets路径
        /// </summary>
        private static string GetStreamingAssetsPath<T>(string defId) where T : ChunkDtoDef
        {
            var temp = Activator.CreateInstance<T>();
            string typePrefix = temp.GetTypePrefix();
            return Path.Combine(Application.streamingAssetsPath, "ChunkData", typePrefix, $"{defId}.json");
        }

        /// <summary>
        /// 复制到运行时路径
        /// </summary>
        private static void CopyToRuntimePath(string sourcePath, string targetPath)
        {
            try
            {
                string directory = Path.GetDirectoryName(targetPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                File.Copy(sourcePath, targetPath, true);
            }
            catch (Exception e)
            {
                Debug.LogError($"复制配置文件失败: {e.Message}");
            }
        }

        /// <summary>
        /// 删除配置文件
        /// </summary>
        public virtual void DeleteThisDef()
        {
            try
            {
                string fullPath = GetFullPath();
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
#if UNITY_EDITOR
                    UnityEditor.AssetDatabase.Refresh();
#endif
                    Debug.Log($"配置已删除: {fullPath}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"删除配置失败: {e.Message}");
            }
        }

        /// <summary>
        /// 保存后的回调
        /// </summary>
        protected virtual void OnDefSaved(string path) { }
    }
}