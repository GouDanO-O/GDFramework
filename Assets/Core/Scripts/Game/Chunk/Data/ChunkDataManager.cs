using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Game.Chunk.Data.Interface;
using Newtonsoft.Json;
using UnityEngine;

namespace Core.Game.Chunk.Data
{
    /// <summary>
    /// 数据管理器
    /// </summary>
    public class ChunkDataManager : IChunkDataManager
    {
        /// <summary>
        /// Def字典
        /// </summary>
        protected Dictionary<string, IChunkDtoDef> DefRegistry = new Dictionary<string, IChunkDtoDef>();
        
        /// <summary>
        /// 数据字典
        /// </summary>
        protected Dictionary<string, IChunkData> InstanceRegistry = new Dictionary<string, IChunkData>();
        
        protected Dictionary<Type, Func<IChunkData>> TypeFactories = new Dictionary<Type, Func<IChunkData>>();

        public Dictionary<string, IChunkData> GetInstanceRegistry()
        {
            return InstanceRegistry;
        }
        
        public void RegisterTypeFactory<T>(Func<T> factory) where T : IChunkData
        {
            TypeFactories[typeof(T)] = () => factory();
        }

        public void RegisterDef(IChunkDtoDef def)
        {
            if (def == null) 
                throw new ArgumentNullException(nameof(def));
            DefRegistry[def.DefId] = def;
        }

        public void RegisterDefs(IEnumerable<IChunkDtoDef> defs)
        {
            foreach (var def in defs) 
                RegisterDef(def);
        }

        /// <summary>
        /// 从指定目录加载所有JSON配置
        /// </summary>
        public void LoadAllDefsFromDirectory<T>(string directoryPath) where T : ChunkDtoDef
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                {
                    Debug.LogWarning($"配置目录不存在: {directoryPath}");
                    return;
                }

                var jsonFiles = Directory.GetFiles(directoryPath, "*.json", SearchOption.AllDirectories);

                foreach (var filePath in jsonFiles)
                {
                    try
                    {
                        string json = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
                        var def = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.Auto
                        });

