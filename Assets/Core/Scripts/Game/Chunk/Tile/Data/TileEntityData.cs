using Core.Game.Chunk.Substance.Data;
using UnityEngine;

namespace Core.Game.Chunk.Tile
{
    public class TileEntityData
    {
        /// <summary>
        /// 实体ID
        /// </summary>
        public string EntityDtoDefId;

        /// <summary>
        /// 起始瓦片坐标
        /// </summary>
        public Vector2Int TileIndex;

        /// <summary>
        /// 放置方向
        /// </summary>
        public EEntityRotationType RotationType;
        
        
    }
}