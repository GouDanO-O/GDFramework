using System;
using Core.Game.Chunk.Data;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Tile.Decoration
{
    [Serializable]
    public class TileDecoration : ChunkDtoDef
    {
        [LabelText("装饰图贴图ID")]
        public string decorationId;
        
        [LabelText("渲染优先级")]
        public int renderPriority;

        protected override string GetTypePrefix()
        {
            return "TileDecoration";
        }
    }
}