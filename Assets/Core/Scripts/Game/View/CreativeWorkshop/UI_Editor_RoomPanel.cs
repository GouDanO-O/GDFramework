using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using GDFrameworkExtend.UIKit;
using Core.Game.Chunk.Room.Grid;
using Core.Game.Chunk.Room.Grid.Editor;

namespace Core.Game.View
{
    /// <summary>
    /// 房间编辑器面板数据
    /// </summary>
    public class UI_Editor_RoomPanelData : UIPanelData
    {
        /// <summary>
        /// 编辑器引用（如果从外部传入）
        /// </summary>
        public RoomGridEditor Editor;

        /// <summary>
        /// 预设的网格配置
        /// </summary>
        public RoomGridConfig PresetConfig;

        /// <summary>
        /// 要加载的存档名称
        /// </summary>
        public string LoadSaveName;
    }

    /// <summary>
    /// 房间编辑器UI面板
    /// 提供完整的房间编辑功能界面
    /// </summary>
    public partial class UI_Editor_RoomPanel : UIPanel
    {
        #region 事件

        /// <summary>
        /// 编辑器初始化完成事件
        /// </summary>
        public event UnityAction OnEditorReady;

        /// <summary>
        /// 模式改变事件
        /// </summary>
        public event UnityAction<EditorMode> OnModeChanged;

        /// <summary>
        /// 保存请求事件
        /// </summary>
        public event UnityAction<string> OnSaveRequested;

        /// <summary>
        /// 关闭编辑器事件
        /// </summary>
        public event UnityAction OnEditorClosed;

        #endregion

        #region 属性

        /// <summary>
        /// 房间编辑器
        /// </summary>
        public RoomGridEditor Editor { get; private set; }

        /// <summary>
        /// 编辑器状态
        /// </summary>
        public RoomGridEditorState EditorState => Editor?.State;

        /// <summary>
        /// 房间网格
        /// </summary>
        public RoomGrid Grid => Editor?.Grid;

        /// <summary>
        /// 是否已初始化
        /// </summary>
        public bool IsReady => Editor != null && Editor.IsInitialized;

        /// <summary>
        /// 当前编辑模式
        /// </summary>
        public EditorMode CurrentMode => EditorState?.CurrentMode ?? EditorMode.None;

        #endregion

        #region UI引用（运行时创建或绑定）

        // 工具栏
        private RoomEditorToolbar _toolbar;
        private RoomEditorObjectPalette _objectPalette;
        private RoomEditorStatusBar _statusBar;
        private RoomEditorTilePalette _tilePalette;

        // 弹窗
        private RoomEditorSaveDialog _saveDialog;
        private RoomEditorLoadDialog _loadDialog;
        private RoomEditorConfigDialog _configDialog;

        #endregion

        #region 私有字段

        private ObjectDefinitionManager _objectDefManager;
        private RoomGridSaveSystem _saveSystem;
        private string _currentSaveName;
        private bool _hasUnsavedChanges;
        private float _autoSaveTimer;

        #endregion

        #region 生命周期

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UI_Editor_RoomPanelData ?? new UI_Editor_RoomPanelData();

            // 获取系统引用
            _objectDefManager = ObjectDefinitionManager.Instance;
            _saveSystem = RoomGridSaveSystem.Instance;

            // 加载默认物品定义（如果未加载）
            if (!_objectDefManager.IsInitialized)
            {
                _objectDefManager.LoadDefaultTestData();
            }

            // 初始化UI组件
            InitializeUIComponents();
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            var data = uiData as UI_Editor_RoomPanelData ?? mData as UI_Editor_RoomPanelData;

            if (data != null)
            {
                // 如果传入了编辑器引用，直接使用
                if (data.Editor != null)
                {
                    AttachToEditor(data.Editor);
                }
                // 如果有存档名称，加载存档
                else if (!string.IsNullOrEmpty(data.LoadSaveName))
                {
                    LoadFromSave(data.LoadSaveName);
                }
                // 否则使用预设配置创建新的
                else
                {
                    CreateNewRoom(data.PresetConfig);
                }
            }
        }

        protected override void OnShow()
        {
            // 订阅输入事件
            SubscribeEvents();

            // 更新UI状态
            RefreshUI();
        }

        protected override void OnHide()
        {
            // 取消订阅
            UnsubscribeEvents();
        }

        protected override void OnClose()
        {
            // 检查未保存的更改
            if (_hasUnsavedChanges)
            {
                // 可以在这里弹出确认对话框
                Debug.LogWarning("[RoomEditorPanel] 关闭时存在未保存的更改");
            }

            OnEditorClosed?.Invoke();
            CleanupEditor();
        }

