using System;
using UnityEngine;

namespace Core.Game.Chunk.Tile
{
    /// <summary>
    /// 瓦片共分为如下层级:(由基础层依次去渲染,越上层,渲染层级越高)
    /// 基础层-->瓦片本身贴图
    /// 底部装饰层-->瓦片上面的装饰(可以是花,草,碎石等),根据装饰渲染优先级,依次去渲染(如果同级则按照先后顺序去渲染)
    /// 物体层-->瓦片上面的物体,根据渲染优先级,依次去渲染(如果同级则按照先后顺序去渲染)
    /// </summary>
    [Serializable]
    public class TileDefinition
    {
        public Vector2Int Position;
        
        public ETileType TileType;
        
        public bool IsWalkable;
        
        
    }
}