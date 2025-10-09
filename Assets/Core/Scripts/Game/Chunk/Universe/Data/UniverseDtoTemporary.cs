using System;
using Core.Game.Chunk.Data;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Universe.Data
{
    [Serializable]
    public class UniverseDtoTemporary : ChunkDtoTemporary
    {
        [LabelText("上一次离开时的焦点世界ID")]
        public string lastFocusWorldId;
    }
}