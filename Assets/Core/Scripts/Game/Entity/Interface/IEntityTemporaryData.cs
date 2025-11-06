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
        string DefId { get; set; }
        DateTime CreateTime { get; set; }
        DateTime LastModifyTime { get; set; }

        int CurrentHealth { get; set; }
        int CurrentMaxHealth { get; set; }
        Vector2Int CurrentPosition { get; set; }
        EEntityRotationType CurrentRotationType { get; set; }

        void CreateEntityTempData(string defId, [CanBeNull] string instanceId);
        void SaveTempData();
    }
}