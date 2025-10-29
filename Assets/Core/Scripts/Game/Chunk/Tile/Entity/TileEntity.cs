using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Tile.Entity
{
    public enum ETileEntityRotationType
    {
        Up,
        Down,
        Left,
        Right
    }
    
    [Serializable]
    public class TileEntity
    {
        [LabelText("实体配置ID")]
        [InfoBox("引用EntityDtoDef的DefId")]
        public string EntityDefId;
        
        [LabelText("实体旋转方向")]
        public ETileEntityRotationType EntityRotationType;
        
        [LabelText("位置")]
        public Vector2Int Position;
    }
}