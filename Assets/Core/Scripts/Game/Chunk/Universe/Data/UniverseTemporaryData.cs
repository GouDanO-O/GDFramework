using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Universe.Data
{
    [Serializable]
    public class UniverseTemporaryData : ChunkContainerTemporaryData
    {
        [LabelText("上一次离开时的焦点世界ID")]
        public string LastFocusWorldInstanceId;
        
        [LabelText("当前世界的实例ID")]
        public string CurrentWorldInstanceId;
        
        [LabelText("所有世界的实例ID")]
        public List<string> WorldInstanceIds = new List<string>();
        
        public UniverseTemporaryData() : base() { }
        public UniverseTemporaryData(string defId) : base(defId) { }
    }
}