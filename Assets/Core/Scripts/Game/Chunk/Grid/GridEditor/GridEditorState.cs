using System;
using Core.Game.Grid.Data;
using UnityEngine;

namespace Core.Game.Grid.Editor
{
    /// <summary>
    /// 编辑器状态
    /// 管理当前编辑模式、工具、选中对象等
    /// </summary>
    public class GridEditorState
    {
        #region 事件

        /// <summary>
        /// 编辑模式改变事件
        /// </summary>
        public event Action<EditorMode, EditorMode> OnModeChanged;

        /// <summary>
        /// 工具改变事件
        /// </summary>
        public event Action<object, object> OnToolChanged;

        /// <summary>
        /// 选中对象改变事件
        /// </summary>
        public event Action<string> OnSelectionChanged;

        /// <summary>
        /// 网格改变事件
        /// </summary>
        public event Action<GridData> OnGridChanged;

        #endregion

        #region 当前状态

        /// <summary>
        /// 当前编辑模式
        /// </summary>
        public EditorMode CurrentMode { get; private set; }

        /// <summary>
        /// 当前结构工具
        /// </summary>
        public StructureToolType CurrentStructureTool { get; private set; }

        /// <summary>
        /// 当前物体工具
        /// </summary>
        public ObjectToolType CurrentObjectTool { get; private set; }

        /// <summary>
        /// 当前网格
        /// </summary>
        public GridData CurrentGrid { get; private set; }

        /// <summary>
        /// 当前选中的对象ID
        /// </summary>
        public string SelectedObjectId { get; private set; }

        /// <summary>
        /// 当前绘制模式
        /// </summary>
        public DrawMode CurrentDrawMode { get; set; }

        /// <summary>
        /// 当前捕捉模式
        /// </summary>
        public SnapMode CurrentSnapMode { get; set; }

        /// <summary>
        /// 当前视图模式
        /// </summary>
        public ViewMode CurrentViewMode { get; set; }

        /// <summary>
        /// 是否启用网格显示
        /// </summary>
        public bool ShowGrid { get; set; }

        /// <summary>
        /// 是否显示辅助线
        /// </summary>
        public bool ShowGuides { get; set; }

        /// <summary>
        /// 当前编辑层级
        /// </summary>
        public int CurrentLayer { get; set; }

        #endregion

        #region 编辑参数

        /// <summary>
        /// 画刷大小
        /// </summary>
        public int BrushSize { get; set; }

        /// <summary>
        /// 当前选择的单元格类型(用于绘制)
        /// </summary>
        public GridCellType CurrentCellType { get; set; }

        /// <summary>
        /// 当前物体预制体ID
        /// </summary>
        public string CurrentObjectPrefabId { get; set; }

        /// <summary>
        /// 物体旋转角度
        /// </summary>
        public float ObjectRotation { get; set; }

        #endregion

        #region 临时状态

        /// <summary>
        /// 是否正在操作
        /// </summary>
        public bool IsOperating { get; set; }

        /// <summary>
        /// 拖拽起始位置
        /// </summary>
        public GridPosition DragStartPosition { get; set; }

        /// <summary>
        /// 当前鼠标位置(网格坐标)
        /// </summary>
        public GridPosition CurrentMouseGridPosition { get; set; }

        /// <summary>
        /// 预览物体位置
        /// </summary>
        public GridPosition PreviewPosition { get; set; }

        /// <summary>
        /// 预览物体大小
        /// </summary>
        public Vector3Int PreviewSize { get; set; }

        /// <summary>
        /// 是否显示预览
        /// </summary>
        public bool ShowPreview { get; set; }

        #endregion

        public GridEditorState()
        {
            // 初始化默认值
            CurrentMode = EditorMode.None;
            CurrentStructureTool = StructureToolType.None;
            CurrentObjectTool = ObjectToolType.None;
            CurrentDrawMode = DrawMode.Single;
            CurrentSnapMode = SnapMode.Grid;
            CurrentViewMode = ViewMode.Perspective;
            ShowGrid = true;
            ShowGuides = true;
            CurrentLayer = 0;
            BrushSize = 1;
            CurrentCellType = GridCellType.Floor;
            ObjectRotation = 0f;
            IsOperating = false;
            ShowPreview = false;
        }

