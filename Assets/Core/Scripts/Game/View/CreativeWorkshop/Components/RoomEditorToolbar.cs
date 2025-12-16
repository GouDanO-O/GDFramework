using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Core.Game.Chunk.Room.Grid;

namespace Core.Game.View
{
    /// <summary>
    /// 房间编辑器工具栏
    /// 提供模式切换、工具选择和文件操作
    /// </summary>
    public class RoomEditorToolbar : MonoBehaviour
    {
        #region 事件

        /// <summary>
        /// 模式选择事件
        /// </summary>
        public event UnityAction<EditorMode> OnModeSelected;

        /// <summary>
        /// 工具选择事件
        /// </summary>
        public event UnityAction<TileEditTool> OnToolSelected;

        /// <summary>
        /// 新建点击事件
        /// </summary>
        public event UnityAction OnNewClicked;

        /// <summary>
        /// 保存点击事件
        /// </summary>
        public event UnityAction OnSaveClicked;

        /// <summary>
        /// 加载点击事件
        /// </summary>
        public event UnityAction OnLoadClicked;

        /// <summary>
        /// 设置点击事件
        /// </summary>
        public event UnityAction OnSettingsClicked;

        #endregion

        #region UI引用

        private RectTransform _rectTransform;
        private HorizontalLayoutGroup _layoutGroup;

        // 模式按钮组
        private ToggleGroup _modeToggleGroup;
        private Dictionary<EditorMode, Toggle> _modeToggles = new Dictionary<EditorMode, Toggle>();

        // 工具按钮组
        private ToggleGroup _toolToggleGroup;
        private Dictionary<TileEditTool, Toggle> _toolToggles = new Dictionary<TileEditTool, Toggle>();

        // 文件操作按钮
        private Button _newButton;
        private Button _saveButton;
        private Button _loadButton;
        private Button _settingsButton;

        #endregion

        #region 配置

        private readonly Dictionary<EditorMode, string> _modeLabels = new Dictionary<EditorMode, string>
        {
            { EditorMode.View, "查看" },
            { EditorMode.TileEdit, "地块" },
            { EditorMode.ObjectPlace, "放置" },
            { EditorMode.ObjectSelect, "选择" },
            { EditorMode.Delete, "删除" }
        };

        private readonly Dictionary<TileEditTool, string> _toolLabels = new Dictionary<TileEditTool, string>
        {
            { TileEditTool.Brush, "画笔" },
            { TileEditTool.Fill, "填充" },
            { TileEditTool.Rectangle, "矩形" },
            { TileEditTool.Eraser, "橡皮" }
        };

        private Color _normalColor = new Color(0.8f, 0.8f, 0.8f);
        private Color _selectedColor = new Color(0.4f, 0.7f, 1f);
        private Color _hoverColor = new Color(0.9f, 0.9f, 0.9f);

        #endregion

        #region 初始化

        public void Initialize()
        {
            _rectTransform = GetComponent<RectTransform>();
            if (_rectTransform == null)
            {
                _rectTransform = gameObject.AddComponent<RectTransform>();
            }

            // 设置工具栏布局
            SetupLayout();

            // 创建UI元素
            CreateFileButtons();
            CreateSeparator();
            CreateModeButtons();
            CreateSeparator();
            CreateToolButtons();

            Debug.Log("[RoomEditorToolbar] 初始化完成");
        }

        private void SetupLayout()
        {
            // 设置为顶部工具栏
            _rectTransform.anchorMin = new Vector2(0, 1);
            _rectTransform.anchorMax = new Vector2(1, 1);
            _rectTransform.pivot = new Vector2(0.5f, 1);
            _rectTransform.anchoredPosition = new Vector2(0, 0);
            _rectTransform.sizeDelta = new Vector2(0, 50);

            // 添加背景
            var bgImage = gameObject.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.95f);

