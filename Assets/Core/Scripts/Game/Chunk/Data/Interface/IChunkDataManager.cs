using System;
using System.Collections.Generic;

namespace Core.Game.Chunk.Data.Interface
{
    public interface IChunkDataManager
    {
        void RegisterDef(IChunkDtoDef def);
        void RegisterDefs(IEnumerable<IChunkDtoDef> defs);
        void RegisterTypeFactory<T>(Func<T> factory) where T : IChunkData;
        
        IChunkDtoDef GetDef(string defId);
        IChunkData GetInstance(string instanceId);
        List<IChunkData> GetInstancesByDefId(string defId);
        
        T CreateInstance<T>(string defId) where T : IChunkData;
        T LoadInstance<T>(string instanceId) where T : IChunkData;
        
        void SaveInstance(string instanceId);
        void SaveAllInstances();
        void DestroyInstance(string instanceId, bool deleteData = false);
        void ClearInstances();
    }
}