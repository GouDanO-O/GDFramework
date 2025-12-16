using System;
using System.Collections.Generic;
using GDFrameworkCore;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Game.Chunk.Room.Grid.Editor
{
    /// <summary>
    /// 房间网格编辑器主控制器
    /// 管理编辑器的所有功能
    /// </summary>
    public class RoomGridEditor : MonoBehaviour, IController
    {
        #region 配置

        [Title("组件引用")]
        
        [LabelText("编辑器相机")]
        [SerializeField]
        private RoomGridEditorCamera _editorCamera;

        [LabelText("预览物体根节点")]
        [SerializeField]
        private Transform _previewRoot;

        [LabelText("地块渲染器")]
        [SerializeField]
        private Transform _tileRendererRoot;

        [Title("调试")]
        
        [LabelText("显示调试信息")]
        [SerializeField]
        private bool _showDebugInfo = true;

        [LabelText("显示网格线")]
        [SerializeField]
        private bool _showGridLines = true;

        #endregion

        #region 属性

        /// <summary>
        /// 编辑器状态
        /// </summary>
        public RoomGridEditorState State { get; private set; }

        /// <summary>
        /// 输入处理器
        /// </summary>
        public RoomGridEditorInput Input { get; private set; }

        /// <summary>
        /// 房间网格
        /// </summary>
        public RoomGrid Grid { get; private set; }

        /// <summary>
        /// 是否已初始化
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// 编辑器相机
        /// </summary>
        public RoomGridEditorCamera EditorCamera => _editorCamera;

        #endregion

        #region 事件

        /// <summary>
        /// 编辑器初始化完成
        /// </summary>
        public event UnityAction OnEditorInitialized;

        /// <summary>
        /// 地块被修改
        /// </summary>
        public event Action<TilePosition, TileData> OnTileModified;

        /// <summary>
        /// 物品被放置
        /// </summary>
        public event Action<PlacedObjectData> OnObjectPlaced;

        /// <summary>
        /// 物品被移除
        /// </summary>
        public event Action<PlacedObjectData> OnObjectRemoved;

        #endregion

        #region 私有字段

        private IArchitecture _architecture;
        private List<TilePosition> _currentBrushPositions = new List<TilePosition>();
        private PlacedObjectData _previewObject;

        #endregion

        #region 生命周期

        public IArchitecture GetArchitecture()
        {
            return _architecture ?? GameMain.Interface;
        }

        private void Awake()
        {
            _architecture = GetArchitecture();

            // 自动获取组件引用
            AutoSetupComponents();
        }

        /// <summary>
        /// 自动设置组件引用
        /// 当组件未在Inspector中绑定时自动查找或创建
        /// </summary>
        private void AutoSetupComponents()
        {
            // 自动获取编辑器相机
            if (_editorCamera == null)
            {
                _editorCamera = GetComponentInChildren<RoomGridEditorCamera>();
                if (_editorCamera == null)
                {
                    _editorCamera = FindFirstObjectByType<RoomGridEditorCamera>();
                }
                if (_editorCamera == null)
                {
                    // 创建编辑器相机
                    var cameraGO = new GameObject("EditorCamera");
                    cameraGO.transform.SetParent(transform);

                    // 创建Pivot
                    var pivotGO = new GameObject("CameraPivot");
                    pivotGO.transform.SetParent(cameraGO.transform);
                    pivotGO.transform.position = Vector3.zero;

                    // 创建Camera
                    var camGO = new GameObject("Camera");
                    camGO.transform.SetParent(pivotGO.transform);
                    var cam = camGO.AddComponent<Camera>();
                    cam.clearFlags = CameraClearFlags.SolidColor;
                    cam.backgroundColor = new Color(0.2f, 0.3f, 0.4f);
                    camGO.transform.localPosition = new Vector3(0, 0, -20f);

                    _editorCamera = cameraGO.AddComponent<RoomGridEditorCamera>();
                    Debug.Log("[RoomGridEditor] 自动创建了编辑器相机");
                }
            }

            // 自动获取或创建预览物体根节点
            if (_previewRoot == null)
            {
                var previewGO = transform.Find("PreviewRoot");
                if (previewGO != null)
                {
                    _previewRoot = previewGO;
                }
                else
                {
                    var newPreviewRoot = new GameObject("PreviewRoot");
                    newPreviewRoot.transform.SetParent(transform);
                    newPreviewRoot.transform.localPosition = Vector3.zero;
                    _previewRoot = newPreviewRoot.transform;
                    Debug.Log("[RoomGridEditor] 自动创建了预览物体根节点");
                }
            }

            // 自动获取或创建地块渲染器根节点
            if (_tileRendererRoot == null)
            {
                var tileRenderGO = transform.Find("TileRendererRoot");
                if (tileRenderGO != null)
                {
                    _tileRendererRoot = tileRenderGO;
                }
                else
                {
                    var newTileRendererRoot = new GameObject("TileRendererRoot");
                    newTileRendererRoot.transform.SetParent(transform);
                    newTileRendererRoot.transform.localPosition = Vector3.zero;
                    _tileRendererRoot = newTileRendererRoot.transform;
                    Debug.Log("[RoomGridEditor] 自动创建了地块渲染器根节点");
                }
            }
        }

        private void Update()
        {
            if (!IsInitialized) return;

            // 更新输入（包括鼠标位置和键盘）
            Input?.Update();

            // 更新预览
            UpdatePreview();

            // 更新调试显示
            if (_showDebugInfo)
            {
                UpdateDebugInfo();
            }
        }

        private void OnDestroy()
        {
            Cleanup();
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化编辑器
        /// </summary>
        public void Initialize(RoomGridConfig config = null)
        {
            if (IsInitialized)
            {
                Debug.LogWarning("[RoomGridEditor] 已经初始化过了");
                return;
            }

            // 创建配置
            config ??= new RoomGridConfig();

            // 创建网格
            Grid = new RoomGrid(config);
            Grid.Initialize();

            // 创建状态
            State = new RoomGridEditorState();

            // 创建输入处理器
            Input = new RoomGridEditorInput(_architecture);
            Input.Initialize(_editorCamera?.TargetCamera ?? Camera.main, config);

            // 订阅输入事件
            SubscribeInputEvents();

            // 订阅网格事件
            SubscribeGridEvents();

            // 初始化相机
            InitializeCamera(config);

            IsInitialized = true;
            OnEditorInitialized?.Invoke();

            Debug.Log($"[RoomGridEditor] 初始化完成: {config}");
        }

        /// <summary>
        /// 从已有网格初始化
        /// </summary>
        public void InitializeWithGrid(RoomGrid grid)
        {
            if (grid == null)
            {
                Debug.LogError("[RoomGridEditor] Grid 不能为空");
                return;
            }

            Grid = grid;

            // 创建状态
            State = new RoomGridEditorState();

            // 创建输入处理器
            Input = new RoomGridEditorInput(_architecture);
            Input.Initialize(_editorCamera?.TargetCamera ?? Camera.main, grid.Config);

            // 订阅事件
            SubscribeInputEvents();
            SubscribeGridEvents();

            // 初始化相机
            InitializeCamera(grid.Config);

            IsInitialized = true;
            OnEditorInitialized?.Invoke();

            Debug.Log($"[RoomGridEditor] 从已有网格初始化完成");
        }

        /// <summary>
        /// 初始化相机
        /// </summary>
        private void InitializeCamera(RoomGridConfig config)
        {
            if (_editorCamera == null) return;

            _editorCamera.SetArchitecture(_architecture);
            _editorCamera.Initialize();
            _editorCamera.SetBoundsFromGrid(config);

            // 聚焦到网格中心
            _editorCamera.FocusOn(config.WorldCenter, true);
        }

        /// <summary>
        /// 订阅输入事件
        /// </summary>
        private void SubscribeInputEvents()
        {
            Input.OnLeftClick += HandleLeftClick;
            Input.OnLeftDragStart += HandleDragStart;
            Input.OnLeftDragging += HandleDragging;
            Input.OnLeftDragEnd += HandleDragEnd;
            Input.OnRightClick += HandleRightClick;
            Input.OnMouseMove += HandleMouseMove;
            Input.OnScroll += HandleScroll;
            Input.OnRotateKey += HandleRotateKey;
            Input.OnDeleteKey += HandleDeleteKey;
            Input.OnCancelKey += HandleCancelKey;
        }

        /// <summary>
        /// 订阅网格事件
        /// </summary>
        private void SubscribeGridEvents()
        {
            Grid.OnTileChanged += HandleTileChanged;
            Grid.OnObjectPlaced += HandleObjectPlaced;
            Grid.OnObjectRemoved += HandleObjectRemoved;
        }

        /// <summary>
        /// 清理
        /// </summary>
        private void Cleanup()
        {
            if (Input != null)
            {
                Input.OnLeftClick -= HandleLeftClick;
                Input.OnLeftDragStart -= HandleDragStart;
                Input.OnLeftDragging -= HandleDragging;
                Input.OnLeftDragEnd -= HandleDragEnd;
                Input.OnRightClick -= HandleRightClick;
                Input.OnMouseMove -= HandleMouseMove;
                Input.OnScroll -= HandleScroll;
                Input.OnRotateKey -= HandleRotateKey;
                Input.OnDeleteKey -= HandleDeleteKey;
                Input.OnCancelKey -= HandleCancelKey;
                
                Input.Dispose();
            }

            if (Grid != null)
            {
                Grid.OnTileChanged -= HandleTileChanged;
                Grid.OnObjectPlaced -= HandleObjectPlaced;
                Grid.OnObjectRemoved -= HandleObjectRemoved;
            }

            State?.Reset();
            IsInitialized = false;
        }

        #endregion

        #region 输入事件处理

        private void HandleLeftClick(Vector3 worldPos, TilePosition tilePos)
        {
            if (!Grid.Config.IsInBounds(tilePos)) return;

            switch (State.CurrentMode)
            {
                case EditorMode.TileEdit:
                    ExecuteTileEdit(tilePos);
                    break;
                    
                case EditorMode.ObjectPlace:
                    ExecuteObjectPlace(tilePos);
                    break;
                    
                case EditorMode.ObjectSelect:
                    ExecuteObjectSelect(tilePos);
                    break;
                    
                case EditorMode.Delete:
                    ExecuteDelete(tilePos);
                    break;
            }
        }

        private void HandleDragStart(Vector3 worldPos, TilePosition tilePos)
        {
            if (!Grid.Config.IsInBounds(tilePos)) return;
            
            State.BeginOperation(tilePos);
            Debug.Log($"[RoomGridEditor] 拖拽开始 - 位置: {tilePos}, 模式: {State.CurrentMode}");

            if (State.CurrentMode == EditorMode.TileEdit)
            {
                // 拖拽开始时立即执行一次编辑
                ExecuteTileEdit(tilePos);
            }
        }

        private void HandleDragging(Vector3 worldPos, TilePosition tilePos)
        {
            if (!State.IsOperating) 
            {
                Debug.Log($"[RoomGridEditor] 拖拽中但IsOperating=false");
                return;
            }
            
            if (!Grid.Config.IsInBounds(tilePos)) return;

            if (State.CurrentMode == EditorMode.TileEdit)
            {
                if (State.CurrentTileTool == TileEditTool.Brush || 
                    State.CurrentTileTool == TileEditTool.Eraser)
                {
                    Debug.Log($"[RoomGridEditor] 拖拽绘制 - 位置: {tilePos}");
                    ExecuteTileEdit(tilePos);
                }
            }
        }

        private void HandleDragEnd(Vector3 worldPos, TilePosition tilePos)
        {
            if (!State.IsOperating) 
            {
                Debug.Log($"[RoomGridEditor] 拖拽结束但IsOperating=false");
                return;
            }

            Debug.Log($"[RoomGridEditor] 拖拽结束 - 起始: {State.DragStartPosition}, 终点: {tilePos}");

            if (State.CurrentMode == EditorMode.TileEdit)
            {
                if (State.CurrentTileTool == TileEditTool.Rectangle)
                {
                    ExecuteRectangleFill(State.DragStartPosition, tilePos);
                }
            }

            State.EndOperation();
        }

        private void HandleRightClick(Vector3 worldPos, TilePosition tilePos)
        {
            // 右键取消当前操作或返回上一级
            if (State.IsOperating)
            {
                State.EndOperation();
            }
            else if (State.CurrentMode != EditorMode.View)
            {
                State.ExitToViewMode();
            }
        }

        private void HandleMouseMove(Vector3 worldPos, TilePosition tilePos)
        {
            bool isValid = Grid.Config.IsInBounds(tilePos);
            State.UpdateMousePosition(worldPos, tilePos, isValid);

            // 更新画笔范围
            UpdateBrushPositions(tilePos);

            // 更新预览有效性
            UpdatePreviewValidity(tilePos);
        }

        private void HandleScroll(float scrollValue)
        {
            // 如果按住Ctrl，调整画笔大小
            if (UnityEngine.Input.GetKey(KeyCode.LeftControl) || UnityEngine.Input.GetKey(KeyCode.RightControl))
            {
                if (scrollValue > 0)
                    State.IncreaseBrushSize();
                else if (scrollValue < 0)
                    State.DecreaseBrushSize();
            }
        }

        private void HandleRotateKey(bool clockwise)
        {
            if (State.CurrentMode == EditorMode.ObjectPlace)
            {
                State.RotateObject(clockwise);
            }
            else if (State.HasSelectedObject())
            {
                Grid.RotateObject(State.SelectedObjectInstanceId, clockwise);
            }
        }

        private void HandleDeleteKey()
        {
            if (State.HasSelectedObject())
            {
                Grid.RemoveObject(State.SelectedObjectInstanceId);
                State.DeselectObject();
            }
        }

        private void HandleCancelKey()
        {
            if (State.IsOperating)
            {
                State.EndOperation();
            }
            else
            {
                State.ExitToViewMode();
            }
        }

        #endregion

        #region 编辑操作

        /// <summary>
        /// 执行地块编辑
        /// </summary>
        private void ExecuteTileEdit(TilePosition centerPos)
        {
            var positions = GetBrushPositions(centerPos);
            
            foreach (var pos in positions)
            {
                if (!Grid.Config.IsInBounds(pos)) continue;

                switch (State.CurrentTileTool)
                {
                    case TileEditTool.Brush:
                        Grid.SetTile(pos, State.SelectedTileType, State.HeightLevel);
                        break;
                        
                    case TileEditTool.Eraser:
                        Grid.RemoveTile(pos);
                        break;
                        
                    case TileEditTool.Fill:
                        Grid.FloodFill(pos, State.SelectedTileType);
                        return; // 填充只执行一次
                }
            }
        }

        /// <summary>
        /// 执行矩形填充
        /// </summary>
        private void ExecuteRectangleFill(TilePosition start, TilePosition end)
        {
            Grid.FillArea(start, end, State.SelectedTileType);
        }

        /// <summary>
        /// 执行物品放置
        /// </summary>
        private void ExecuteObjectPlace(TilePosition pos)
        {
            if (string.IsNullOrEmpty(State.SelectedObjectDefId)) return;

            // 从物品定义获取尺寸
            var def = ObjectDefinitionManager.Instance.GetDefinition(State.SelectedObjectDefId);
            if (def == null)
            {
                Debug.LogWarning($"[RoomGridEditor] 找不到物品定义: {State.SelectedObjectDefId}");
                return;
            }

            var size = def.Size;
            var category = def.Category;

            if (Grid.CanPlaceObject(pos, size, State.CurrentRotation))
            {
                Grid.PlaceObject(
                    State.SelectedObjectDefId,
                    pos,
                    size,
                    State.CurrentRotation,
                    category
                );
            }
        }

        /// <summary>
        /// 执行物品选择
        /// </summary>
        private void ExecuteObjectSelect(TilePosition pos)
        {
            var obj = Grid.GetObjectAtPosition(pos);
            if (obj != null)
            {
                State.SelectObjectInstance(obj.InstanceId);
                Debug.Log($"[RoomGridEditor] 选中物品: {obj.InstanceId}");
            }
            else
            {
                State.DeselectObject();
            }
        }

        /// <summary>
        /// 执行删除
        /// </summary>
        private void ExecuteDelete(TilePosition pos)
        {
            // 优先删除物品
            var obj = Grid.GetObjectAtPosition(pos);
            if (obj != null)
            {
                Grid.RemoveObject(obj.InstanceId);
            }
            else
            {
                // 否则删除地块
                Grid.RemoveTile(pos);
            }
        }

        #endregion

        #region 画笔和预览

        /// <summary>
        /// 获取画笔覆盖的位置
        /// </summary>
        private List<TilePosition> GetBrushPositions(TilePosition center)
        {
            var positions = new List<TilePosition>();
            int radius = (State.BrushSize - 1) / 2;

            for (int x = -radius; x <= radius; x++)
            {
                for (int z = -radius; z <= radius; z++)
                {
                    positions.Add(new TilePosition(center.X + x, center.Z + z));
                }
            }

            return positions;
        }

        /// <summary>
        /// 更新画笔位置列表
        /// </summary>
        private void UpdateBrushPositions(TilePosition center)
        {
            _currentBrushPositions = GetBrushPositions(center);
        }

        /// <summary>
        /// 更新预览有效性
        /// </summary>
        private void UpdatePreviewValidity(TilePosition pos)
        {
            if (State.CurrentMode == EditorMode.ObjectPlace)
            {
                // 从物品定义获取尺寸
                var def = ObjectDefinitionManager.Instance.GetDefinition(State.SelectedObjectDefId);
                var size = def?.Size ?? ObjectSize.One;
                State.IsPreviewValid = Grid.CanPlaceObject(pos, size, State.CurrentRotation);
            }
            else if (State.CurrentMode == EditorMode.TileEdit)
            {
                State.IsPreviewValid = Grid.Config.IsInBounds(pos);
            }
        }

        /// <summary>
        /// 更新预览显示
        /// </summary>
        private void UpdatePreview()
        {
            // TODO: 实现预览可视化
            State.ShowPreview = State.CurrentMode == EditorMode.TileEdit || 
                               State.CurrentMode == EditorMode.ObjectPlace;
        }

        #endregion

        #region 网格事件处理

        private void HandleTileChanged(TilePosition pos, TileData tile)
        {
            OnTileModified?.Invoke(pos, tile);
            // TODO: 更新渲染
        }

        private void HandleObjectPlaced(PlacedObjectData obj)
        {
            OnObjectPlaced?.Invoke(obj);
            // TODO: 创建物品显示
        }

        private void HandleObjectRemoved(PlacedObjectData obj)
        {
            OnObjectRemoved?.Invoke(obj);
            // TODO: 移除物品显示
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 设置编辑模式
        /// </summary>
        public void SetMode(EditorMode mode)
        {
            State.SetMode(mode);
        }

        /// <summary>
        /// 设置地块编辑工具
        /// </summary>
        public void SetTileTool(TileEditTool tool)
        {
            State.SetTileTool(tool);
        }

        /// <summary>
        /// 设置选中的地块类型
        /// </summary>
        public void SetTileType(TileType type)
        {
            State.SetSelectedTileType(type);
        }

        /// <summary>
        /// 进入物品放置模式
        /// </summary>
        public void StartPlaceObject(string objectDefId)
        {
            State.EnterObjectPlaceMode(objectDefId);
        }

        /// <summary>
        /// 获取网格统计
        /// </summary>
        public RoomGridStatistics GetStatistics()
        {
            return Grid?.GetStatistics() ?? default;
        }

        /// <summary>
        /// 保存网格数据
        /// </summary>
        public string SaveToJson()
        {
            if (Grid == null) return null;
            return Newtonsoft.Json.JsonConvert.SerializeObject(Grid);
        }

        /// <summary>
        /// 从JSON加载
        /// </summary>
        public void LoadFromJson(string json)
        {
            if (string.IsNullOrEmpty(json)) return;
            
            var grid = Newtonsoft.Json.JsonConvert.DeserializeObject<RoomGrid>(json);
            if (grid != null)
            {
                InitializeWithGrid(grid);
            }
        }

        #endregion

        #region 调试

        private void UpdateDebugInfo()
        {
            // TODO: 更新调试UI
        }

        private void OnDrawGizmos()
        {
            if (!IsInitialized || !_showGridLines) return;
            if (Grid?.Config == null) return;

            var config = Grid.Config;

            // 绘制网格线
            Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
            
            for (int x = 0; x <= config.Width; x++)
            {
                Vector3 start = new Vector3(x * config.TileSize, 0.01f, 0);
                Vector3 end = new Vector3(x * config.TileSize, 0.01f, config.Depth * config.TileSize);
                Gizmos.DrawLine(start, end);
            }

            for (int z = 0; z <= config.Depth; z++)
            {
                Vector3 start = new Vector3(0, 0.01f, z * config.TileSize);
                Vector3 end = new Vector3(config.Width * config.TileSize, 0.01f, z * config.TileSize);
                Gizmos.DrawLine(start, end);
            }

            // 绘制画笔预览
            if (State != null && State.ShowPreview && _currentBrushPositions != null)
            {
                Gizmos.color = State.IsPreviewValid ? 
                    new Color(0, 1, 0, 0.3f) : 
                    new Color(1, 0, 0, 0.3f);

                foreach (var pos in _currentBrushPositions)
                {
                    if (!config.IsInBounds(pos)) continue;
                    
                    Vector3 worldPos = config.TileToWorld(pos);
                    Vector3 size = new Vector3(config.TileSize, 0.1f, config.TileSize);
                    Gizmos.DrawCube(worldPos, size * 0.95f);
                }
            }
        }

        #endregion
    }
}