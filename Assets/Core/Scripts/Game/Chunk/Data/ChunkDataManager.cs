using System;
using System.Collections.Generic;
using System.Linq;
using Core.Game.Chunk.Data.Interface;
using UnityEngine;

namespace Core.Game.Chunk.Data
{
    public class ChunkDataManager : IChunkDataManager
    {
        public Dictionary<string, IChunkDtoDef> DefRegistry = new Dictionary<string, IChunkDtoDef>();
        public Dictionary<string, IChunkData> InstanceRegistry = new Dictionary<string, IChunkData>();
        public Dictionary<Type, Func<IChunkData>> TypeFactories = new Dictionary<Type, Func<IChunkData>>();

        public void RegisterTypeFactory<T>(Func<T> factory) where T : IChunkData
        {
            TypeFactories[typeof(T)] = () => factory();
        }
        
        #region Def

        public void RegisterDef(IChunkDtoDef def)
        {
            if (def == null)
                throw new ArgumentNullException(nameof(def));

            if (!DefRegistry.ContainsKey(def.DefId))
            {
                DefRegistry[def.DefId] = def;
            }
        }

        public void RegisterDefs(IEnumerable<IChunkDtoDef> defs)
        {
            foreach (var def in defs)
            {
                RegisterDef(def);
            }
        }

        public IChunkDtoDef GetDef(string defId)
        {
            return DefRegistry.TryGetValue(defId, out var def) ? def : null;
        }

        #endregion


        #region Instance

         public T CreateInstance<T>(string defId) where T : IChunkData
        {
            if (!DefRegistry.TryGetValue(defId, out var def))
            {
                throw new KeyNotFoundException($"找不到配置: {defId}");
            }

            T instance;
            if (TypeFactories.TryGetValue(typeof(T), out var factory))
            {
                instance = (T)factory();
            }
            else
            {
                instance = Activator.CreateInstance<T>();
            }

            instance.InitFromDef(def);
            InstanceRegistry[instance.InstanceId] = instance;

            return instance;
        }

        public T LoadInstance<T>(string instanceId) where T : IChunkData
        {
            if (InstanceRegistry.TryGetValue(instanceId, out var existing))
            {
                return (T)existing;
            }

            if (!ES3.KeyExists(instanceId))
            {
                return default(T);
            }

            var tempData = ES3.Load<ChunkTemporaryData>(instanceId);
            if (!DefRegistry.TryGetValue(tempData.DefId, out var def))
            {
                Debug.LogError($"找不到配置: {tempData.DefId}");
                return default(T);
            }

            T instance;
            if (TypeFactories.TryGetValue(typeof(T), out var factory))
            {
                instance = (T)factory();
            }
            else
            {
                instance = Activator.CreateInstance<T>();
            }

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
            return InstanceRegistry.Values
                .Where(i => i.DefId == defId)
                .ToList();
        }

        public void SaveAllInstances()
        {
            foreach (var instance in InstanceRegistry.Values)
            {
                instance.SaveTemporaryData();
            }
        }

        public void SaveInstance(string instanceId)
        {
            if (InstanceRegistry.TryGetValue(instanceId, out var instance))
            {
                instance.SaveTemporaryData();
            }
        }

        public void DestroyInstance(string instanceId, bool deleteData = false)
        {
            if (InstanceRegistry.TryGetValue(instanceId, out var instance))
            {
                if (deleteData)
                {
                    instance.DeleteTemporaryData();
                }

                InstanceRegistry.Remove(instanceId);
            }
        }

        public void ClearInstances()
        {
            InstanceRegistry.Clear();
        }

        #endregion

       
    }
}