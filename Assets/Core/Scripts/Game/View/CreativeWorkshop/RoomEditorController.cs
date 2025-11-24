using System.Collections.Generic;
using Core.Game.Chunk.Room;
using Core.Game.Chunk.Room.Data;
using GDFrameworkCore;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using TileData = Core.Game.Chunk.Room.TileData;

namespace Core.Game.View.Room
{
    /// <summary>
    /// 房间编辑器主控制器 - 增强版
    /// </summary>
    public class RoomEditorController : MonoBehaviour, ICanGetSystem
    {
        [Header("系统引用")]
        [SerializeField] private RoomEditorUIController uiController;
        
        [Header("UI面板引用")]
        [SerializeField] private RoomEditorToolbar toolbar;
        [SerializeField] private TilePalettePanel tilePalette;
        [SerializeField] private ObjectPalettePanel objectPalette;
        [SerializeField] private ConfirmDialog confirmDialog;
        [SerializeField] private FillAreaDialog fillAreaDialog;
        [SerializeField] private RoomLoadDialog roomLoadDialog;
        [SerializeField] private RoomPropertiesPanel propertiesPanel;
        [SerializeField] private KeyboardShortcutsPanel shortcutsPanel;
        
        [Header("相机控制")]
        [SerializeField] private Camera editorCamera;
        [SerializeField] private float zoomSpeed = 0.5f;
        [SerializeField] private float panSpeed = 0.1f;
        [SerializeField] private float minZoom = 5f;
        [SerializeField] private float maxZoom = 20f;
        
        private RoomEditorSystem _editorSystem;
        private bool _isPanning;
        private Vector3 _lastPanPosition;
        
        // 填充模式状态
        private bool _isFillMode;
        private Vector2Int? _fillStartPos;
        
        private void Awake()
        {
            _editorSystem = this.GetSystem<RoomEditorSystem>();
        }
        
        private void Start()
        {
            InitializeEditor();
        }
        
        private void Update()
        {
            if (!_editorSystem.IsEditing) return;
            
            HandleKeyboardInput();
            HandleCameraControl();
        }
        
        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }
        
        #region 初始化
        
        private void InitializeEditor()
        {
            // 默认隐藏所有面板
            tilePalette.gameObject.SetActive(false);
            objectPalette.gameObject.SetActive(false);
            confirmDialog.gameObject.SetActive(false);
            fillAreaDialog.gameObject.SetActive(false);
            roomLoadDialog.gameObject.SetActive(false);
            propertiesPanel.gameObject.SetActive(false);
            shortcutsPanel.gameObject.SetActive(false);
        }
        
        #endregion
        
        #region 房间管理
        
        /// <summary>
        /// 创建新房间
        /// </summary>
        public void CreateNewRoom()
        {
            var roomDef = new RoomDtoDef
            {
                DefName = "新房间",
                Width = 20,
                Height = 20,
                HasOutdoorArea = false,
                DefaultFloorType = ETileType.Floor
            };
            
            propertiesPanel.Show(roomDef, (def) =>
            {
                var roomData = new RoomData();
                roomData.InitChunkData(def);
                
                // 初始化默认地板
                InitializeDefaultFloor(roomData);
                
                StartEditingRoom(roomData);
            });
        }
        
        /// <summary>
        /// 加载现有房间
        /// </summary>
        public void LoadExistingRoom()
        {
            roomLoadDialog.Show((roomDef) =>
            {
                var roomData = new RoomData();
                roomData.InitChunkData(roomDef);
                StartEditingRoom(roomData);
            });
        }
        
        /// <summary>
        /// 开始编辑房间
        /// </summary>
        private void StartEditingRoom(RoomData roomData)
        {
            uiController.LoadRoom(roomData);
            
            // 显示工具栏和瓦片面板
            toolbar.gameObject.SetActive(true);
            tilePalette.gameObject.SetActive(true);
            
            Debug.Log($"开始编辑房间: {roomData.DtoDef.DefName}");
        }
        
        /// <summary>
        /// 初始化默认地板
        /// </summary>
        private void InitializeDefaultFloor(RoomData roomData)
        {
            var def = roomData.DtoDef;
            
            for (int x = 0; x < def.Width; x++)
            {
                for (int y = 0; y < def.Height; y++)
                {
                    var tile = new TileData(new Vector2Int(x, y), def.DefaultFloorType);
                    roomData.SetTile(new Vector2Int(x, y), tile);
                }
            }
            
            roomData.SaveTemporaryData();
        }
        
        /// <summary>
        /// 保存当前房间
        /// </summary>
        public void SaveCurrentRoom()
        {
            if (_editorSystem.CurrentRoom == null)
            {
                Debug.LogWarning("没有正在编辑的房间");
                return;
            }
            
            _editorSystem.AutoSave();
            _editorSystem.CurrentRoom.DtoDef.SaveThisDef();
            
            ShowMessage("房间已保存!");
        }
        
        /// <summary>
        /// 退出编辑器
        /// </summary>
        public void ExitEditor()
        {
            confirmDialog.Show(
                "确定要退出编辑器吗?\n未保存的更改将丢失。",
                () =>
                {
                    _editorSystem.StopEditRoom();
                    
                    // 隐藏所有UI
                    toolbar.gameObject.SetActive(false);
                    tilePalette.gameObject.SetActive(false);
                    objectPalette.gameObject.SetActive(false);
                    
                    // TODO: 返回主菜单或上一个场景
                    Debug.Log("退出房间编辑器");
                }
            );
        }
        
        #endregion
        
        #region 键盘输入处理
        
