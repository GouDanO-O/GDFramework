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
        [LabelText("上一次离开时的焦点世界ID")]
        public string LastFocusWorldDefId;
        
        [LabelText("当前世界的实例ID")]
        public string CurrentWorldDefId;
        
        [LabelText("所有世界的实例ID")]
        public List<string> WorldDefIds = new List<string>();

        public UniverseTemporaryData() : base()
        {
            LastFocusWorldDefId = string.Empty;
            CurrentWorldDefId = string.Empty;
        }
        
        public UniverseTemporaryData(string defId) : base(defId) { }
    }
}