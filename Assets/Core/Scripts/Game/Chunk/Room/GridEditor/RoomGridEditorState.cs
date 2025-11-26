using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Room.Grid.Editor
{
    /// <summary>
    /// 编辑器状态机
    /// 管理编辑器的所有状态和模式切换
    /// </summary>
    [Serializable]
    public class RoomGridEditorState
    {
        #region 事件

        /// <summary>
        /// 编辑模式改变事件
        /// </summary>
        public event Action<EditorMode, EditorMode> OnModeChanged;

        /// <summary>
        /// 地块编辑工具改变事件
        /// </summary>
        public event Action<TileEditTool, TileEditTool> OnTileToolChanged;

        /// <summary>
        /// 选中地块类型改变事件
        /// </summary>
        public event Action<TileType> OnSelectedTileTypeChanged;

        /// <summary>
        /// 选中物品改变事件
        /// </summary>
        public event Action<string> OnSelectedObjectChanged;

        /// <summary>
        /// 画笔大小改变事件
        /// </summary>
        public event Action<int> OnBrushSizeChanged;

        /// <summary>
        /// 当前楼层改变事件
        /// </summary>
        public event Action<int> OnCurrentFloorChanged;

        #endregion

        #region 当前状态

        [Title("编辑模式")]
        
        [LabelText("当前模式")]
        [ReadOnly]
        [ShowInInspector]
        public EditorMode CurrentMode { get; private set; } = EditorMode.View;

        [LabelText("地块编辑工具")]
        [ReadOnly]
        [ShowInInspector]
        public TileEditTool CurrentTileTool { get; private set; } = TileEditTool.Brush;

        [Title("选中项")]
        
        [LabelText("选中的地块类型")]
        [ShowInInspector]
        public TileType SelectedTileType { get; private set; } = TileType.Grass;

        [LabelText("选中的物品定义ID")]
        [ShowInInspector]
        public string SelectedObjectDefId { get; private set; }

        [LabelText("物品旋转")]
        [ShowInInspector]
        public ObjectRotation CurrentRotation { get; private set; } = ObjectRotation.Deg0;

        [Title("画笔设置")]
        
        [LabelText("画笔大小")]
        [Range(1, 10)]
        [ShowInInspector]
        public int BrushSize { get; private set; } = 1;

        [LabelText("高度等级")]
        [Range(0, 10)]
        [ShowInInspector]
        public int HeightLevel { get; private set; } = 0;

        [Title("楼层")]
        
        [LabelText("当前楼层")]
        [ShowInInspector]
        public int CurrentFloor { get; private set; } = 0;

        #endregion

        #region 临时状态

        [Title("操作状态")]
        
        [LabelText("是否正在操作")]
        [ReadOnly]
        [ShowInInspector]
        public bool IsOperating { get; set; }

        [LabelText("拖拽起始位置")]
        [ReadOnly]
        [ShowInInspector]
        public TilePosition DragStartPosition { get; set; }

        [LabelText("当前鼠标位置(地块)")]
        [ReadOnly]
        [ShowInInspector]
        public TilePosition CurrentMouseTilePosition { get; set; }

        [LabelText("当前鼠标位置(世界)")]
        [ReadOnly]
        [ShowInInspector]
        public Vector3 CurrentMouseWorldPosition { get; set; }

        [LabelText("鼠标是否在有效区域")]
        [ReadOnly]
        [ShowInInspector]
        public bool IsMouseInValidArea { get; set; }

        [LabelText("当前选中的物品实例ID")]
        [ReadOnly]
        [ShowInInspector]
        public string SelectedObjectInstanceId { get; set; }

        [LabelText("是否显示预览")]
        [ReadOnly]
        [ShowInInspector]
        public bool ShowPreview { get; set; }

        [LabelText("预览是否有效(可放置)")]
        [ReadOnly]
        [ShowInInspector]
        public bool IsPreviewValid { get; set; }

        #endregion

        #region 模式切换

        /// <summary>
        /// 设置编辑模式
        /// </summary>
        public void SetMode(EditorMode newMode)
        {
            if (CurrentMode == newMode) return;

            var oldMode = CurrentMode;
            CurrentMode = newMode;

            // 切换模式时清理状态
            ClearOperatingState();

            // 根据模式设置预览
            UpdatePreviewState();

            OnModeChanged?.Invoke(oldMode, newMode);
            Debug.Log($"[EditorState] 模式切换: {oldMode} -> {newMode}");
        }

        /// <summary>
        /// 设置地块编辑工具
        /// </summary>
        public void SetTileTool(TileEditTool tool)
        {
            if (CurrentTileTool == tool) return;

            var oldTool = CurrentTileTool;
            CurrentTileTool = tool;

            // 自动切换到地块编辑模式
            if (CurrentMode != EditorMode.TileEdit)
            {
                SetMode(EditorMode.TileEdit);
            }

            ClearOperatingState();
            OnTileToolChanged?.Invoke(oldTool, tool);
            Debug.Log($"[EditorState] 地块工具切换: {oldTool} -> {tool}");
        }

        /// <summary>
        /// 进入物品放置模式
        /// </summary>
        public void EnterObjectPlaceMode(string objectDefId)
        {
            if (string.IsNullOrEmpty(objectDefId)) return;

            SelectedObjectDefId = objectDefId;
            SetMode(EditorMode.ObjectPlace);
            OnSelectedObjectChanged?.Invoke(objectDefId);
        }

        /// <summary>
        /// 进入物品选择模式
        /// </summary>
        public void EnterObjectSelectMode()
        {
            SetMode(EditorMode.ObjectSelect);
        }

        /// <summary>
        /// 进入删除模式
        /// </summary>
        public void EnterDeleteMode()
        {
            SetMode(EditorMode.Delete);
        }

        /// <summary>
        /// 返回查看模式
        /// </summary>
        public void ExitToViewMode()
        {
            SetMode(EditorMode.View);
        }

        #endregion

        #region 选择设置

        /// <summary>
        /// 设置选中的地块类型
        /// </summary>
        public void SetSelectedTileType(TileType type)
        {
            if (SelectedTileType == type) return;

            SelectedTileType = type;
            OnSelectedTileTypeChanged?.Invoke(type);
        }

        /// <summary>
        /// 设置画笔大小
        /// </summary>
        public void SetBrushSize(int size)
        {
            size = Mathf.Clamp(size, 1, 10);
            if (BrushSize == size) return;

            BrushSize = size;
            OnBrushSizeChanged?.Invoke(size);
        }

        /// <summary>
        /// 增加画笔大小
        /// </summary>
        public void IncreaseBrushSize()
        {
            SetBrushSize(BrushSize + 1);
        }

        /// <summary>
        /// 减小画笔大小
        /// </summary>
        public void DecreaseBrushSize()
        {
            SetBrushSize(BrushSize - 1);
        }

        /// <summary>
        /// 设置高度等级
        /// </summary>
        public void SetHeightLevel(int level)
        {
            HeightLevel = Mathf.Clamp(level, 0, 10);
        }

        /// <summary>
        /// 设置当前楼层
        /// </summary>
        public void SetCurrentFloor(int floor)
        {
            if (CurrentFloor == floor) return;

            CurrentFloor = floor;
            OnCurrentFloorChanged?.Invoke(floor);
        }

        /// <summary>
        /// 旋转物品（顺时针）
        /// </summary>
        public void RotateObject(bool clockwise = true)
        {
            int deg = (int)CurrentRotation;
            deg += clockwise ? 90 : -90;
            
            if (deg >= 360) deg -= 360;
            if (deg < 0) deg += 360;
            
            CurrentRotation = (ObjectRotation)deg;
            Debug.Log($"[EditorState] 旋转: {CurrentRotation}");
        }

        /// <summary>
        /// 选中物品实例
        /// </summary>
        public void SelectObjectInstance(string instanceId)
        {
            SelectedObjectInstanceId = instanceId;
        }

        /// <summary>
        /// 取消选中物品
        /// </summary>
        public void DeselectObject()
        {
            SelectedObjectInstanceId = null;
        }

        /// <summary>
        /// 是否有选中的物品实例
        /// </summary>
        public bool HasSelectedObject()
        {
            return !string.IsNullOrEmpty(SelectedObjectInstanceId);
        }

        #endregion

        #region 状态管理

        /// <summary>
        /// 更新鼠标位置
        /// </summary>
        public void UpdateMousePosition(Vector3 worldPos, TilePosition tilePos, bool isValid)
        {
            CurrentMouseWorldPosition = worldPos;
            CurrentMouseTilePosition = tilePos;
            IsMouseInValidArea = isValid;
        }

        /// <summary>
        /// 开始操作
        /// </summary>
        public void BeginOperation(TilePosition startPos)
        {
            IsOperating = true;
            DragStartPosition = startPos;
        }

        /// <summary>
        /// 结束操作
        /// </summary>
        public void EndOperation()
        {
            IsOperating = false;
        }

        /// <summary>
        /// 清除操作状态
        /// </summary>
        public void ClearOperatingState()
        {
            IsOperating = false;
            DragStartPosition = TilePosition.Zero;
            SelectedObjectInstanceId = null;
        }

        /// <summary>
        /// 更新预览状态
        /// </summary>
        private void UpdatePreviewState()
        {
            switch (CurrentMode)
            {
                case EditorMode.TileEdit:
                    ShowPreview = true;
                    break;
                case EditorMode.ObjectPlace:
                    ShowPreview = true;
                    break;
                default:
                    ShowPreview = false;
                    break;
            }
        }

        /// <summary>
        /// 重置所有状态
        /// </summary>
        public void Reset()
        {
            CurrentMode = EditorMode.View;
            CurrentTileTool = TileEditTool.Brush;
            SelectedTileType = TileType.Grass;
            SelectedObjectDefId = null;
            CurrentRotation = ObjectRotation.Deg0;
            BrushSize = 1;
            HeightLevel = 0;
            CurrentFloor = 0;
            
            ClearOperatingState();
            ShowPreview = false;
            IsPreviewValid = false;
        }

        /// <summary>
        /// 获取状态摘要
        /// </summary>
        public string GetStatusSummary()
        {
            string modeInfo = $"Mode: {CurrentMode}";
            string toolInfo = CurrentMode == EditorMode.TileEdit ? $" Tool: {CurrentTileTool}" : "";
            string tileInfo = CurrentMode == EditorMode.TileEdit ? $" Type: {SelectedTileType}" : "";
            string objInfo = CurrentMode == EditorMode.ObjectPlace ? $" Object: {SelectedObjectDefId}" : "";
            string floorInfo = $" Floor: {CurrentFloor + 1}F";

            return $"{modeInfo}{toolInfo}{tileInfo}{objInfo}{floorInfo}";
        }

        #endregion
    }
}