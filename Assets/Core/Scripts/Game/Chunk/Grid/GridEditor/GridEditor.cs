using System;
using Core.Game.Grid.Data;
using GDFrameworkExtend.LogKit;
using UnityEngine;

namespace Core.Game.Grid.Editor
{
    /// <summary>
    /// 网格编辑器核心
    /// 统一管理编辑器的状态、输入和工具
    /// </summary>
    public class GridEditor : MonoBehaviour
    {
        #region 组件引用

        [Header("必需组件")]
        [SerializeField] private Camera _editorCamera;

        #endregion

        #region 核心模块

        /// <summary>
        /// 编辑器状态
        /// </summary>
        public GridEditorState State { get; private set; }

        /// <summary>
        /// 输入处理
        /// </summary>
        public GridEditorInput Input { get; private set; }

        /// <summary>
        /// 网格管理器引用
        /// </summary>
        public GridManager GridManager { get; private set; }

        #endregion

        #region 编辑器设置

        [Header("编辑器设置")]
        [SerializeField] private bool _enableInput = true;
        [SerializeField] private float _dragThreshold = 5f;

        #endregion

        #region 生命周期

        private void Awake()
        {
            InitializeEditor();
        }

        private void Update()
        {
            if (State == null || Input == null)
                return;

            // 更新输入
            Input.Update(State.CurrentGrid?.Grid, State.CurrentLayer);

            // 更新当前鼠标网格位置
            UpdateMousePosition();
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
        private void InitializeEditor()
        {
            // 初始化状态
            State = new GridEditorState();

            // 初始化输入
            if (_editorCamera == null)
            {
                _editorCamera = Camera.main;
            }

            Input = new GridEditorInput(_editorCamera)
            {
                InputEnabled = _enableInput,
                DragThreshold = _dragThreshold
            };

            // 订阅输入事件
            SubscribeInputEvents();

            // 订阅状态事件
            SubscribeStateEvents();

            LogKit.Log("[GridEditor] 编辑器初始化完成");
        }

        /// <summary>
        /// 订阅输入事件
        /// </summary>
        private void SubscribeInputEvents()
        {
            Input.OnMouseClick += HandleMouseClick;
            Input.OnDragStart += HandleDragStart;
            Input.OnDragging += HandleDragging;
            Input.OnDragEnd += HandleDragEnd;
            Input.OnMouseMove += HandleMouseMove;
            Input.OnKeyPressed += HandleKeyPressed;
        }

        /// <summary>
        /// 订阅状态事件
        /// </summary>
        private void SubscribeStateEvents()
        {
            State.OnModeChanged += HandleModeChanged;
            State.OnToolChanged += HandleToolChanged;
            State.OnSelectionChanged += HandleSelectionChanged;
            State.OnGridChanged += HandleGridChanged;
        }

        /// <summary>
        /// 清理
        /// </summary>
        private void Cleanup()
        {
            // 取消订阅
            if (Input != null)
            {
                Input.OnMouseClick -= HandleMouseClick;
                Input.OnDragStart -= HandleDragStart;
                Input.OnDragging -= HandleDragging;
                Input.OnDragEnd -= HandleDragEnd;
                Input.OnMouseMove -= HandleMouseMove;
                Input.OnKeyPressed -= HandleKeyPressed;
            }

            if (State != null)
            {
                State.OnModeChanged -= HandleModeChanged;
                State.OnToolChanged -= HandleToolChanged;
                State.OnSelectionChanged -= HandleSelectionChanged;
                State.OnGridChanged -= HandleGridChanged;
            }
        }

        #endregion

        #region 输入事件处理

        private void HandleMouseClick(GridPosition gridPos, int button)
        {
            if (State.CurrentGrid == null)
                return;

            switch (State.CurrentMode)
            {
                case EditorMode.Structure:
                    HandleStructureClick(gridPos, button);
                    break;
                case EditorMode.Object:
                    HandleObjectClick(gridPos, button);
                    break;
            }
        }

        private void HandleDragStart(GridPosition gridPos)
        {
            State.IsOperating = true;
            State.DragStartPosition = gridPos;

            LogKit.Log($"[GridEditor] 开始拖拽: {gridPos}");
        }

        private void HandleDragging(GridPosition gridPos)
        {
            if (!State.IsOperating)
                return;

            // 根据当前模式和工具处理拖拽
            switch (State.CurrentMode)
            {
                case EditorMode.Structure:
                    HandleStructureDrag(gridPos);
                    break;
                case EditorMode.Object:
                    HandleObjectDrag(gridPos);
                    break;
            }
        }

        private void HandleDragEnd(GridPosition gridPos)
        {
            State.IsOperating = false;

            LogKit.Log($"[GridEditor] 结束拖拽: {gridPos}");
        }

        private void HandleMouseMove(GridPosition gridPos)
        {
            State.CurrentMouseGridPosition = gridPos;

            // 更新预览
            UpdatePreview(gridPos);
        }

        private void HandleKeyPressed(KeyCode key)
        {
            switch (key)
            {
                case KeyCode.Delete:
                    DeleteSelected();
                    break;
                case KeyCode.Escape:
                    CancelOperation();
                    break;
                case KeyCode.Z:
                    // TODO: 撤销
                    LogKit.Log("[GridEditor] 撤销");
                    break;
                case KeyCode.Y:
                    // TODO: 重做
                    LogKit.Log("[GridEditor] 重做");
                    break;
                case KeyCode.R:
                    RotateObject();
                    break;
                case KeyCode.G:
                    State.ShowGrid = !State.ShowGrid;
                    break;
                case KeyCode.H:
                    State.ShowGuides = !State.ShowGuides;
                    break;
                case KeyCode.Alpha1:
                    SetLayer(0);
                    break;
                case KeyCode.Alpha2:
                    SetLayer(1);
                    break;
                case KeyCode.Alpha3:
                    SetLayer(2);
                    break;
            }
        }

        #endregion

        #region 状态事件处理

        private void HandleModeChanged(EditorMode oldMode, EditorMode newMode)
        {
            LogKit.Log($"[GridEditor] 模式切换: {oldMode} -> {newMode}");
        }

        private void HandleToolChanged(object oldTool, object newTool)
        {
            LogKit.Log($"[GridEditor] 工具切换: {oldTool} -> {newTool}");
        }

        private void HandleSelectionChanged(string objectId)
        {
            LogKit.Log($"[GridEditor] 选择改变: {objectId ?? "None"}");
        }

        private void HandleGridChanged(GridData grid)
        {
            LogKit.Log($"[GridEditor] 网格切换: {grid?.DefId ?? "None"}");
        }

        #endregion

        #region 结构编辑

        private void HandleStructureClick(GridPosition gridPos, int button)
        {
            if (button == 0) // 左键
            {
                switch (State.CurrentStructureTool)
                {
                    case StructureToolType.DrawWall:
                        PlaceWall(gridPos);
                        break;
                    case StructureToolType.EraseWall:
                        EraseWall(gridPos);
                        break;
                    case StructureToolType.DrawFloor:
                        PlaceFloor(gridPos);
                        break;
                    // TODO: 其他工具
                }
            }
        }

        private void HandleStructureDrag(GridPosition gridPos)
        {
            // TODO: 实现拖拽绘制
        }

        private void PlaceWall(GridPosition pos)
        {
            if (State.CurrentGrid?.Grid.SetCellType(pos, GridCellType.Wall) == true)
            {
                LogKit.Log($"[GridEditor] 放置墙壁: {pos}");
            }
        }

        private void EraseWall(GridPosition pos)
        {
            if (State.CurrentGrid?.Grid.SetCellType(pos, GridCellType.Empty) == true)
            {
                LogKit.Log($"[GridEditor] 擦除墙壁: {pos}");
            }
        }

        private void PlaceFloor(GridPosition pos)
        {
            if (State.CurrentGrid?.Grid.SetCellType(pos, GridCellType.Floor) == true)
            {
                LogKit.Log($"[GridEditor] 放置地板: {pos}");
            }
        }

        #endregion

        #region 物体编辑

        private void HandleObjectClick(GridPosition gridPos, int button)
        {
            if (button == 0) // 左键
            {
                switch (State.CurrentObjectTool)
                {
                    case ObjectToolType.Place:
                        PlaceObject(gridPos);
                        break;
                    case ObjectToolType.Select:
                        SelectObject(gridPos);
                        break;
                    case ObjectToolType.Delete:
                        DeleteObjectAt(gridPos);
                        break;
                }
            }
        }

        private void HandleObjectDrag(GridPosition gridPos)
        {
            if (State.CurrentObjectTool == ObjectToolType.Move && State.HasSelection())
            {
                // TODO: 实现物体移动
            }
        }

        private void PlaceObject(GridPosition pos)
        {
            if (string.IsNullOrEmpty(State.CurrentObjectPrefabId))
            {
                LogKit.Warning("[GridEditor] 未选择物体预制体");
                return;
            }

            // TODO: 从预制体获取实际尺寸
            var size = State.PreviewSize;
            if (size == Vector3Int.zero)
            {
                size = Vector3Int.one;
            }

            string objectId = $"obj_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
            if (State.CurrentGrid.PlaceObject(objectId, pos, size))
            {
                LogKit.Log($"[GridEditor] 放置物体: {objectId} at {pos}");
            }
        }

        private void SelectObject(GridPosition pos)
        {
            var cell = State.CurrentGrid?.Grid.GetCell(pos);
            if (cell != null && cell.IsOccupied)
            {
                State.SetSelection(cell.OccupyingObjectId);
            }
            else
            {
                State.ClearSelection();
            }
        }

        private void DeleteObjectAt(GridPosition pos)
        {
            var cell = State.CurrentGrid?.Grid.GetCell(pos);
            if (cell != null && cell.IsOccupied)
            {
                State.CurrentGrid.RemoveObject(cell.OccupyingObjectId);
                LogKit.Log($"[GridEditor] 删除物体: {cell.OccupyingObjectId}");
            }
        }

        #endregion

        #region 通用操作

        /// <summary>
        /// 删除选中对象
        /// </summary>
        private void DeleteSelected()
        {
            if (State.HasSelection() && State.CurrentGrid != null)
            {
                State.CurrentGrid.RemoveObject(State.SelectedObjectId);
                State.ClearSelection();
            }
        }

        /// <summary>
        /// 取消当前操作
        /// </summary>
        private void CancelOperation()
        {
            State.ClearSelection();
            State.ClearTemporaryState();
        }

        /// <summary>
        /// 旋转物体
        /// </summary>
        private void RotateObject()
        {
            State.ObjectRotation += 90f;
            if (State.ObjectRotation >= 360f)
            {
                State.ObjectRotation = 0f;
            }
            LogKit.Log($"[GridEditor] 旋转: {State.ObjectRotation}°");
        }

        /// <summary>
        /// 设置编辑层级
        /// </summary>
        private void SetLayer(int layer)
        {
            if (State.CurrentGrid != null)
            {
                int maxLayer = State.CurrentGrid.GridDef.GridSize.y - 1;
                State.CurrentLayer = Mathf.Clamp(layer, 0, maxLayer);
                LogKit.Log($"[GridEditor] 切换层级: {State.CurrentLayer}");
            }
        }

        /// <summary>
        /// 更新鼠标位置
        /// </summary>
        private void UpdateMousePosition()
        {
            // 已在HandleMouseMove中更新
        }

        /// <summary>
        /// 更新预览
        /// </summary>
        private void UpdatePreview(GridPosition pos)
        {
            if (State.CurrentMode == EditorMode.Object && 
                State.CurrentObjectTool == ObjectToolType.Place)
            {
                State.PreviewPosition = pos;
                State.ShowPreview = true;
            }
            else
            {
                State.ShowPreview = false;
            }
        }

        #endregion

        #region 公共接口

        /// <summary>
        /// 设置网格管理器
        /// </summary>
        public void SetGridManager(GridManager manager)
        {
            GridManager = manager;
        }

        /// <summary>
        /// 设置当前网格
        /// </summary>
        public void SetCurrentGrid(GridData grid)
        {
            State.SetCurrentGrid(grid);
        }

        /// <summary>
        /// 启用/禁用编辑器
        /// </summary>
        public void SetEnabled(bool enabled)
        {
            Input.InputEnabled = enabled;
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmos()
        {
            if (State == null || State.CurrentGrid == null || !State.ShowGrid)
                return;

            // 绘制网格
            GridUtils.DrawGridGizmos(
                State.CurrentGrid.Grid,
                new Color(0.2f, 0.8f, 0.2f, 0.3f),
                new Color(0.5f, 0.5f, 0.5f, 0.5f),
                new Color(1f, 0.5f, 0f, 0.7f)
            );

            // 绘制预览
            if (State.ShowPreview)
            {
                DrawPreview();
            }
        }

        private void DrawPreview()
        {
            var pos = State.PreviewPosition;
            var size = State.PreviewSize;
            if (size == Vector3Int.zero)
            {
                size = Vector3Int.one;
            }

            var worldPos = State.CurrentGrid.Grid.GridToWorld(pos);
            var worldSize = new Vector3(
                size.x * State.CurrentGrid.GridDef.CellSize,
                size.y * State.CurrentGrid.GridDef.CellSize,
                size.z * State.CurrentGrid.GridDef.CellSize
            );

            Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
            Gizmos.DrawCube(worldPos, worldSize * 0.9f);
        }

        #endregion
    }
}