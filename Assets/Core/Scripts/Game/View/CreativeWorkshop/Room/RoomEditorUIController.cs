using System.Collections.Generic;
using Core.Game.Chunk.Room;
using Core.Game.Chunk.Room.Data;
using GDFrameworkCore;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TileData = Core.Game.Chunk.Room.TileData;

namespace Core.Game.View.Room
{
    /// <summary>
    /// 房间编辑器UI控制器
    /// </summary>
    public class RoomEditorUIController : MonoBehaviour, ICanGetSystem
    {
        [Header("Tilemap引用")]
        [SerializeField] private Tilemap floorTilemap;
        [SerializeField] private Tilemap wallTilemap;
        [SerializeField] private Tilemap previewTilemap;
        
        [Header("瓦片资源")]
        [SerializeField] private TileBase[] floorTiles;  // 对应TileType的瓦片
        [SerializeField] private TileBase[] wallTiles;
        [SerializeField] private TileBase previewTile;
        
        [Header("物体预制体容器")]
        [SerializeField] private Transform objectContainer;
        
        [Header("UI面板")]
        [SerializeField] private GameObject tilePalettePanel;
        [SerializeField] private GameObject objectPalettePanel;
        [SerializeField] private GameObject toolbarPanel;
        
        [Header("相机")]
        [SerializeField] private Camera editorCamera;
        
        private RoomEditorSystem _editorSystem;
        private Dictionary<string, GameObject> _objectInstances = new Dictionary<string, GameObject>();
        
        // 拖拽状态
        private bool _isDragging;
        private Vector2Int _dragStartPos;
        
        private void Start()
        {
            _editorSystem = this.GetSystem<RoomEditorSystem>();
            InitializeUI();
        }

        private void Update()
        {
            if (!_editorSystem.IsEditing)
                return;

            HandleInput();
            UpdatePreview();
        }

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        #region 初始化

        private void InitializeUI()
        {
            // 默认显示瓦片面板
            ShowTilePalette();
        }

        /// <summary>
        /// 加载房间到编辑器
        /// </summary>
        public void LoadRoom(RoomData roomData)
        {
            ClearEditor();
            _editorSystem.StartEditRoom(roomData);
            RenderRoom();
        }

        /// <summary>
        /// 渲染整个房间
        /// </summary>
        private void RenderRoom()
        {
            var room = _editorSystem.CurrentRoom;
            if (room == null) return;

            // 清空现有渲染
            floorTilemap.ClearAllTiles();
            wallTilemap.ClearAllTiles();
            
            // 渲染瓦片
            foreach (var kvp in room.TemporaryData.TileMap)
            {
                RenderTile(kvp.Value);
            }
            
            // 渲染物体
            foreach (var obj in room.TemporaryData.PlacedObjects)
            {
                RenderObject(obj);
            }
        }

        #endregion

        #region 输入处理

        private void HandleInput()
        {
            // 检查是否点击在UI上
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            Vector2Int tilePos = GetMouseTilePosition();
            _editorSystem.UpdateHoveredPosition(tilePos);

            // 左键放置
            if (Input.GetMouseButtonDown(0))
            {
                _isDragging = true;
                _dragStartPos = tilePos;
                _editorSystem.HandleMouseClick(tilePos, false);
                RenderTile(_editorSystem.CurrentRoom.GetTile(tilePos));
            }
            
            // 持续拖拽
            if (Input.GetMouseButton(0) && _isDragging)
            {
                if (tilePos != _dragStartPos && _editorSystem.CurrentMode == EditorMode.Tile)
                {
                    _editorSystem.HandleMouseClick(tilePos, false);
                    RenderTile(_editorSystem.CurrentRoom.GetTile(tilePos));
                }
            }
            
            if (Input.GetMouseButtonUp(0))
            {
                _isDragging = false;
            }

            // 右键删除
            if (Input.GetMouseButtonDown(1))
            {
                _editorSystem.HandleMouseClick(tilePos, true);
                RefreshTileAt(tilePos);
            }

            // 快捷键
            if (Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.LeftControl))
            {
                SaveRoom();
            }
            
            if (Input.GetKeyDown(KeyCode.G))
            {
                ToggleGrid();
            }
        }

        /// <summary>
        /// 获取鼠标在瓦片地图上的位置
        /// </summary>
        private Vector2Int GetMouseTilePosition()
        {
            Vector3 worldPos = editorCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = floorTilemap.WorldToCell(worldPos);
            return new Vector2Int(cellPos.x, cellPos.y);
        }

        #endregion

        #region 渲染

        /// <summary>
        /// 渲染单个瓦片
        /// </summary>
        private void RenderTile(TileData tileData)
        {
            if (tileData == null) return;

            Vector3Int cellPos = new Vector3Int(tileData.Position.x, tileData.Position.y, 0);
            
            TileBase tile = GetTileAsset(tileData.Type);
            
            if (tileData.Type == ETileType.Wall || tileData.Type == ETileType.Door || tileData.Type == ETileType.Window)
            {
                wallTilemap.SetTile(cellPos, tile);
            }
            else
            {
                floorTilemap.SetTile(cellPos, tile);
            }
        }

