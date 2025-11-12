using System;
using Core.Game.Chunk.Substance.Data;
using GDFrameworkExtend.Data;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.Game.Chunk.Substance.Interface
{
    public interface IEntityTemporaryData : ITemporaryData
    {
        string EntityInstanceId { get; set; }
        
        DateTime CreateTime { get; set; }
        
        DateTime LastModifyTime { get; set; }

        void CreateEntityTempData(string defId, [CanBeNull] string instanceId);
        
        void SaveTempData();
    }
}