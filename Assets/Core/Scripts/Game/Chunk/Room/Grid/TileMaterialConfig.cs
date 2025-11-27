using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Room.Grid
{
    /// <summary>
    /// 地块材质配置
    /// </summary>
    [Serializable]
    public class TileMaterialConfig
    {
        [LabelText("地块类型")]
        public TileType TileType;
        
        [LabelText("材质")]
        public Material Material;
        
        [LabelText("UV缩放")]
        public Vector2 UVScale = Vector2.one;
        
        [LabelText("UV偏移")]
        public Vector2 UVOffset = Vector2.zero;
        
        [LabelText("顶点颜色")]
        public Color VertexColor = Color.white;
    }
}