            // 添加水平布局
            _layoutGroup = gameObject.AddComponent<HorizontalLayoutGroup>();
            _layoutGroup.padding = new RectOffset(10, 10, 5, 5);
            _layoutGroup.spacing = 5;
            _layoutGroup.childAlignment = TextAnchor.MiddleLeft;
            _layoutGroup.childControlWidth = false;
            _layoutGroup.childControlHeight = true;
            _layoutGroup.childForceExpandWidth = false;
            _layoutGroup.childForceExpandHeight = true;
        }

        #endregion

        #region UI创建

        private void CreateFileButtons()
        {
            var container = CreateButtonContainer("FileButtons");

            _newButton = CreateButton(container.transform, "新建", OnNewButtonClicked);
            _saveButton = CreateButton(container.transform, "保存", OnSaveButtonClicked);
            _loadButton = CreateButton(container.transform, "加载", OnLoadButtonClicked);
            _settingsButton = CreateButton(container.transform, "设置", OnSettingsButtonClicked);
        }

        private void CreateModeButtons()
        {
            var container = CreateButtonContainer("ModeButtons");

            // 创建Toggle组
            _modeToggleGroup = container.AddComponent<ToggleGroup>();
            _modeToggleGroup.allowSwitchOff = false;

            // 创建模式切换按钮
            foreach (var kvp in _modeLabels)
            {
                var toggle = CreateToggleButton(container.transform, kvp.Value, _modeToggleGroup);
                _modeToggles[kvp.Key] = toggle;

                var mode = kvp.Key;
                toggle.onValueChanged.AddListener(isOn =>
                {
                    if (isOn) OnModeToggleChanged(mode);
                });
            }

            // 默认选中查看模式
            if (_modeToggles.ContainsKey(EditorMode.View))
            {
                _modeToggles[EditorMode.View].isOn = true;
            }
        }

        private void CreateToolButtons()
        {
            var container = CreateButtonContainer("ToolButtons");

            // 创建Toggle组
            _toolToggleGroup = container.AddComponent<ToggleGroup>();
            _toolToggleGroup.allowSwitchOff = false;

            // 创建工具切换按钮
            foreach (var kvp in _toolLabels)
            {
                var toggle = CreateToggleButton(container.transform, kvp.Value, _toolToggleGroup);
                _toolToggles[kvp.Key] = toggle;

                var tool = kvp.Key;
                toggle.onValueChanged.AddListener(isOn =>
                {
                    if (isOn) OnToolToggleChanged(tool);
                });
            }

            // 默认选中画笔
            if (_toolToggles.ContainsKey(TileEditTool.Brush))
            {
                _toolToggles[TileEditTool.Brush].isOn = true;
            }
        }

        private GameObject CreateButtonContainer(string name)
        {
            var container = new GameObject(name, typeof(RectTransform));
            container.transform.SetParent(transform);

            var rect = container.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 40);

            // 添加水平布局
            var layout = container.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 3;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            // 添加ContentSizeFitter让容器自适应
            var fitter = container.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;

