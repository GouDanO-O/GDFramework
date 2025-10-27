using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Tile
{
    [Serializable]
    public class TileDtoDef
    {
        [LabelText("当前瓦片类型")]
        public ETileType tileType;
        
        [LabelText("当前瓦片贴图ID")]
        public string tileSpriteId;
        
        [LabelText("当前瓦片上放置的底部装饰层")]
        public List<string> decorationIdList = new List<string>();
        
        [LabelText("当前瓦片上放置的行为结点(可以是人,也可以是其他东西)")]
        public List<string> nodeIdList = new List<string>();
    }
}