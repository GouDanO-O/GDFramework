using System.Collections.Generic;
using Core.Game.Chunk.Room.Grid.Editor;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Room.Grid
{
/// <summary>
    /// 地块渲染器
    /// 使用合并 Mesh 方案实现高性能渲染
    /// </summary>
    public class TileRenderer : MonoBehaviour
    {
        #region 配置

        [Title("基础设置")]
        
        [LabelText("编辑器引用")]
        [SerializeField]
        private RoomGridEditor _editor;

        [LabelText("地块厚度")]
        [SerializeField]
        private float _tileHeight = 0.1f;

        [LabelText("地块间隙")]
        [SerializeField]
        [Range(0f, 0.1f)]
        private float _tileGap = 0.02f;

        [LabelText("高度单位")]
        [SerializeField]
        private float _heightUnit = 0.5f;

        [Title("材质配置")]
        
        [LabelText("地块材质列表")]
        [SerializeField]
        private List<TileMaterialConfig> _tileMaterials = new List<TileMaterialConfig>();

        [LabelText("默认材质")]
        [SerializeField]
        private Material _defaultMaterial;

        [Title("渲染设置")]
        
        [LabelText("启用阴影")]
        [SerializeField]
        private bool _castShadows = true;

        [LabelText("接收阴影")]
        [SerializeField]
        private bool _receiveShadows = true;

        [LabelText("渲染层")]
        [SerializeField]
        private int _renderLayer = 0;

        [Title("优化设置")]
        
        [LabelText("分块大小")]
        [SerializeField]
        [Range(8, 32)]
        private int _chunkSize = 16;

        [LabelText("延迟更新时间")]
        [SerializeField]
        private float _updateDelay = 0.05f;

        #endregion

        #region 运行时数据

        [Title("调试信息")]
        
        [LabelText("当前楼层")]
        [ReadOnly]
        [ShowInInspector]
        private int _currentFloor;

        [LabelText("渲染块数量")]
        [ReadOnly]
        [ShowInInspector]
        private int _chunkCount;

        [LabelText("总顶点数")]
        [ReadOnly]
        [ShowInInspector]
        private int _totalVertices;

        [LabelText("总三角形数")]
        [ReadOnly]
        [ShowInInspector]
        private int _totalTriangles;

        // 材质查找字典
        private Dictionary<TileType, TileMaterialConfig> _materialDict;
        
        // 渲染块
        private Dictionary<Vector2Int, TileChunk> _chunks;
        
        // 待更新的块
        private HashSet<Vector2Int> _dirtyChunks;
        
        // 更新计时器
        private float _updateTimer;
        
        // 是否需要全量重建
        private bool _needFullRebuild;

        #endregion

        #region 生命周期

        private void Awake()
        {
            _materialDict = new Dictionary<TileType, TileMaterialConfig>();
            _chunks = new Dictionary<Vector2Int, TileChunk>();
            _dirtyChunks = new HashSet<Vector2Int>();
            
            InitializeMaterials();
        }

        private void Start()
        {
            if (_editor == null)
            {
                _editor = GetComponent<RoomGridEditor>();
            }

            if (_editor != null)
            {
                SubscribeEvents();
                
                // 等待编辑器初始化后重建
                if (_editor.IsInitialized)
                {
                    RebuildAll();
                }
            }
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
            ClearAllChunks();
        }

        private void Update()
        {
            // 延迟批量更新
            if (_dirtyChunks.Count > 0)
            {
                _updateTimer += Time.deltaTime;
                if (_updateTimer >= _updateDelay)
                {
                    ProcessDirtyChunks();
                    _updateTimer = 0f;
                }
            }

            // 全量重建
            if (_needFullRebuild)
            {
                _needFullRebuild = false;
                RebuildAll();
            }
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化材质字典
        /// </summary>
        private void InitializeMaterials()
        {
            _materialDict.Clear();
            
            foreach (var config in _tileMaterials)
            {
                if (!_materialDict.ContainsKey(config.TileType))
                {
                    _materialDict[config.TileType] = config;
                }
            }

            // 确保有默认材质
            if (_defaultMaterial == null)
            {
                _defaultMaterial = new Material(Shader.Find("Standard"));
                _defaultMaterial.color = Color.gray;
            }

            Debug.Log($"[TileRenderer] 初始化材质: {_materialDict.Count} 种");
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        private void SubscribeEvents()
        {
            if (_editor == null) return;

            _editor.OnEditorInitialized += OnEditorInitialized;
            _editor.OnTileModified += OnTileModified;
            
            if (_editor.Grid != null)
            {
                _editor.Grid.OnFloorChanged += OnFloorChanged;
            }
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        private void UnsubscribeEvents()
        {
            if (_editor == null) return;

            _editor.OnEditorInitialized -= OnEditorInitialized;
            _editor.OnTileModified -= OnTileModified;
            
            if (_editor.Grid != null)
            {
                _editor.Grid.OnFloorChanged -= OnFloorChanged;
            }
        }

        #endregion

        #region 事件处理

        private void OnEditorInitialized()
        {
            Debug.Log("[TileRenderer] 编辑器初始化完成，重建渲染");
            
            // 重新订阅楼层变化事件
            if (_editor.Grid != null)
            {
                _editor.Grid.OnFloorChanged += OnFloorChanged;
            }
            
            _needFullRebuild = true;
        }

        private void OnTileModified(TilePosition pos, TileData tile)
        {
            // 标记对应的块为脏
            var chunkCoord = GetChunkCoord(pos);
            _dirtyChunks.Add(chunkCoord);
        }

        private void OnFloorChanged(int oldFloor, int newFloor)
        {
            Debug.Log($"[TileRenderer] 楼层切换: {oldFloor} -> {newFloor}");
            _currentFloor = newFloor;
            _needFullRebuild = true;
        }

        #endregion

        #region 渲染块管理

        /// <summary>
        /// 获取地块所属的块坐标
        /// </summary>
        private Vector2Int GetChunkCoord(TilePosition pos)
        {
            return new Vector2Int(
                Mathf.FloorToInt((float)pos.X / _chunkSize),
                Mathf.FloorToInt((float)pos.Z / _chunkSize)
            );
        }

        /// <summary>
        /// 处理脏块
        /// </summary>
        private void ProcessDirtyChunks()
        {
            foreach (var chunkCoord in _dirtyChunks)
            {
                RebuildChunk(chunkCoord);
            }
            _dirtyChunks.Clear();
            
            UpdateStatistics();
        }

        /// <summary>
        /// 重建所有渲染块
        /// </summary>
        [Button("重建所有", ButtonSizes.Large)]
        public void RebuildAll()
        {
            if (_editor?.Grid == null)
            {
                Debug.LogWarning("[TileRenderer] 编辑器或网格未初始化");
                return;
            }

            ClearAllChunks();

            var config = _editor.Grid.Config;
            _currentFloor = _editor.State?.CurrentFloor ?? 0;

            // 计算需要的块数量
            int chunksX = Mathf.CeilToInt((float)config.Width / _chunkSize);
            int chunksZ = Mathf.CeilToInt((float)config.Depth / _chunkSize);

            Debug.Log($"[TileRenderer] 开始重建: {chunksX}x{chunksZ} 块, 楼层: {_currentFloor}");

            for (int cx = 0; cx < chunksX; cx++)
            {
                for (int cz = 0; cz < chunksZ; cz++)
                {
                    var chunkCoord = new Vector2Int(cx, cz);
                    RebuildChunk(chunkCoord);
                }
            }

            UpdateStatistics();
            Debug.Log($"[TileRenderer] 重建完成: {_chunkCount} 块, {_totalVertices} 顶点");
        }

        /// <summary>
        /// 重建单个块
        /// </summary>
        private void RebuildChunk(Vector2Int chunkCoord)
        {
            if (_editor?.Grid == null) return;

            var config = _editor.Grid.Config;
            var floorData = _editor.Grid.GetFloorData(_currentFloor);
            if (floorData == null) return;

            // 获取或创建块
            if (!_chunks.TryGetValue(chunkCoord, out var chunk))
            {
                chunk = CreateChunk(chunkCoord);
                _chunks[chunkCoord] = chunk;
            }

            // 按材质分组的网格数据
            var meshDataByMaterial = new Dictionary<Material, TileMeshData>();

            // 计算块的地块范围
            int startX = chunkCoord.x * _chunkSize;
            int startZ = chunkCoord.y * _chunkSize;
            int endX = Mathf.Min(startX + _chunkSize, config.Width);
            int endZ = Mathf.Min(startZ + _chunkSize, config.Depth);

            // 遍历块内的所有地块
            for (int x = startX; x < endX; x++)
            {
                for (int z = startZ; z < endZ; z++)
                {
                    var pos = new TilePosition(x, z);
                    var tile = _editor.Grid.GetTile(pos, _currentFloor);

                    if (tile == null || tile.Type == TileType.None) continue;

                    // 获取材质
                    var material = GetMaterialForTile(tile.Type);
                    var materialConfig = GetMaterialConfig(tile.Type);

                    // 获取或创建该材质的网格数据
                    if (!meshDataByMaterial.TryGetValue(material, out var meshData))
                    {
                        meshData = new TileMeshData();
                        meshDataByMaterial[material] = meshData;
                    }

                    // 添加地块几何体
                    AddTileGeometry(meshData, tile, config, materialConfig);
                }
            }

            // 更新块的子网格
            chunk.UpdateMeshes(meshDataByMaterial, _castShadows, _receiveShadows);
        }

        /// <summary>
        /// 创建渲染块
        /// </summary>
        private TileChunk CreateChunk(Vector2Int coord)
        {
            var chunkGO = new GameObject($"TileChunk_{coord.x}_{coord.y}");
            chunkGO.transform.SetParent(transform);
            chunkGO.transform.localPosition = Vector3.zero;
            chunkGO.layer = _renderLayer;

            var chunk = chunkGO.AddComponent<TileChunk>();
            chunk.Initialize(coord);

            return chunk;
        }

        /// <summary>
        /// 清空所有块
        /// </summary>
        [Button("清空渲染")]
        public void ClearAllChunks()
        {
            foreach (var chunk in _chunks.Values)
            {
                if (chunk != null)
                {
                    Destroy(chunk.gameObject);
                }
            }
            _chunks.Clear();
            _dirtyChunks.Clear();
            
            UpdateStatistics();
        }

        #endregion

        #region 几何体生成

        /// <summary>
        /// 添加地块几何体
        /// </summary>
        private void AddTileGeometry(TileMeshData meshData, TileData tile, RoomGridConfig config, TileMaterialConfig matConfig)
        {
            // 计算世界位置
            Vector3 worldPos = config.TileToWorld(tile.Position);
            float heightOffset = tile.HeightLevel * _heightUnit;
            
            // 地块尺寸（考虑间隙）
            float halfSize = (config.TileSize - _tileGap) * 0.5f;
            float halfHeight = _tileHeight * 0.5f;

            // 顶点基础索引
            int baseIndex = meshData.Vertices.Count;

            // UV 缩放和偏移
            Vector2 uvScale = matConfig?.UVScale ?? Vector2.one;
            Vector2 uvOffset = matConfig?.UVOffset ?? Vector2.zero;
            Color vertexColor = matConfig?.VertexColor ?? Color.white;

            // === 顶面（主要可见面）===
            // 4个顶点
            meshData.Vertices.Add(new Vector3(worldPos.x - halfSize, heightOffset + halfHeight, worldPos.z - halfSize));
            meshData.Vertices.Add(new Vector3(worldPos.x + halfSize, heightOffset + halfHeight, worldPos.z - halfSize));
            meshData.Vertices.Add(new Vector3(worldPos.x + halfSize, heightOffset + halfHeight, worldPos.z + halfSize));
            meshData.Vertices.Add(new Vector3(worldPos.x - halfSize, heightOffset + halfHeight, worldPos.z + halfSize));

            // 法线（朝上）
            for (int i = 0; i < 4; i++)
            {
                meshData.Normals.Add(Vector3.up);
                meshData.Colors.Add(vertexColor);
            }

            // UV
            meshData.UVs.Add(new Vector2(0, 0) * uvScale + uvOffset);
            meshData.UVs.Add(new Vector2(1, 0) * uvScale + uvOffset);
            meshData.UVs.Add(new Vector2(1, 1) * uvScale + uvOffset);
            meshData.UVs.Add(new Vector2(0, 1) * uvScale + uvOffset);

            // 三角形（顶面）
            meshData.Triangles.Add(baseIndex + 0);
            meshData.Triangles.Add(baseIndex + 2);
            meshData.Triangles.Add(baseIndex + 1);
            meshData.Triangles.Add(baseIndex + 0);
            meshData.Triangles.Add(baseIndex + 3);
            meshData.Triangles.Add(baseIndex + 2);

            // === 侧面（可选，增加立体感）===
            if (_tileHeight > 0.01f)
            {
                AddTileSides(meshData, worldPos, halfSize, halfHeight, heightOffset, vertexColor);
            }
        }

        /// <summary>
        /// 添加地块侧面
        /// </summary>
        private void AddTileSides(TileMeshData meshData, Vector3 center, float halfSize, float halfHeight, float heightOffset, Color color)
        {
            // 侧面颜色稍暗
            Color sideColor = color * 0.8f;
            sideColor.a = color.a;

            float top = heightOffset + halfHeight;
            float bottom = heightOffset - halfHeight;

            // 前面 (+Z)
            AddQuad(meshData,
                new Vector3(center.x - halfSize, bottom, center.z + halfSize),
                new Vector3(center.x + halfSize, bottom, center.z + halfSize),
                new Vector3(center.x + halfSize, top, center.z + halfSize),
                new Vector3(center.x - halfSize, top, center.z + halfSize),
                Vector3.forward, sideColor);

            // 后面 (-Z)
            AddQuad(meshData,
                new Vector3(center.x + halfSize, bottom, center.z - halfSize),
                new Vector3(center.x - halfSize, bottom, center.z - halfSize),
                new Vector3(center.x - halfSize, top, center.z - halfSize),
                new Vector3(center.x + halfSize, top, center.z - halfSize),
                Vector3.back, sideColor);

            // 右面 (+X)
            AddQuad(meshData,
                new Vector3(center.x + halfSize, bottom, center.z + halfSize),
                new Vector3(center.x + halfSize, bottom, center.z - halfSize),
                new Vector3(center.x + halfSize, top, center.z - halfSize),
                new Vector3(center.x + halfSize, top, center.z + halfSize),
                Vector3.right, sideColor);

            // 左面 (-X)
            AddQuad(meshData,
                new Vector3(center.x - halfSize, bottom, center.z - halfSize),
                new Vector3(center.x - halfSize, bottom, center.z + halfSize),
                new Vector3(center.x - halfSize, top, center.z + halfSize),
                new Vector3(center.x - halfSize, top, center.z - halfSize),
                Vector3.left, sideColor);
        }

        /// <summary>
        /// 添加四边形
        /// </summary>
        private void AddQuad(TileMeshData meshData, Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Vector3 normal, Color color)
        {
            int baseIndex = meshData.Vertices.Count;

            meshData.Vertices.Add(v0);
            meshData.Vertices.Add(v1);
            meshData.Vertices.Add(v2);
            meshData.Vertices.Add(v3);

            for (int i = 0; i < 4; i++)
            {
                meshData.Normals.Add(normal);
                meshData.Colors.Add(color);
            }

            meshData.UVs.Add(new Vector2(0, 0));
            meshData.UVs.Add(new Vector2(1, 0));
            meshData.UVs.Add(new Vector2(1, 1));
            meshData.UVs.Add(new Vector2(0, 1));

            meshData.Triangles.Add(baseIndex + 0);
            meshData.Triangles.Add(baseIndex + 2);
            meshData.Triangles.Add(baseIndex + 1);
            meshData.Triangles.Add(baseIndex + 0);
            meshData.Triangles.Add(baseIndex + 3);
            meshData.Triangles.Add(baseIndex + 2);
        }

        #endregion

        #region 材质管理

        /// <summary>
        /// 获取地块材质
        /// </summary>
        private Material GetMaterialForTile(TileType type)
        {
            if (_materialDict.TryGetValue(type, out var config) && config.Material != null)
            {
                return config.Material;
            }
            return _defaultMaterial;
        }

        /// <summary>
        /// 获取材质配置
        /// </summary>
        private TileMaterialConfig GetMaterialConfig(TileType type)
        {
            _materialDict.TryGetValue(type, out var config);
            return config;
        }

        /// <summary>
        /// 设置地块材质
        /// </summary>
        public void SetTileMaterial(TileType type, Material material)
        {
            if (_materialDict.TryGetValue(type, out var config))
            {
                config.Material = material;
            }
            else
            {
                _materialDict[type] = new TileMaterialConfig
                {
                    TileType = type,
                    Material = material
                };
            }

            _needFullRebuild = true;
        }

        #endregion

        #region 统计

        /// <summary>
        /// 更新统计信息
        /// </summary>
        private void UpdateStatistics()
        {
            _chunkCount = _chunks.Count;
            _totalVertices = 0;
            _totalTriangles = 0;

            foreach (var chunk in _chunks.Values)
            {
                var stats = chunk.GetStatistics();
                _totalVertices += stats.vertices;
                _totalTriangles += stats.triangles;
            }
        }

        #endregion

        #region 编辑器工具

        [Title("快捷操作")]

        [Button("自动生成默认材质")]
        private void GenerateDefaultMaterials()
        {
            _tileMaterials.Clear();
            
            var shader = Shader.Find("Standard");
            
            // 定义每种类型的颜色
            var typeColors = new Dictionary<TileType, Color>
            {
                { TileType.Grass, new Color(0.3f, 0.7f, 0.3f) },
                { TileType.Dirt, new Color(0.5f, 0.35f, 0.2f) },
                { TileType.Stone, new Color(0.5f, 0.5f, 0.5f) },
                { TileType.Wood, new Color(0.6f, 0.4f, 0.2f) },
                { TileType.Sand, new Color(0.9f, 0.85f, 0.6f) },
                { TileType.Water, new Color(0.2f, 0.5f, 0.9f) },
                { TileType.Carpet, new Color(0.7f, 0.2f, 0.2f) },
                { TileType.Tile, new Color(0.9f, 0.9f, 0.9f) },
                { TileType.Metal, new Color(0.7f, 0.7f, 0.8f) },
                { TileType.Glass, new Color(0.5f, 0.8f, 1f) },
                { TileType.Snow, Color.white },
                { TileType.Lava, new Color(1f, 0.3f, 0f) },
                { TileType.Ice, new Color(0.7f, 0.9f, 1f) },
            };

            foreach (var kvp in typeColors)
            {
                var mat = new Material(shader);
                mat.color = kvp.Value;
                mat.name = $"Tile_{kvp.Key}";

                _tileMaterials.Add(new TileMaterialConfig
                {
                    TileType = kvp.Key,
                    Material = mat,
                    VertexColor = kvp.Value
                });
            }

            InitializeMaterials();
            Debug.Log($"[TileRenderer] 生成了 {_tileMaterials.Count} 个默认材质");
        }

        #endregion
    }
}