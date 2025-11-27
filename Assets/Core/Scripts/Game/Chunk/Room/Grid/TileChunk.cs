using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Core.Game.Chunk.Room.Grid.Renderer
{
    /// <summary>
    /// 地块渲染块
    /// 使用单一 Mesh + 多 SubMesh 方案
    /// 一个 TileChunk 只有一个 MeshRenderer，通过 SubMesh 支持多材质
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class TileChunk : MonoBehaviour
    {
        /// <summary>
        /// 块坐标
        /// </summary>
        public Vector2Int Coord { get; private set; }

        /// <summary>
        /// MeshFilter 组件
        /// </summary>
        private MeshFilter _meshFilter;
        
        /// <summary>
        /// MeshRenderer 组件
        /// </summary>
        private MeshRenderer _meshRenderer;
        
        /// <summary>
        /// 当前 Mesh
        /// </summary>
        private Mesh _mesh;

        /// <summary>
        /// 统计信息
        /// </summary>
        private int _vertexCount;
        private int _triangleCount;

        /// <summary>
        /// 初始化块
        /// </summary>
        public void Initialize(Vector2Int coord)
        {
            Coord = coord;
            
            // 获取或添加组件
            _meshFilter = GetComponent<MeshFilter>();
            if (_meshFilter == null)
            {
                _meshFilter = gameObject.AddComponent<MeshFilter>();
            }
            
            _meshRenderer = GetComponent<MeshRenderer>();
            if (_meshRenderer == null)
            {
                _meshRenderer = gameObject.AddComponent<MeshRenderer>();
            }
            
            // 创建 Mesh
            _mesh = new Mesh();
            _mesh.name = $"TileChunk_{coord.x}_{coord.y}";
            _meshFilter.sharedMesh = _mesh;
        }

        /// <summary>
        /// 更新网格
        /// 使用单一 Mesh + 多 SubMesh 方案
        /// </summary>
        public void UpdateMeshes(Dictionary<Material, TileMeshData> meshDataByMaterial, bool castShadows, bool receiveShadows)
        {
            if (_mesh == null)
            {
                _mesh = new Mesh();
                _mesh.name = $"TileChunk_{Coord.x}_{Coord.y}";
            }
            
            _mesh.Clear();
            _vertexCount = 0;
            _triangleCount = 0;

            if (meshDataByMaterial == null || meshDataByMaterial.Count == 0)
            {
                _meshFilter.sharedMesh = _mesh;
                _meshRenderer.sharedMaterials = new Material[0];
                return;
            }

            // === 收集所有数据 ===
            var allVertices = new List<Vector3>();
            var allNormals = new List<Vector3>();
            var allUVs = new List<Vector2>();
            var allColors = new List<Color>();
            var subMeshTriangles = new List<List<int>>(); // 每个 SubMesh 的三角形列表
            var materials = new List<Material>();
            
            int currentVertexOffset = 0;
            
            foreach (var kvp in meshDataByMaterial)
            {
                var material = kvp.Key;
                var meshData = kvp.Value;

                if (meshData.Vertices.Count == 0) continue;

                // 添加顶点数据
                allVertices.AddRange(meshData.Vertices);
                allNormals.AddRange(meshData.Normals);
                allUVs.AddRange(meshData.UVs);
                allColors.AddRange(meshData.Colors);

                // 调整三角形索引（加上偏移）
                var triangles = new List<int>();
                for (int i = 0; i < meshData.Triangles.Count; i++)
                {
                    triangles.Add(meshData.Triangles[i] + currentVertexOffset);
                }
                subMeshTriangles.Add(triangles);
                materials.Add(material);

                // 统计
                _vertexCount += meshData.Vertices.Count;
                _triangleCount += meshData.Triangles.Count / 3;
                
                currentVertexOffset += meshData.Vertices.Count;
            }

            if (allVertices.Count == 0)
            {
                _meshFilter.sharedMesh = _mesh;
                _meshRenderer.sharedMaterials = new Material[0];
                return;
            }

            // === 设置 Mesh 数据 ===
            _mesh.SetVertices(allVertices);
            _mesh.SetNormals(allNormals);
            _mesh.SetUVs(0, allUVs);
            _mesh.SetColors(allColors);

            // 设置 SubMesh 数量
            _mesh.subMeshCount = subMeshTriangles.Count;

            // 设置每个 SubMesh 的三角形
            for (int i = 0; i < subMeshTriangles.Count; i++)
            {
                _mesh.SetTriangles(subMeshTriangles[i], i);
            }

            // 优化 Mesh
            _mesh.RecalculateBounds();
            _mesh.Optimize();

            // 设置 MeshFilter
            _meshFilter.sharedMesh = _mesh;

            // 设置材质
            _meshRenderer.sharedMaterials = materials.ToArray();
            
            // 设置阴影
            _meshRenderer.shadowCastingMode = castShadows 
                ? ShadowCastingMode.On 
                : ShadowCastingMode.Off;
            _meshRenderer.receiveShadows = receiveShadows;
        }

        /// <summary>
        /// 清除 Mesh
        /// </summary>
        public void ClearMesh()
        {
            if (_mesh != null)
            {
                _mesh.Clear();
            }
            
            if (_meshRenderer != null)
            {
                _meshRenderer.sharedMaterials = new Material[0];
            }
            
            _vertexCount = 0;
            _triangleCount = 0;
        }

        /// <summary>
        /// 获取统计信息
        /// </summary>
        public (int vertices, int triangles) GetStatistics()
        {
            return (_vertexCount, _triangleCount);
        }

        private void OnDestroy()
        {
            // 销毁 Mesh 资源
            if (_mesh != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(_mesh);
                }
                else
                {
                    DestroyImmediate(_mesh);
                }
            }
        }
    }
}