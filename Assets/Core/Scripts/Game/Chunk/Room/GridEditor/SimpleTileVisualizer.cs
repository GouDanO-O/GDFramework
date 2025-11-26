using System.Collections.Generic;
using Core.Game.Chunk.Room.Grid;
using Core.Game.Chunk.Room.Grid.Editor;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Room.Test
{
    /// <summary>
    /// 简单的地块可视化
    /// 用于测试阶段，后续会用合并Mesh替代
    /// </summary>
    public class SimpleTileVisualizer : MonoBehaviour
    {
        [Title("引用")]
        
        [LabelText("编辑器")]
        [SerializeField]
        private RoomGridEditor _editor;

        [LabelText("地块预制体")]
        [SerializeField]
        private GameObject _tilePrefab;

        [LabelText("地块材质")]
        [SerializeField]
        private Material[] _tileMaterials;

        [Title("设置")]
        
        [LabelText("自动更新")]
        [SerializeField]
        private bool _autoUpdate = true;

        [LabelText("更新间隔")]
        [SerializeField]
        private float _updateInterval = 0.1f;

        [Title("调试")]
        
        [LabelText("显示的地块数")]
        [ReadOnly]
        [ShowInInspector]
        private int _visibleTileCount;

        private Dictionary<string, GameObject> _tileObjects = new Dictionary<string, GameObject>();
        private float _lastUpdateTime;
        private bool _isDirty;

        private void Start()
        {
            if (_editor == null)
            {
                _editor = GetComponent<RoomGridEditor>();
            }

            if (_editor != null)
            {
                _editor.OnTileModified += OnTileModified;
                _editor.OnEditorInitialized += OnEditorInitialized;
            }

            // 创建默认预制体
            if (_tilePrefab == null)
            {
                CreateDefaultPrefab();
            }

            // 创建默认材质
            if (_tileMaterials == null || _tileMaterials.Length == 0)
            {
                CreateDefaultMaterials();
            }
        }

        private void Update()
        {
            if (!_autoUpdate || _editor == null || !_editor.IsInitialized) return;

            if (_isDirty && Time.time - _lastUpdateTime > _updateInterval)
            {
                RefreshAllTiles();
                _isDirty = false;
                _lastUpdateTime = Time.time;
            }
        }

        private void OnDestroy()
        {
            if (_editor != null)
            {
                _editor.OnTileModified -= OnTileModified;
                _editor.OnEditorInitialized -= OnEditorInitialized;
            }

            ClearAllTiles();
        }

        private void OnEditorInitialized()
        {
            _isDirty = true;
        }

        private void OnTileModified(TilePosition pos, TileData tile)
        {
            _isDirty = true;
        }

        /// <summary>
        /// 创建默认预制体
        /// </summary>
        private void CreateDefaultPrefab()
        {
            _tilePrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _tilePrefab.name = "TilePrefab";
            _tilePrefab.transform.localScale = new Vector3(0.95f, 0.1f, 0.95f);
            _tilePrefab.SetActive(false);
            
            // 移除碰撞体（测试时不需要）
            var collider = _tilePrefab.GetComponent<Collider>();
            if (collider != null)
            {
                Destroy(collider);
            }
        }

        /// <summary>
        /// 创建默认材质
        /// </summary>
        private void CreateDefaultMaterials()
        {
            _tileMaterials = new Material[13];
            
            var shader = Shader.Find("Standard");
            
            // None - 透明
            _tileMaterials[0] = new Material(shader) { color = new Color(0, 0, 0, 0) };
            // Grass - 绿色
            _tileMaterials[1] = new Material(shader) { color = new Color(0.3f, 0.7f, 0.3f) };
            // Dirt - 棕色
            _tileMaterials[2] = new Material(shader) { color = new Color(0.5f, 0.35f, 0.2f) };
            // Stone - 灰色
            _tileMaterials[3] = new Material(shader) { color = new Color(0.5f, 0.5f, 0.5f) };
            // Wood - 木色
            _tileMaterials[4] = new Material(shader) { color = new Color(0.6f, 0.4f, 0.2f) };
            // Sand - 沙色
            _tileMaterials[5] = new Material(shader) { color = new Color(0.9f, 0.85f, 0.6f) };
            // Water - 蓝色
            _tileMaterials[6] = new Material(shader) { color = new Color(0.2f, 0.5f, 0.9f, 0.7f) };
            // Carpet - 红色
            _tileMaterials[7] = new Material(shader) { color = new Color(0.7f, 0.2f, 0.2f) };
            // Tile - 白色
            _tileMaterials[8] = new Material(shader) { color = new Color(0.9f, 0.9f, 0.9f) };
            // Metal - 银色
            _tileMaterials[9] = new Material(shader) { color = new Color(0.7f, 0.7f, 0.8f) };
            // Glass - 透明蓝
            _tileMaterials[10] = new Material(shader) { color = new Color(0.5f, 0.8f, 1f, 0.5f) };
            // Snow - 白色
            _tileMaterials[11] = new Material(shader) { color = Color.white };
            // Lava - 橙红
            _tileMaterials[12] = new Material(shader) { color = new Color(1f, 0.3f, 0f) };
        }

        /// <summary>
        /// 刷新所有地块显示
        /// </summary>
        [Button("刷新显示", ButtonSizes.Large)]
        public void RefreshAllTiles()
        {
            if (_editor?.Grid == null) return;

            var config = _editor.Grid.Config;
            var floor = _editor.State?.CurrentFloor ?? 0;
            var floorData = _editor.Grid.Floors[floor];

            // 标记所有现有的为待删除
            var toRemove = new HashSet<string>(_tileObjects.Keys);

            // 更新或创建地块
            foreach (var kvp in floorData.Tiles)
            {
                var tile = kvp.Value;
                var key = kvp.Key;

                if (tile.Type == TileType.None) continue;

                toRemove.Remove(key);

                if (_tileObjects.TryGetValue(key, out var existingObj))
                {
                    // 更新现有的
                    UpdateTileObject(existingObj, tile, config);
                }
                else
                {
                    // 创建新的
                    CreateTileObject(tile, config);
                }
            }

            // 删除不再需要的
            foreach (var key in toRemove)
            {
                if (_tileObjects.TryGetValue(key, out var obj))
                {
                    Destroy(obj);
                    _tileObjects.Remove(key);
                }
            }

            _visibleTileCount = _tileObjects.Count;
        }

        /// <summary>
        /// 创建地块对象
        /// </summary>
        private void CreateTileObject(TileData tile, RoomGridConfig config)
        {
            if (_tilePrefab == null) return;

            var obj = Instantiate(_tilePrefab, transform);
            obj.SetActive(true);
            obj.name = $"Tile_{tile.Position.X}_{tile.Position.Z}";

            UpdateTileObject(obj, tile, config);

            _tileObjects[tile.Position.ToKey()] = obj;
        }

        /// <summary>
        /// 更新地块对象
        /// </summary>
        private void UpdateTileObject(GameObject obj, TileData tile, RoomGridConfig config)
        {
            // 位置
            Vector3 worldPos = config.TileToWorld(tile.Position);
            worldPos.y = tile.HeightLevel * 0.5f; // 高度
            obj.transform.position = worldPos;

            // 材质
            var renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                int matIndex = (int)tile.Type;
                if (matIndex >= 0 && matIndex < _tileMaterials.Length)
                {
                    renderer.material = _tileMaterials[matIndex];
                }
            }
        }

        /// <summary>
        /// 清空所有地块对象
        /// </summary>
        [Button("清空显示")]
        public void ClearAllTiles()
        {
            foreach (var obj in _tileObjects.Values)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
            _tileObjects.Clear();
            _visibleTileCount = 0;
        }
    }
}