        private void HandleKeyboardInput()
        {
            // 保存
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S))
            {
                SaveCurrentRoom();
            }
            
            // 模式切换
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _editorSystem.SetEditorMode(EditorMode.Tile);
                tilePalette.gameObject.SetActive(true);
                objectPalette.gameObject.SetActive(false);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _editorSystem.SetEditorMode(EditorMode.Object);
                tilePalette.gameObject.SetActive(false);
                objectPalette.gameObject.SetActive(true);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                _editorSystem.SetEditorMode(EditorMode.Erase);
            }
            
            // 网格切换
            if (Input.GetKeyDown(KeyCode.G))
            {
                uiController.ToggleGrid();
            }
            
            // 退出
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ExitEditor();
            }
            
            // 快捷键提示
            if (Input.GetKeyDown(KeyCode.F1))
            {
                shortcutsPanel.Show();
            }
        }
        
        #endregion
        
        #region 相机控制
        
        private void HandleCameraControl()
        {
            // 缩放
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                float newSize = editorCamera.orthographicSize - scroll * zoomSpeed;
                editorCamera.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
            }
            
            // 平移
            if (Input.GetMouseButtonDown(2)) // 鼠标中键按下
            {
                _isPanning = true;
                _lastPanPosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(2))
            {
                _isPanning = false;
            }
            
            if (_isPanning)
            {
                Vector3 delta = Input.mousePosition - _lastPanPosition;
                Vector3 move = new Vector3(-delta.x * panSpeed, -delta.y * panSpeed, 0);
                editorCamera.transform.Translate(move, Space.World);
                _lastPanPosition = Input.mousePosition;
            }
        }
        
        #endregion
        
        #region 填充功能
        
        /// <summary>
        /// 启动填充模式
        /// </summary>
        public void StartFillMode()
        {
            _isFillMode = true;
            _fillStartPos = null;
            
            fillAreaDialog.StartSelection((start, end) =>
            {
                _editorSystem.FillArea(start, end, _editorSystem.SelectedTileType);
                uiController.LoadRoom(_editorSystem.CurrentRoom);
                _isFillMode = false;
            });
        }
        
        /// <summary>
        /// 处理填充模式的点击
        /// </summary>
        public void HandleFillModeClick(Vector2Int position)
        {
            if (!_isFillMode) return;
            
            if (!_fillStartPos.HasValue)
            {
                _fillStartPos = position;
                fillAreaDialog.SetStartPosition(position);
            }
            else
            {
                fillAreaDialog.SetEndPosition(position);
            }
        }
        
        #endregion
        
        #region 辅助功能
        
        /// <summary>
        /// 显示消息
        /// </summary>
        private void ShowMessage(string message)
        {
            Debug.Log($"[房间编辑器] {message}");
            // TODO: 实现Toast提示
        }
        
        /// <summary>
        /// 清空所有内容
        /// </summary>
        public void ClearAllContent()
        {
            confirmDialog.Show(
                "确定要清空所有内容吗?\n此操作不可撤销!",
                () =>
                {
                    _editorSystem.ClearAllTiles();
                    
                    if (_editorSystem.CurrentRoom != null)
                    {
                        _editorSystem.CurrentRoom.TemporaryData.PlacedObjects.Clear();
                    }
                    
                    uiController.ClearEditor();
                    ShowMessage("已清空所有内容");
                }
            );
        }
        
        /// <summary>
        /// 显示房间属性
        /// </summary>
        public void ShowRoomProperties()
        {
            if (_editorSystem.CurrentRoom == null) return;
            
            propertiesPanel.Show(_editorSystem.CurrentRoom.DtoDef, (def) =>
            {
                // 如果尺寸改变,需要重新加载
                uiController.LoadRoom(_editorSystem.CurrentRoom);
                ShowMessage("房间属性已更新");
            });
        }
        
        #endregion
    }
    
    /// <summary>
    /// 自动保存管理器
    /// </summary>
    public class AutoSaveManager : MonoBehaviour, ICanGetSystem
    {
        [Header("自动保存设置")]
        [SerializeField] private float autoSaveInterval = 300f; // 5分钟
        [SerializeField] private bool enableAutoSave = true;
        
        private RoomEditorSystem _editorSystem;
        private float _timeSinceLastSave;
        
        private void Start()
        {
            _editorSystem = this.GetSystem<RoomEditorSystem>();
        }
        
        private void Update()
        {
            if (!enableAutoSave || !_editorSystem.IsEditing)
                return;
            
            _timeSinceLastSave += Time.deltaTime;
            
            if (_timeSinceLastSave >= autoSaveInterval)
            {
                PerformAutoSave();
                _timeSinceLastSave = 0f;
            }
        }
        
        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }
        
        private void PerformAutoSave()
        {
            _editorSystem.AutoSave();
            Debug.Log($"[自动保存] 房间数据已保存 - {System.DateTime.Now:HH:mm:ss}");
        }
        
        public void ResetTimer()
        {
            _timeSinceLastSave = 0f;
        }
    }
    
    /// <summary>
    /// 性能监视器
    /// </summary>
    public class EditorPerformanceMonitor : MonoBehaviour
    {
        [Header("显示设置")]
        [SerializeField] private bool showFPS = true;
        [SerializeField] private TextMesh fpsText;
        
        private float _deltaTime;
        
        private void Update()
        {
            if (!showFPS) return;
            
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
            float fps = 1.0f / _deltaTime;
            
            if (fpsText != null)
            {
                fpsText.text = $"FPS: {Mathf.Ceil(fps)}";
            }
        }
    }
}