using System;
using System.Collections.Generic;
using GDFrameworkCore;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Core.Game.Chunk.Room.Grid.Editor.UI
{
    /// <summary>
    /// 房间编辑器UI面板
    /// 管理编辑器的所有UI
    /// </summary>
    public class RoomEditorUIPanel : MonoBehaviour
    {
        #region UI引用

        [Title("工具栏")]
        
        [LabelText("模式选择按钮组")]
        [SerializeField]
        private Transform _modeButtonGroup;

        [LabelText("地块工具按钮组")]
        [SerializeField]
        private Transform _tileToolButtonGroup;

        [LabelText("地块类型选择")]
        [SerializeField]
        private Transform _tileTypePanel;

        [LabelText("物品列表面板")]
        [SerializeField]
        private Transform _objectListPanel;

        [Title("状态栏")]
        
        [LabelText("当前模式文本")]
        [SerializeField]
        private Text _currentModeText;

        [LabelText("鼠标位置文本")]
        [SerializeField]
        private Text _mousePositionText;

        [LabelText("统计信息文本")]
        [SerializeField]
        private Text _statisticsText;

        [Title("画笔设置")]
        
        [LabelText("画笔大小滑块")]
        [SerializeField]
        private Slider _brushSizeSlider;

        [LabelText("画笔大小文本")]
        [SerializeField]
        private Text _brushSizeText;

        [LabelText("高度等级滑块")]
        [SerializeField]
        private Slider _heightLevelSlider;

        [LabelText("高度等级文本")]
        [SerializeField]
        private Text _heightLevelText;

        [Title("楼层控制")]
        
        [LabelText("楼层文本")]
        [SerializeField]
        private Text _floorText;

        [LabelText("上一层按钮")]
        [SerializeField]
        private Button _floorUpButton;

        [LabelText("下一层按钮")]
        [SerializeField]
        private Button _floorDownButton;

        [Title("操作按钮")]
        
        [LabelText("保存按钮")]
        [SerializeField]
        private Button _saveButton;

        [LabelText("加载按钮")]
        [SerializeField]
        private Button _loadButton;

        [LabelText("清空按钮")]
        [SerializeField]
        private Button _clearButton;

        [LabelText("退出按钮")]
        [SerializeField]
        private Button _exitButton;

        #endregion

        #region 事件

        public event Action<EditorMode> OnModeSelected;
        public event Action<TileEditTool> OnTileToolSelected;
        public event Action<TileType> OnTileTypeSelected;
        public event Action<string> OnObjectSelected;
        public event Action<int> OnBrushSizeChanged;
        public event Action<int> OnHeightLevelChanged;
        public event UnityAction OnFloorUp;
        public event UnityAction OnFloorDown;
        public event UnityAction OnSave;
        public event UnityAction OnLoad;
        public event UnityAction OnClear;
        public event UnityAction OnExit;

        #endregion

        #region 属性

        public RoomGridEditor Editor { get; private set; }

        #endregion

        #region 私有字段

        private Dictionary<EditorMode, Button> _modeButtons = new Dictionary<EditorMode, Button>();
        private Dictionary<TileEditTool, Button> _toolButtons = new Dictionary<TileEditTool, Button>();
        private Dictionary<TileType, Button> _tileTypeButtons = new Dictionary<TileType, Button>();

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化UI
        /// </summary>
        public void Initialize(RoomGridEditor editor)
        {
            Editor = editor;

            SetupModeButtons();
            SetupToolButtons();
            SetupTileTypeButtons();
            SetupSliders();
            SetupFloorButtons();
            SetupActionButtons();

            // 订阅编辑器事件
            if (editor != null)
            {
                SubscribeEditorEvents();
            }

            Debug.Log("[RoomEditorUIPanel] 初始化完成");
        }

        /// <summary>
        /// 订阅编辑器事件
        /// </summary>
        private void SubscribeEditorEvents()
        {
            if (Editor?.State == null) return;

            Editor.State.OnModeChanged += UpdateModeDisplay;
            Editor.State.OnTileToolChanged += UpdateToolDisplay;
            Editor.State.OnSelectedTileTypeChanged += UpdateTileTypeDisplay;
            Editor.State.OnBrushSizeChanged += UpdateBrushSizeDisplay;
            Editor.State.OnCurrentFloorChanged += UpdateFloorDisplay;
        }

        #endregion

        #region 按钮设置

        private void SetupModeButtons()
        {
            // 这里需要根据实际UI结构来设置
            // 示例：为每个模式创建按钮
            CreateModeButton(EditorMode.View, "查看");
            CreateModeButton(EditorMode.TileEdit, "地块编辑");
            CreateModeButton(EditorMode.ObjectPlace, "物品放置");
            CreateModeButton(EditorMode.ObjectSelect, "选择");
            CreateModeButton(EditorMode.Delete, "删除");
        }

        private void CreateModeButton(EditorMode mode, string label)
        {
            if (_modeButtonGroup == null) return;

            // 查找或创建按钮
            var buttonObj = _modeButtonGroup.Find(mode.ToString());
            if (buttonObj == null)
            {
                // 如果需要动态创建按钮
                // buttonObj = Instantiate(buttonPrefab, _modeButtonGroup);
                return;
            }

            var button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                _modeButtons[mode] = button;
                button.onClick.AddListener(() => 
                {
                    OnModeSelected?.Invoke(mode);
                    Editor?.SetMode(mode);
                });
            }
        }

        private void SetupToolButtons()
        {
            CreateToolButton(TileEditTool.Brush, "画笔");
            CreateToolButton(TileEditTool.Fill, "填充");
            CreateToolButton(TileEditTool.Rectangle, "矩形");
            CreateToolButton(TileEditTool.Eraser, "橡皮擦");
        }

        private void CreateToolButton(TileEditTool tool, string label)
        {
            if (_tileToolButtonGroup == null) return;

            var buttonObj = _tileToolButtonGroup.Find(tool.ToString());
            if (buttonObj == null) return;

            var button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                _toolButtons[tool] = button;
                button.onClick.AddListener(() => 
                {
                    OnTileToolSelected?.Invoke(tool);
                    Editor?.SetTileTool(tool);
                });
            }
        }

        private void SetupTileTypeButtons()
        {
            // 常用地块类型
            TileType[] commonTypes = 
            {
                TileType.Grass,
                TileType.Dirt,
                TileType.Stone,
                TileType.Wood,
                TileType.Sand,
                TileType.Water
            };

            foreach (var type in commonTypes)
            {
                CreateTileTypeButton(type);
            }
        }

        private void CreateTileTypeButton(TileType type)
        {
            if (_tileTypePanel == null) return;

            var buttonObj = _tileTypePanel.Find(type.ToString());
            if (buttonObj == null) return;

            var button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                _tileTypeButtons[type] = button;
                button.onClick.AddListener(() => 
                {
                    OnTileTypeSelected?.Invoke(type);
                    Editor?.SetTileType(type);
                });
            }
        }

        private void SetupSliders()
        {
            // 画笔大小滑块
            if (_brushSizeSlider != null)
            {
                _brushSizeSlider.minValue = 1;
                _brushSizeSlider.maxValue = 10;
                _brushSizeSlider.wholeNumbers = true;
                _brushSizeSlider.value = 1;
                _brushSizeSlider.onValueChanged.AddListener(value => 
                {
                    int size = Mathf.RoundToInt(value);
                    OnBrushSizeChanged?.Invoke(size);
                    Editor?.State?.SetBrushSize(size);
                    UpdateBrushSizeDisplay(size);
                });
            }

            // 高度等级滑块
            if (_heightLevelSlider != null)
            {
                _heightLevelSlider.minValue = 0;
                _heightLevelSlider.maxValue = 10;
                _heightLevelSlider.wholeNumbers = true;
                _heightLevelSlider.value = 0;
                _heightLevelSlider.onValueChanged.AddListener(value => 
                {
                    int level = Mathf.RoundToInt(value);
                    OnHeightLevelChanged?.Invoke(level);
                    Editor?.State?.SetHeightLevel(level);
                    UpdateHeightLevelDisplay(level);
                });
            }
        }

        private void SetupFloorButtons()
        {
            if (_floorUpButton != null)
            {
                _floorUpButton.onClick.AddListener(() => 
                {
                    OnFloorUp?.Invoke();
                    Editor?.Grid?.GoUpFloor();
                });
            }

            if (_floorDownButton != null)
            {
                _floorDownButton.onClick.AddListener(() => 
                {
                    OnFloorDown?.Invoke();
                    Editor?.Grid?.GoDownFloor();
                });
            }
        }

        private void SetupActionButtons()
        {
            if (_saveButton != null)
            {
                _saveButton.onClick.AddListener(() => OnSave?.Invoke());
            }

            if (_loadButton != null)
            {
                _loadButton.onClick.AddListener(() => OnLoad?.Invoke());
            }

            if (_clearButton != null)
            {
                _clearButton.onClick.AddListener(() => 
                {
                    OnClear?.Invoke();
                    // 可以添加确认对话框
                });
            }

            if (_exitButton != null)
            {
                _exitButton.onClick.AddListener(() => OnExit?.Invoke());
            }
        }

        #endregion

        #region 更新显示

        private void Update()
        {
            if (Editor == null || !Editor.IsInitialized) return;

            UpdateMousePositionDisplay();
            UpdateStatisticsDisplay();
        }

        private void UpdateModeDisplay(EditorMode oldMode, EditorMode newMode)
        {
            // 更新模式按钮高亮
            foreach (var kvp in _modeButtons)
            {
                SetButtonHighlight(kvp.Value, kvp.Key == newMode);
            }

            // 更新模式文本
            if (_currentModeText != null)
            {
                _currentModeText.text = GetModeDisplayName(newMode);
            }

            // 显示/隐藏相关面板
            UpdatePanelVisibility(newMode);
        }

        private void UpdateToolDisplay(TileEditTool oldTool, TileEditTool newTool)
        {
            foreach (var kvp in _toolButtons)
            {
                SetButtonHighlight(kvp.Value, kvp.Key == newTool);
            }
        }

        private void UpdateTileTypeDisplay(TileType type)
        {
            foreach (var kvp in _tileTypeButtons)
            {
                SetButtonHighlight(kvp.Value, kvp.Key == type);
            }
        }

        private void UpdateBrushSizeDisplay(int size)
        {
            if (_brushSizeText != null)
            {
                _brushSizeText.text = $"画笔: {size}";
            }

            if (_brushSizeSlider != null && Mathf.RoundToInt(_brushSizeSlider.value) != size)
            {
                _brushSizeSlider.value = size;
            }
        }

        private void UpdateHeightLevelDisplay(int level)
        {
            if (_heightLevelText != null)
            {
                _heightLevelText.text = $"高度: {level}";
            }
        }

        private void UpdateFloorDisplay(int floor)
        {
            if (_floorText != null)
            {
                _floorText.text = $"{floor + 1}F";
            }

            // 更新按钮状态
            if (Editor?.Grid?.Config != null)
            {
                int maxFloor = Editor.Grid.Config.FloorCount - 1;
                if (_floorUpButton != null)
                    _floorUpButton.interactable = floor < maxFloor;
                if (_floorDownButton != null)
                    _floorDownButton.interactable = floor > 0;
            }
        }

        private void UpdateMousePositionDisplay()
        {
            if (_mousePositionText == null || Editor?.State == null) return;

            var pos = Editor.State.CurrentMouseTilePosition;
            bool valid = Editor.State.IsMouseInValidArea;

            _mousePositionText.text = valid ? 
                $"位置: ({pos.X}, {pos.Z})" : 
                "位置: 无效";
        }

        private void UpdateStatisticsDisplay()
        {
            if (_statisticsText == null || Editor == null) return;

            var stats = Editor.GetStatistics();
            _statisticsText.text = $"地块: {stats.TotalTiles} | 物品: {stats.PlacedObjects}";
        }

        private void UpdatePanelVisibility(EditorMode mode)
        {
            // 地块工具面板
            if (_tileToolButtonGroup != null)
            {
                _tileToolButtonGroup.gameObject.SetActive(mode == EditorMode.TileEdit);
            }

            // 地块类型面板
            if (_tileTypePanel != null)
            {
                _tileTypePanel.gameObject.SetActive(mode == EditorMode.TileEdit);
            }

            // 物品列表面板
            if (_objectListPanel != null)
            {
                _objectListPanel.gameObject.SetActive(mode == EditorMode.ObjectPlace);
            }
        }

        #endregion

        #region 辅助方法

        private void SetButtonHighlight(Button button, bool highlight)
        {
            if (button == null) return;

            // 简单的高亮实现，可以根据实际需求修改
            var colors = button.colors;
            colors.normalColor = highlight ? Color.yellow : Color.white;
            button.colors = colors;
        }

        private string GetModeDisplayName(EditorMode mode)
        {
            return mode switch
            {
                EditorMode.View => "查看模式",
                EditorMode.TileEdit => "地块编辑",
                EditorMode.ObjectPlace => "物品放置",
                EditorMode.ObjectSelect => "选择物品",
                EditorMode.Delete => "删除模式",
                _ => mode.ToString()
            };
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 显示提示信息
        /// </summary>
        public void ShowToast(string message, float duration = 2f)
        {
            Debug.Log($"[Toast] {message}");
            // TODO: 实现Toast UI
        }

        /// <summary>
        /// 显示确认对话框
        /// </summary>
        public void ShowConfirmDialog(string title, string message, UnityAction onConfirm, UnityAction onCancel = null)
        {
            // TODO: 实现确认对话框
            Debug.Log($"[Confirm] {title}: {message}");
            onConfirm?.Invoke();
        }

        /// <summary>
        /// 刷新物品列表
        /// </summary>
        public void RefreshObjectList(List<ObjectDefinition> objects)
        {
            // TODO: 实现物品列表刷新
        }

        #endregion
    }
}