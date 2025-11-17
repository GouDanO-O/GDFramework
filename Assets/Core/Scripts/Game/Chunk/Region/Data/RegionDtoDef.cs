using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Room;
using Core.Game.Chunk.Room.Data;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Region.Data
{
    [Serializable,JsonObject]
    public class RegionDtoDef : ChunkDtoDef
    {
        public Vector2 InitialSpawnPos;

        public bool IsLockInInitialWorld;
        
        [Title("玩家第一次进入区块所处的副本ID,如果为空,则默认取索引第一位"),LabelText("初始副本ID"),ReadOnly]
        public string initialPlayerLocateDungeonId;

        [LabelText("第一次进入区块展示的副本"),ReadOnly]
        public List<string> initialShowingStrongholdIdList = new List<string>();
        
        [LabelText("区块拥有的所有副本ID"),ReadOnly]
        public List<string> strongholdIdList  = new List<string>();
        
        public override string GetTypePrefix()
        {
            return "Region";
        }
    }
}