        #region 模式切换

        /// <summary>
        /// 设置编辑模式
        /// </summary>
        public void SetMode(EditorMode newMode)
        {
            if (CurrentMode == newMode)
                return;

            var oldMode = CurrentMode;
            CurrentMode = newMode;

            // 清理状态
            ClearTemporaryState();

            // 触发事件
            OnModeChanged?.Invoke(oldMode, newMode);

            Debug.Log($"[EditorState] 切换模式: {oldMode} -> {newMode}");
        }

        /// <summary>
        /// 设置结构工具
        /// </summary>
        public void SetStructureTool(StructureToolType tool)
        {
            if (CurrentStructureTool == tool)
                return;

            var oldTool = CurrentStructureTool;
            CurrentStructureTool = tool;

            // 自动切换到结构编辑模式
            if (CurrentMode != EditorMode.Structure)
            {
                SetMode(EditorMode.Structure);
            }

            ClearTemporaryState();
            OnToolChanged?.Invoke(oldTool, tool);

            Debug.Log($"[EditorState] 切换结构工具: {oldTool} -> {tool}");
        }

        /// <summary>
        /// 设置物体工具
        /// </summary>
        public void SetObjectTool(ObjectToolType tool)
        {
            if (CurrentObjectTool == tool)
                return;

            var oldTool = CurrentObjectTool;
            CurrentObjectTool = tool;

            // 自动切换到物体编辑模式
            if (CurrentMode != EditorMode.Object)
            {
                SetMode(EditorMode.Object);
            }

            ClearTemporaryState();
            OnToolChanged?.Invoke(oldTool, tool);

            Debug.Log($"[EditorState] 切换物体工具: {oldTool} -> {tool}");
        }

        #endregion

        #region 网格管理

        /// <summary>
        /// 设置当前网格
        /// </summary>
        public void SetCurrentGrid(GridData grid)
        {
            if (CurrentGrid == grid)
                return;

            CurrentGrid = grid;
            ClearSelection();
            ClearTemporaryState();

            OnGridChanged?.Invoke(grid);

            Debug.Log($"[EditorState] 切换网格: {grid?.DefId ?? "None"}");
        }

        #endregion

        #region 选择管理

        /// <summary>
        /// 设置选中对象
        /// </summary>
        public void SetSelection(string objectId)
        {
            if (SelectedObjectId == objectId)
                return;

            SelectedObjectId = objectId;
            OnSelectionChanged?.Invoke(objectId);

            Debug.Log($"[EditorState] 选中对象: {objectId ?? "None"}");
        }

        /// <summary>
        /// 清除选择
        /// </summary>
        public void ClearSelection()
        {
            SetSelection(null);
        }

        /// <summary>
        /// 是否有选中对象
        /// </summary>
        public bool HasSelection()
        {
            return !string.IsNullOrEmpty(SelectedObjectId);
        }

        #endregion

        #region 状态管理

        /// <summary>
        /// 清除临时状态
        /// </summary>
        public void ClearTemporaryState()
        {
            IsOperating = false;
            DragStartPosition = GridPosition.Zero;
            ShowPreview = false;
        }

        /// <summary>
        /// 重置编辑器状态
        /// </summary>
        public void Reset()
        {
            SetMode(EditorMode.None);
            CurrentStructureTool = StructureToolType.None;
            CurrentObjectTool = ObjectToolType.None;
            ClearSelection();
            ClearTemporaryState();
            CurrentGrid = null;
        }

        /// <summary>
        /// 获取状态摘要
        /// </summary>
        public string GetStatusSummary()
        {
            string gridInfo = CurrentGrid != null ? $"Grid: {CurrentGrid.DefId}" : "No Grid";
            string modeInfo = $"Mode: {CurrentMode}";
            string toolInfo = CurrentMode switch
            {
                EditorMode.Structure => $"Tool: {CurrentStructureTool}",
                EditorMode.Object => $"Tool: {CurrentObjectTool}",
                _ => ""
            };
            string selectionInfo = HasSelection() ? $"Selected: {SelectedObjectId}" : "";

            return $"{gridInfo} | {modeInfo} | {toolInfo} | {selectionInfo}";
        }

        #endregion
    }
}