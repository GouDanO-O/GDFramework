using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Region;
using Core.Game.Chunk.Region.Data;
using GDFrameworkExtend.JsonKit;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.World.Data
{
    [Serializable,JsonObject]
    public class WorldDtoDef : ChunkDtoDef
    {
        [LabelText("第一次进入宇宙时,当前世界是否处于解锁状态")]
        public bool IsLockInInitialUniverse;

        [LabelText("在宇宙中生成的坐标")]
        [JsonConverter(typeof(Vector2Converter))]
        public Vector2 InitialSpawnedPosition;
        
        public override string GetTypePrefix()
        {
            return "World";
        }
        
    }
}