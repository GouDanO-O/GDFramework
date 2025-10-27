using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Tile
{
    [Serializable]
    public class TileDtoTemporary : ChunkDtoTemporary
    {
        [LabelText("当前瓦片上放置的物体->存档")]
        public List<string> curTilePlacedNodeIdList = new List<string>();
        
        [LabelText("当前瓦片上放置的装饰物->存档")]
        public List<string> curTilePlacedDecorationIdList = new List<string>();
    }
}