        private void Update()
        {
            if (!IsReady) return;

            // 处理快捷键
            HandleHotkeys();

            // 自动保存计时
            if (_saveSystem.AutoSaveEnabled && _hasUnsavedChanges)
            {
                _autoSaveTimer += Time.deltaTime;
                if (_autoSaveTimer >= _saveSystem.AutoSaveInterval)
                {
                    AutoSave();
                    _autoSaveTimer = 0f;
                }
            }
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化UI组件
        /// </summary>
        private void InitializeUIComponents()
        {
            // 创建工具栏
            _toolbar = CreateToolbar();

            // 创建地块面板
            _tilePalette = CreateTilePalette();

            // 创建物品面板
            _objectPalette = CreateObjectPalette();

            // 创建状态栏
            _statusBar = CreateStatusBar();

            Debug.Log("[RoomEditorPanel] UI组件初始化完成");
        }

        /// <summary>
        /// 绑定到已存在的编辑器
        /// </summary>
        public void AttachToEditor(RoomGridEditor editor)
        {
            if (editor == null)
            {
                Debug.LogError("[RoomEditorPanel] Editor为空");
                return;
            }

            Editor = editor;

            // 订阅编辑器事件
            SubscribeEditorEvents();

            // 刷新UI
            RefreshUI();

            OnEditorReady?.Invoke();
            Debug.Log("[RoomEditorPanel] 已绑定到编辑器");
        }

        /// <summary>
        /// 创建新房间
        /// </summary>
        public void CreateNewRoom(RoomGridConfig config = null)
        {
            config ??= new RoomGridConfig();

            // 查找或创建编辑器GameObject
            var editorGO = FindOrCreateEditorGameObject();
            Editor = editorGO.GetComponent<RoomGridEditor>();

            if (Editor == null)
            {
                Editor = editorGO.AddComponent<RoomGridEditor>();
            }

            // 初始化编辑器
            Editor.Initialize(config);

            // 订阅事件
            SubscribeEditorEvents();

            // 标记为新建
            _currentSaveName = null;
            _hasUnsavedChanges = false;

            RefreshUI();
            OnEditorReady?.Invoke();

            Debug.Log($"[RoomEditorPanel] 创建新房间: {config}");
        }

        /// <summary>
        /// 从存档加载
        /// </summary>
        public void LoadFromSave(string saveName)
        {
            var saveData = _saveSystem.Load(saveName);
            if (saveData == null)
            {
                Debug.LogError($"[RoomEditorPanel] 加载存档失败: {saveName}");
                return;
            }

            var grid = _saveSystem.RestoreGrid(saveData);
            if (grid == null)
            {
                Debug.LogError($"[RoomEditorPanel] 恢复网格失败: {saveName}");
                return;
            }

            // 查找或创建编辑器
            var editorGO = FindOrCreateEditorGameObject();
            Editor = editorGO.GetComponent<RoomGridEditor>();

            if (Editor == null)
            {
                Editor = editorGO.AddComponent<RoomGridEditor>();
            }

            // 从网格初始化
            Editor.InitializeWithGrid(grid);

            // 订阅事件
            SubscribeEditorEvents();

            _currentSaveName = saveName;
            _hasUnsavedChanges = false;

            RefreshUI();
            OnEditorReady?.Invoke();

            Debug.Log($"[RoomEditorPanel] 加载存档成功: {saveName}");
        }

        /// <summary>
        /// 查找或创建编辑器GameObject
        /// </summary>
        private GameObject FindOrCreateEditorGameObject()
        {
            // 首先查找现有的
            var existing = GameObject.Find("RoomGridEditor");
            if (existing != null)
            {
                return existing;
            }

            // 创建新的
            var editorGO = new GameObject("RoomGridEditor");
            return editorGO;
        }

        /// <summary>
        /// 清理编辑器
        /// </summary>
        private void CleanupEditor()
        {
            UnsubscribeEditorEvents();
            Editor = null;
        }

        #endregion

        #region 事件订阅

        private void SubscribeEvents()
        {
            // 订阅工具栏事件
            if (_toolbar != null)
            {
                _toolbar.OnModeSelected += HandleModeSelected;
                _toolbar.OnToolSelected += HandleToolSelected;
                _toolbar.OnSaveClicked += HandleSaveClicked;
                _toolbar.OnLoadClicked += HandleLoadClicked;
                _toolbar.OnNewClicked += HandleNewClicked;
                _toolbar.OnSettingsClicked += HandleSettingsClicked;
            }

            // 订阅地块面板事件
            if (_tilePalette != null)
            {
                _tilePalette.OnTileTypeSelected += HandleTileTypeSelected;
                _tilePalette.OnBrushSizeChanged += HandleBrushSizeChanged;
                _tilePalette.OnHeightLevelChanged += HandleHeightLevelChanged;
            }

            // 订阅物品面板事件
            if (_objectPalette != null)
            {
                _objectPalette.OnObjectSelected += HandleObjectSelected;
                _objectPalette.OnCategoryChanged += HandleCategoryChanged;
            }
        }

        private void UnsubscribeEvents()
        {
            if (_toolbar != null)
            {
                _toolbar.OnModeSelected -= HandleModeSelected;
                _toolbar.OnToolSelected -= HandleToolSelected;
                _toolbar.OnSaveClicked -= HandleSaveClicked;
                _toolbar.OnLoadClicked -= HandleLoadClicked;
                _toolbar.OnNewClicked -= HandleNewClicked;
                _toolbar.OnSettingsClicked -= HandleSettingsClicked;
            }

            if (_tilePalette != null)
            {
                _tilePalette.OnTileTypeSelected -= HandleTileTypeSelected;
                _tilePalette.OnBrushSizeChanged -= HandleBrushSizeChanged;
                _tilePalette.OnHeightLevelChanged -= HandleHeightLevelChanged;
            }

            if (_objectPalette != null)
            {
                _objectPalette.OnObjectSelected -= HandleObjectSelected;
                _objectPalette.OnCategoryChanged -= HandleCategoryChanged;
            }
        }

        private void SubscribeEditorEvents()
        {
            if (Editor == null) return;

            Editor.OnEditorInitialized += HandleEditorInitialized;
            Editor.OnTileModified += HandleTileModified;
            Editor.OnObjectPlaced += HandleObjectPlaced;
            Editor.OnObjectRemoved += HandleObjectRemoved;

            if (EditorState != null)
            {
                EditorState.OnModeChanged += HandleEditorModeChanged;
                EditorState.OnTileToolChanged += HandleTileToolChanged;
                EditorState.OnSelectedTileTypeChanged += HandleSelectedTileTypeChanged;
                EditorState.OnBrushSizeChanged += HandleBrushSizeChanged_Editor;
                EditorState.OnCurrentFloorChanged += HandleFloorChanged;
            }
        }

        private void UnsubscribeEditorEvents()
        {
            if (Editor == null) return;

            Editor.OnEditorInitialized -= HandleEditorInitialized;
            Editor.OnTileModified -= HandleTileModified;
            Editor.OnObjectPlaced -= HandleObjectPlaced;
            Editor.OnObjectRemoved -= HandleObjectRemoved;

            if (EditorState != null)
            {
                EditorState.OnModeChanged -= HandleEditorModeChanged;
                EditorState.OnTileToolChanged -= HandleTileToolChanged;
                EditorState.OnSelectedTileTypeChanged -= HandleSelectedTileTypeChanged;
                EditorState.OnBrushSizeChanged -= HandleBrushSizeChanged_Editor;
                EditorState.OnCurrentFloorChanged -= HandleFloorChanged;
            }
        }

        #endregion

        #region UI事件处理

        private void HandleModeSelected(EditorMode mode)
        {
            if (!IsReady) return;

            Editor.SetMode(mode);
            UpdatePanelVisibility(mode);
        }

        private void HandleToolSelected(TileEditTool tool)
        {
            if (!IsReady) return;

            Editor.SetTileTool(tool);
        }

        private void HandleTileTypeSelected(TileType type)
        {
            if (!IsReady) return;

            Editor.SetTileType(type);
        }

        private void HandleBrushSizeChanged(int size)
        {
            if (!IsReady) return;

            EditorState?.SetBrushSize(size);
        }

        private void HandleHeightLevelChanged(int level)
        {
            if (!IsReady) return;

            EditorState?.SetHeightLevel(level);
        }

        private void HandleObjectSelected(string objectDefId)
        {
            if (!IsReady) return;

            Editor.StartPlaceObject(objectDefId);
        }

        private void HandleCategoryChanged(ObjectCategory category)
        {
            // 刷新物品列表
            _objectPalette?.RefreshObjectList(category);
        }

        private void HandleSaveClicked()
        {
            ShowSaveDialog();
        }

        private void HandleLoadClicked()
        {
            ShowLoadDialog();
        }

        private void HandleNewClicked()
        {
            ShowNewRoomDialog();
        }

        private void HandleSettingsClicked()
        {
            ShowSettingsDialog();
        }

        #endregion

        #region 编辑器事件处理

        private void HandleEditorInitialized()
        {
            RefreshUI();
        }

        private void HandleTileModified(TilePosition pos, TileData tile)
        {
            _hasUnsavedChanges = true;
            _statusBar?.UpdateStatus($"修改地块: {pos}");
        }

        private void HandleObjectPlaced(PlacedObjectData obj)
        {
            _hasUnsavedChanges = true;
            var def = _objectDefManager.GetDefinition(obj.ObjectDefId);
            _statusBar?.UpdateStatus($"放置物品: {def?.Name ?? obj.ObjectDefId}");
        }

        private void HandleObjectRemoved(PlacedObjectData obj)
        {
            _hasUnsavedChanges = true;
            _statusBar?.UpdateStatus($"移除物品");
        }

        private void HandleEditorModeChanged(EditorMode oldMode, EditorMode newMode)
        {
            _toolbar?.SetActiveMode(newMode);
            UpdatePanelVisibility(newMode);
            OnModeChanged?.Invoke(newMode);
        }

        private void HandleTileToolChanged(TileEditTool oldTool, TileEditTool newTool)
        {
            _toolbar?.SetActiveTool(newTool);
        }

        private void HandleSelectedTileTypeChanged(TileType type)
        {
            _tilePalette?.SetSelectedTileType(type);
        }

        private void HandleBrushSizeChanged_Editor(int size)
        {
            _tilePalette?.SetBrushSize(size);
        }

        private void HandleFloorChanged(int floor)
        {
            _statusBar?.UpdateFloor(floor);
        }

        #endregion

        #region 快捷键处理

        private void HandleHotkeys()
        {
            // Ctrl + S 保存
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                && Input.GetKeyDown(KeyCode.S))
            {
                QuickSave();
            }

            // Ctrl + N 新建
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                && Input.GetKeyDown(KeyCode.N))
            {
                ShowNewRoomDialog();
            }

