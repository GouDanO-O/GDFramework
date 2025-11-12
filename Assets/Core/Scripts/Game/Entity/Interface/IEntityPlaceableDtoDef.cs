using Core.Game.Chunk.Substance.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Substance.Interface
{
    public interface IEntityPlaceableDtoDef
    {
        Vector2Int EntitySize { get; set; }
        
        bool IsBlockingMovement { get; set; }
    }
}