using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Tile
{
    [Serializable]
    public class TileDtoDef
    {
        [LabelText("瓦片类型")]
        public ETileType tileType;
        
        [LabelText("瓦片贴图ID")]
        public string tileSpriteId;
        
        [LabelText("瓦片上放置的底部装饰层")]
        public List<string> decorationIdList = new List<string>();
        
        [LabelText("瓦片上放置的物体(可以是人,也可以是其他东西)")]
        public List<string> substanceIdList = new List<string>();
    }
}