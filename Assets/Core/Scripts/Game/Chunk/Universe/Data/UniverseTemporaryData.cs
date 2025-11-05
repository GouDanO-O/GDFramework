using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Data.Interface;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Universe.Data
{
    [Serializable]
    public class UniverseTemporaryData : ChunkTemporaryData
    {
        [LabelText("上一次离开时的焦点世界DefId")]
        public string LastFocusWorldDefId;
        
        [LabelText("当前世界的DefId")]
        public string CurrentWorldDefId;
        
        [LabelText("激活的世界DefId列表")]
        [InfoBox("这个宇宙中当前存在的所有世界")]
        public List<string> ActiveWorldDefIds = new List<string>();

        public UniverseTemporaryData() : base()
        {
            LastFocusWorldDefId = string.Empty;
            CurrentWorldDefId = string.Empty;
        }
        
        public UniverseTemporaryData(string defId) : base(defId) 
        {
            LastFocusWorldDefId = string.Empty;
            CurrentWorldDefId = string.Empty;
        }
    }
}