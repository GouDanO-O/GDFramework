using System.Collections.Generic;
using UnityEngine;

namespace Core.Game.Chunk.Room.Grid
{
    /// <summary>
    /// 地块渲染块
    /// 管理一个区域内的地块网格渲染
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
        /// 子网格对象列表
        /// </summary>
        private List<GameObject> _subMeshObjects = new List<GameObject>();

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
        }

        /// <summary>
        /// 更新网格
        /// </summary>
        public void UpdateMeshes(Dictionary<Material, TileMeshData> meshDataByMaterial, bool castShadows, bool receiveShadows)
        {
            // 清除旧的子网格
            ClearSubMeshes();

            _vertexCount = 0;
            _triangleCount = 0;

            // 为每种材质创建子网格
            int subMeshIndex = 0;
            foreach (var kvp in meshDataByMaterial)
            {
                var material = kvp.Key;
                var meshData = kvp.Value;

                if (meshData.Vertices.Count == 0) continue;

                // 创建子网格对象
                var subMeshGO = new GameObject($"SubMesh_{subMeshIndex}");
                subMeshGO.transform.SetParent(transform);
                subMeshGO.transform.localPosition = Vector3.zero;
                subMeshGO.transform.localRotation = Quaternion.identity;
                subMeshGO.transform.localScale = Vector3.one;
                subMeshGO.layer = gameObject.layer;

                // 添加组件
                var meshFilter = subMeshGO.AddComponent<MeshFilter>();
                var meshRenderer = subMeshGO.AddComponent<MeshRenderer>();

                // 创建网格
                var mesh = new Mesh();
                mesh.name = $"TileChunk_{Coord.x}_{Coord.y}_Sub{subMeshIndex}";

                // 设置网格数据
                mesh.SetVertices(meshData.Vertices);
                mesh.SetTriangles(meshData.Triangles, 0);
                mesh.SetUVs(0, meshData.UVs);
                mesh.SetColors(meshData.Colors);
                mesh.SetNormals(meshData.Normals);

                // 优化网格
                mesh.RecalculateBounds();
                mesh.Optimize();

                meshFilter.mesh = mesh;

                // 设置材质
                meshRenderer.material = material;
                meshRenderer.shadowCastingMode = castShadows 
                    ? UnityEngine.Rendering.ShadowCastingMode.On 
                    : UnityEngine.Rendering.ShadowCastingMode.Off;
                meshRenderer.receiveShadows = receiveShadows;

                _subMeshObjects.Add(subMeshGO);

                // 统计
                _vertexCount += meshData.Vertices.Count;
                _triangleCount += meshData.Triangles.Count / 3;

                subMeshIndex++;
            }
        }

        /// <summary>
        /// 清除子网格
        /// </summary>
        private void ClearSubMeshes()
        {
            foreach (var subMeshGO in _subMeshObjects)
            {
                if (subMeshGO != null)
                {
                    // 销毁网格资源
                    var meshFilter = subMeshGO.GetComponent<MeshFilter>();
                    if (meshFilter != null && meshFilter.sharedMesh != null)
                    {
                        Destroy(meshFilter.sharedMesh);
                    }
                    
                    Destroy(subMeshGO);
                }
            }
            _subMeshObjects.Clear();
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
            ClearSubMeshes();
        }
    }
}