                        if (def != null)
                        {
                            RegisterDef(def);
                            Debug.Log($"已加载配置: {def.DefId} from {filePath}");
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"加载配置失败 {filePath}: {e.Message}");
                    }
                }

                Debug.Log($"从 {directoryPath} 加载了 {jsonFiles.Length} 个配置");
            }
            catch (Exception e)
            {
                Debug.LogError($"加载配置目录失败: {e.Message}");
            }
        }

        /// <summary>
        /// 从多个路径加载配置(优先级: 运行时路径 > StreamingAssets > Assets)
        /// </summary>
        public void LoadAllDefsFromMultiplePaths<T>() where T : ChunkDtoDef
        {
            var temp = Activator.CreateInstance<T>();
            string typePrefix = temp.GetTypePrefix();

            // 1. 优先从运行时路径加载(用户修改的配置)
            string runtimePath = Path.Combine(Application.persistentDataPath, "ChunkData", typePrefix);
            if (Directory.Exists(runtimePath))
            {
                LoadAllDefsFromDirectory<T>(runtimePath);
            }

#if UNITY_EDITOR
            // 2. 编辑器模式下从Assets路径加载
            string editorPath = Path.Combine("Assets/Core/Resources/ChunkData", typePrefix);
            if (Directory.Exists(editorPath))
            {
                LoadAllDefsFromDirectory<T>(editorPath);
            }
#else
            // 3. 打包后从StreamingAssets加载初始配置
            string streamingPath = Path.Combine(Application.streamingAssetsPath, "ChunkData", typePrefix);
            if (Directory.Exists(streamingPath))
            {
                LoadAllDefsFromDirectory<T>(streamingPath);
                
                // 首次启动时,将StreamingAssets的配置复制到持久化路径
                CopyInitialConfigsToRuntime(streamingPath, runtimePath);
            }
#endif
        }

        /// <summary>
        /// 首次启动时复制初始配置
        /// </summary>
        private void CopyInitialConfigsToRuntime(string sourcePath, string targetPath)
        {
            try
            {
                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);

                    var files = Directory.GetFiles(sourcePath, "*.json", SearchOption.AllDirectories);
                    foreach (var sourceFile in files)
                    {
                        string relativePath = sourceFile.Substring(sourcePath.Length + 1);
                        string targetFile = Path.Combine(targetPath, relativePath);

                        string targetDir = Path.GetDirectoryName(targetFile);
                        if (!Directory.Exists(targetDir))
                        {
                            Directory.CreateDirectory(targetDir);
                        }

                        File.Copy(sourceFile, targetFile, false);
                    }

                    Debug.Log($"已复制初始配置到: {targetPath}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"复制初始配置失败: {e.Message}");
            }
        }

        /// <summary>
        /// 保存指定的Def配置
        /// </summary>
        public void SaveDef(string defId)
        {
            if (DefRegistry.TryGetValue(defId, out var def))
            {
                if (def is ChunkDtoDef chunkDef)
                {
                    chunkDef.SaveThisDef();
                }
            }
        }

        /// <summary>
        /// 保存所有Def配置
        /// </summary>
        public void SaveAllDefs()
        {
            foreach (var def in DefRegistry.Values)
            {
                if (def is ChunkDtoDef chunkDef)
                {
                    chunkDef.SaveThisDef();
                }
            }

            Debug.Log($"已保存 {DefRegistry.Count} 个配置");
        }

        public IChunkDtoDef GetDef(string defId)
        {
            return DefRegistry.TryGetValue(defId, out var def) ? def : null;
        }

        public T CreateInstance<T>(string defId) where T : IChunkData
        {
            if (!DefRegistry.TryGetValue(defId, out var def))
                throw new KeyNotFoundException($"找不到配置: {defId}");

            T instance = TypeFactories.TryGetValue(typeof(T), out var factory)
                ? (T)factory()
                : Activator.CreateInstance<T>();

            instance.InitFromDef(def);
            InstanceRegistry[instance.InstanceId] = instance;

            return instance;
        }

        public T LoadInstance<T>(string instanceId) where T : IChunkData
        {
            if (InstanceRegistry.TryGetValue(instanceId, out var existing))
                return (T)existing;

            if (!ES3.KeyExists(instanceId))
                return default(T);

            var tempData = ES3.Load<ChunkTemporaryData>(instanceId);
            if (!DefRegistry.TryGetValue(tempData.DefId, out var def))
            {
                Debug.LogError($"找不到配置: {tempData.DefId}");
                return default(T);
            }

            T instance = TypeFactories.TryGetValue(typeof(T), out var factory)
                ? (T)factory()
                : Activator.CreateInstance<T>();

            instance.InitFromInstanceId(instanceId, def);
            InstanceRegistry[instanceId] = instance;

            return instance;
        }

        public IChunkData GetInstance(string instanceId)
        {
            return InstanceRegistry.TryGetValue(instanceId, out var instance) ? instance : null;
        }

        public List<IChunkData> GetInstancesByDefId(string defId)
        {
            return InstanceRegistry.Values.Where(i => i.DefId == defId).ToList();
        }

        public List<string> GetAllInstanceIds()
        {
            return InstanceRegistry.Keys.ToList();
        }

        public void SaveAllInstances()
        {
            foreach (var instance in InstanceRegistry.Values)
                instance.SaveTemporaryData();
        }

        public void SaveInstance(string instanceId)
        {
            if (InstanceRegistry.TryGetValue(instanceId, out var instance))
                instance.SaveTemporaryData();
        }

        public void DestroyInstance(string instanceId, bool deleteData = false)
        {
            if (InstanceRegistry.TryGetValue(instanceId, out var instance))
            {
                if (deleteData) instance.DeleteTemporaryData();
                InstanceRegistry.Remove(instanceId);
            }
        }

        public void ClearInstances()
        {
            InstanceRegistry.Clear();
        }
    }
}