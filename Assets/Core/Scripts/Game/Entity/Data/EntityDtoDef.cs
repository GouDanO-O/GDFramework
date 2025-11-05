using System;
using Core.Game.Chunk.Data;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Substance.Data
{
    [Serializable,JsonObject]
    public class EntityDtoDef : ChunkDtoDef
    {
        [LabelText("贴图ID")]
        public string SpriteId;

        [LabelText("尺寸")]
        public Vector2Int EntitySize = Vector2Int.one;

        public bool IsBlockingMovement = true;
        
        public int InitialHealth;
        
        public int InitialMaxHealth;

        public Vector2Int Position;
        
        public EEntityRotationType RotationType;
        
        public override string GetTypePrefix()
        {
            return "Entity";
        }
    }
}