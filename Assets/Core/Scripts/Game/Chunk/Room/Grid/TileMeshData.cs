using System.Collections.Generic;
using UnityEngine;

namespace Core.Game.Chunk.Room.Grid
{

    /// <summary>
    /// 地块网格数据（用于合并）
    /// </summary>
    public class TileMeshData
    {
        public List<Vector3> Vertices = new List<Vector3>();
        public List<int> Triangles = new List<int>();
        public List<Vector2> UVs = new List<Vector2>();
        public List<Color> Colors = new List<Color>();
        public List<Vector3> Normals = new List<Vector3>();
        
        public void Clear()
        {
            Vertices.Clear();
            Triangles.Clear();
            UVs.Clear();
            Colors.Clear();
            Normals.Clear();
        }
    }
}