            return container;
        }

        private void CreateSeparator()
        {
            var separator = new GameObject("Separator", typeof(RectTransform));
            separator.transform.SetParent(transform);

            var rect = separator.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(2, 30);

            var image = separator.AddComponent<Image>();
            image.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);

            var layoutElement = separator.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = 2;
            layoutElement.minWidth = 2;
        }

        private Button CreateButton(Transform parent, string text, UnityAction onClick)
        {
            var buttonGO = new GameObject(text + "Button", typeof(RectTransform));
            buttonGO.transform.SetParent(parent);

            var rect = buttonGO.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(60, 35);

            // 背景
            var image = buttonGO.AddComponent<Image>();
            image.color = _normalColor;

            // 按钮组件
            var button = buttonGO.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(onClick);

            // 设置颜色
            var colors = button.colors;
            colors.normalColor = _normalColor;
            colors.highlightedColor = _hoverColor;
            colors.pressedColor = _selectedColor;
            button.colors = colors;

            // 文字
            var textGO = new GameObject("Text", typeof(RectTransform));
            textGO.transform.SetParent(buttonGO.transform);

            var textRect = textGO.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            var textComp = textGO.AddComponent<Text>();
            textComp.text = text;
            textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            textComp.fontSize = 14;
            textComp.color = Color.black;
            textComp.alignment = TextAnchor.MiddleCenter;

            // LayoutElement
            var layoutElement = buttonGO.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = 60;
            layoutElement.minWidth = 50;

            return button;
        }

        private Toggle CreateToggleButton(Transform parent, string text, ToggleGroup group)
        {
            var toggleGO = new GameObject(text + "Toggle", typeof(RectTransform));
            toggleGO.transform.SetParent(parent);

            var rect = toggleGO.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(60, 35);

            // 背景
            var bgImage = toggleGO.AddComponent<Image>();
            bgImage.color = _normalColor;

            // Toggle组件
            var toggle = toggleGO.AddComponent<Toggle>();
            toggle.targetGraphic = bgImage;
            toggle.group = group;

            // 创建选中标记（改变背景色）
            var checkmark = new GameObject("Checkmark", typeof(RectTransform));
            checkmark.transform.SetParent(toggleGO.transform);

            var checkRect = checkmark.GetComponent<RectTransform>();
            checkRect.anchorMin = Vector2.zero;
            checkRect.anchorMax = Vector2.one;
            checkRect.offsetMin = Vector2.zero;
            checkRect.offsetMax = Vector2.zero;

            var checkImage = checkmark.AddComponent<Image>();
            checkImage.color = _selectedColor;

            toggle.graphic = checkImage;

            // 文字
            var textGO = new GameObject("Text", typeof(RectTransform));
            textGO.transform.SetParent(toggleGO.transform);

            var textRect = textGO.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            var textComp = textGO.AddComponent<Text>();
            textComp.text = text;
            textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            textComp.fontSize = 14;
            textComp.color = Color.black;
            textComp.alignment = TextAnchor.MiddleCenter;

            // LayoutElement
            var layoutElement = toggleGO.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = 60;
            layoutElement.minWidth = 50;

            return toggle;
        }

        #endregion

        #region 事件处理

        private void OnNewButtonClicked() => OnNewClicked?.Invoke();
        private void OnSaveButtonClicked() => OnSaveClicked?.Invoke();
        private void OnLoadButtonClicked() => OnLoadClicked?.Invoke();
        private void OnSettingsButtonClicked() => OnSettingsClicked?.Invoke();

        private void OnModeToggleChanged(EditorMode mode)
        {
            OnModeSelected?.Invoke(mode);
        }

        private void OnToolToggleChanged(TileEditTool tool)
        {
            OnToolSelected?.Invoke(tool);
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 设置当前激活的模式
        /// </summary>
        public void SetActiveMode(EditorMode mode)
        {
            if (_modeToggles.TryGetValue(mode, out var toggle))
            {
                toggle.isOn = true;
            }
        }

        /// <summary>
        /// 设置当前激活的工具
        /// </summary>
        public void SetActiveTool(TileEditTool tool)
        {
            if (_toolToggles.TryGetValue(tool, out var toggle))
            {
                toggle.isOn = true;
            }
        }

        /// <summary>
        /// 设置工具按钮可见性（仅在地块编辑模式下显示）
        /// </summary>
        public void SetToolButtonsVisible(bool visible)
        {
            foreach (var toggle in _toolToggles.Values)
            {
                toggle.gameObject.SetActive(visible);
            }
        }

        /// <summary>
        /// 启用/禁用保存按钮
        /// </summary>
        public void SetSaveEnabled(bool enabled)
        {
            if (_saveButton != null)
            {
                _saveButton.interactable = enabled;
            }
        }

        #endregion
    }
}
