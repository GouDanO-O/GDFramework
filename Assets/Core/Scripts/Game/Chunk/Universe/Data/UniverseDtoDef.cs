using System;
using System.Collections.Generic;
using System.IO;
using Core.Game.Chunk.Data;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Universe.Data
{
    [Serializable,JsonObject]
    public class UniverseDtoDef : ChunkDtoDef
    {
        [LabelText("初始玩家所处的世界ID")]
        [InfoBox("无特殊事件的情况下,玩家会处于的第一个世界的ID")]
        public string InitialPlayerLocateWorldId;
        
        [LabelText("第一次进入宇宙展示的世界")]
        public List<string> InitialShowingWorldIdList;
        
        [LabelText("宇宙拥有的所有世界的ID")]
        public List<string> WorldIdList = new List<string>();

        
        public override string GetTypePrefix()
        {
            return "Universe";
        }

        public void ChangeWorldName(string newName)
        {
            this.DefName = newName;
        }
    }
}