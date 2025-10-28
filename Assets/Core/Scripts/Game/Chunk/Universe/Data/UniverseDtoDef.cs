using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Region;
using Core.Game.Chunk.World;
using Core.Game.Chunk.World.Data;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Universe.Data
{
    [Serializable]
    public class UniverseDtoDef : ChunkDtoDef
    {
        [LabelText("初始玩家所处的世界ID"),ReadOnly]
        [InfoBox("无特殊事件的情况下,玩家会处于的第一个世界的ID")]
        public string initialPlayerLocateWorldId;
        
        [LabelText("第一次进入宇宙展示的世界"),ReadOnly]
        public List<string> initialShowingWorldIdList;
        
        [LabelText("宇宙拥有的所有世界的ID"), ReadOnly]
        public List<string> worldIdList = new List<string>();
        
        protected override string GetTypePrefix()
        {
            return "Universe";
        }
    }
}