            // Ctrl + O 打开
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                && Input.GetKeyDown(KeyCode.O))
            {
                ShowLoadDialog();
            }

            // 数字键切换模式
            if (Input.GetKeyDown(KeyCode.Alpha1)) SetMode(EditorMode.View);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SetMode(EditorMode.TileEdit);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SetMode(EditorMode.ObjectPlace);
            if (Input.GetKeyDown(KeyCode.Alpha4)) SetMode(EditorMode.ObjectSelect);
            if (Input.GetKeyDown(KeyCode.Alpha5)) SetMode(EditorMode.Delete);

            // Q/W/E/R 切换工具（在地块编辑模式下）
            if (CurrentMode == EditorMode.TileEdit)
            {
                if (Input.GetKeyDown(KeyCode.Q)) SetTool(TileEditTool.Brush);
                if (Input.GetKeyDown(KeyCode.W)) SetTool(TileEditTool.Fill);
                if (Input.GetKeyDown(KeyCode.E)) SetTool(TileEditTool.Rectangle);
                //if (Input.GetKeyDown(KeyCode.R)) SetTool(TileEditTool.Eraser); // R用于旋转
            }

            // Page Up/Down 切换楼层
            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                Grid?.GoUpFloor();
            }
            if (Input.GetKeyDown(KeyCode.PageDown))
            {
                Grid?.GoDownFloor();
            }
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 设置编辑模式
        /// </summary>
        public void SetMode(EditorMode mode)
        {
            Editor?.SetMode(mode);
        }

        /// <summary>
        /// 设置地块编辑工具
        /// </summary>
        public void SetTool(TileEditTool tool)
        {
            Editor?.SetTileTool(tool);
        }

        /// <summary>
        /// 设置地块类型
        /// </summary>
        public void SetTileType(TileType type)
        {
            Editor?.SetTileType(type);
        }

        /// <summary>
        /// 开始放置物品
        /// </summary>
        public void StartPlaceObject(string objectDefId)
        {
            Editor?.StartPlaceObject(objectDefId);
        }

        /// <summary>
        /// 保存当前房间
        /// </summary>
        public bool Save(string saveName = null)
        {
            if (!IsReady) return false;

            saveName ??= _currentSaveName ?? $"Room_{DateTime.Now:yyyyMMdd_HHmmss}";

            bool success = _saveSystem.Save(Grid, saveName);
            if (success)
            {
                _currentSaveName = saveName;
                _hasUnsavedChanges = false;
                _statusBar?.UpdateStatus($"已保存: {saveName}");
                OnSaveRequested?.Invoke(saveName);
            }

            return success;
        }

        /// <summary>
        /// 快速保存
        /// </summary>
        public bool QuickSave()
        {
            if (!string.IsNullOrEmpty(_currentSaveName))
            {
                return Save(_currentSaveName);
            }
            else
            {
                ShowSaveDialog();
                return false;
            }
        }

        /// <summary>
        /// 自动保存
        /// </summary>
        private bool AutoSave()
        {
            if (!IsReady) return false;

            bool success = _saveSystem.AutoSave(Grid);
            if (success)
            {
                _statusBar?.UpdateStatus("自动保存完成");
            }
            return success;
        }

        /// <summary>
        /// 获取编辑器统计信息
        /// </summary>
        public RoomGridStatistics GetStatistics()
        {
            return Editor?.GetStatistics() ?? default;
        }

        #endregion

        #region UI刷新

        /// <summary>
        /// 刷新所有UI
        /// </summary>
        public void RefreshUI()
        {
            if (!IsReady) return;

            // 刷新工具栏
            _toolbar?.SetActiveMode(CurrentMode);
            _toolbar?.SetActiveTool(EditorState?.CurrentTileTool ?? TileEditTool.Brush);

            // 刷新地块面板
            _tilePalette?.SetSelectedTileType(EditorState?.SelectedTileType ?? TileType.Grass);
            _tilePalette?.SetBrushSize(EditorState?.BrushSize ?? 1);

            // 刷新状态栏
            _statusBar?.UpdateFloor(EditorState?.CurrentFloor ?? 0);
            _statusBar?.UpdateStatistics(GetStatistics());

            // 更新面板可见性
            UpdatePanelVisibility(CurrentMode);
        }

        /// <summary>
        /// 根据模式更新面板可见性
        /// </summary>
        private void UpdatePanelVisibility(EditorMode mode)
        {
            // 地块面板在地块编辑模式下显示
            _tilePalette?.SetVisible(mode == EditorMode.TileEdit);

            // 物品面板在物品放置模式下显示
            _objectPalette?.SetVisible(mode == EditorMode.ObjectPlace);
        }

        #endregion

        #region 对话框

        private void ShowSaveDialog()
        {
            // TODO: 实现保存对话框
            // 暂时使用默认名称保存
            string saveName = _currentSaveName ?? $"Room_{DateTime.Now:yyyyMMdd_HHmmss}";
            Save(saveName);
        }

        private void ShowLoadDialog()
        {
            // TODO: 实现加载对话框
            Debug.Log("[RoomEditorPanel] 加载对话框（待实现）");
        }

        private void ShowNewRoomDialog()
        {
            // TODO: 实现新建对话框
            // 暂时使用默认配置
            if (_hasUnsavedChanges)
            {
                Debug.LogWarning("[RoomEditorPanel] 存在未保存的更改，创建新房间将丢失");
            }
            CreateNewRoom(new RoomGridConfig());
        }

        private void ShowSettingsDialog()
        {
            // TODO: 实现设置对话框
            Debug.Log("[RoomEditorPanel] 设置对话框（待实现）");
        }

        #endregion

        #region UI组件创建（运行时）

        private RoomEditorToolbar CreateToolbar()
        {
            // 在运行时创建工具栏组件
            var toolbarGO = new GameObject("Toolbar", typeof(RectTransform));
            toolbarGO.transform.SetParent(transform);

            var toolbar = toolbarGO.AddComponent<RoomEditorToolbar>();
            toolbar.Initialize();

            return toolbar;
        }

        private RoomEditorTilePalette CreateTilePalette()
        {
            var paletteGO = new GameObject("TilePalette", typeof(RectTransform));
            paletteGO.transform.SetParent(transform);

            var palette = paletteGO.AddComponent<RoomEditorTilePalette>();
            palette.Initialize();

            return palette;
        }

        private RoomEditorObjectPalette CreateObjectPalette()
        {
            var paletteGO = new GameObject("ObjectPalette", typeof(RectTransform));
            paletteGO.transform.SetParent(transform);

            var palette = paletteGO.AddComponent<RoomEditorObjectPalette>();
            palette.Initialize(_objectDefManager);

            return palette;
        }

        private RoomEditorStatusBar CreateStatusBar()
        {
            var statusBarGO = new GameObject("StatusBar", typeof(RectTransform));
            statusBarGO.transform.SetParent(transform);

            var statusBar = statusBarGO.AddComponent<RoomEditorStatusBar>();
            statusBar.Initialize();

            return statusBar;
        }

        #endregion
    }
}