        /// <summary>
        /// 渲染物体
        /// </summary>
        private void RenderObject(PlaceableObjectData objData)
        {
            if (_objectInstances.ContainsKey(objData.ObjectId))
                return; // 已经渲染过

            GameObject prefab = Resources.Load<GameObject>(objData.PrefabPath);
            if (prefab == null)
            {
                Debug.LogWarning($"找不到预制体: {objData.PrefabPath}");
                return;
            }

            GameObject instance = Instantiate(prefab, objectContainer);
            
            // 设置位置
            Vector3 worldPos = floorTilemap.CellToWorld(new Vector3Int(objData.Position.x, objData.Position.y, 0));
            instance.transform.position = worldPos;
            
            // 设置旋转
            instance.transform.rotation = Quaternion.Euler(0, 0, objData.Rotation);
            
            _objectInstances[objData.ObjectId] = instance;
        }

        /// <summary>
        /// 刷新指定位置的瓦片显示
        /// </summary>
        private void RefreshTileAt(Vector2Int pos)
        {
            Vector3Int cellPos = new Vector3Int(pos.x, pos.y, 0);
            
            floorTilemap.SetTile(cellPos, null);
            wallTilemap.SetTile(cellPos, null);
            
            var tile = _editorSystem.CurrentRoom?.GetTile(pos);
            if (tile != null)
            {
                RenderTile(tile);
            }
        }

        /// <summary>
        /// 更新预览
        /// </summary>
        private void UpdatePreview()
        {
            previewTilemap.ClearAllTiles();
            
            Vector2Int hoverPos = _editorSystem.HoveredPosition;
            Vector3Int cellPos = new Vector3Int(hoverPos.x, hoverPos.y, 0);
            
            if (_editorSystem.CurrentMode == EditorMode.Tile)
            {
                previewTilemap.SetTile(cellPos, previewTile);
            }
        }

        /// <summary>
        /// 获取瓦片资源
        /// </summary>
        private TileBase GetTileAsset(ETileType type)
        {
            int index = (int)type;
            
            if (type == ETileType.Wall || type == ETileType.Door || type == ETileType.Window)
            {
                if (index < wallTiles.Length)
                    return wallTiles[index];
            }
            else
            {
                if (index < floorTiles.Length)
                    return floorTiles[index];
            }
            
            return null;
        }

        #endregion

        #region UI事件

        /// <summary>
        /// 显示瓦片面板
        /// </summary>
        public void ShowTilePalette()
        {
            tilePalettePanel.SetActive(true);
            objectPalettePanel.SetActive(false);
            _editorSystem.SetEditorMode(EditorMode.Tile);
        }

        /// <summary>
        /// 显示物体面板
        /// </summary>
        public void ShowObjectPalette()
        {
            tilePalettePanel.SetActive(false);
            objectPalettePanel.SetActive(true);
            _editorSystem.SetEditorMode(EditorMode.Object);
        }

        /// <summary>
        /// 选择瓦片类型
        /// </summary>
        public void OnTileTypeSelected(int tileTypeIndex)
        {
            ETileType type = (ETileType)tileTypeIndex;
            _editorSystem.SelectTileType(type);
        }

        /// <summary>
        /// 选择物体模板
        /// </summary>
        public void OnObjectTemplateSelected(PlaceableObjectData template)
        {
            _editorSystem.SelectObjectTemplate(template);
        }

        /// <summary>
        /// 保存房间
        /// </summary>
        public void SaveRoom()
        {
            _editorSystem.AutoSave();
            Debug.Log("房间已保存");
        }

        /// <summary>
        /// 清空编辑器
        /// </summary>
        public void ClearEditor()
        {
            floorTilemap.ClearAllTiles();
            wallTilemap.ClearAllTiles();
            previewTilemap.ClearAllTiles();
            
            foreach (var obj in _objectInstances.Values)
            {
                if (obj != null)
                    Destroy(obj);
            }
            _objectInstances.Clear();
        }

        /// <summary>
        /// 切换网格显示
        /// </summary>
        public void ToggleGrid()
        {
            var grid = floorTilemap.GetComponentInParent<Grid>();
            if (grid != null)
            {
                var renderer = grid.GetComponent<Renderer>();
                if (renderer != null)
                    renderer.enabled = !renderer.enabled;
            }
        }

        /// <summary>
        /// 填充工具
        /// </summary>
        public void FillArea()
        {
            // 实现区域填充UI逻辑
            Debug.Log("填充工具");
        }

        /// <summary>
        /// 清空所有
        /// </summary>
        public void ClearAll()
        {
            if (_editorSystem.CurrentRoom != null)
            {
                _editorSystem.ClearAllTiles();
                ClearEditor();
            }
        }

        #endregion
    }
}