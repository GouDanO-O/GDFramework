using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Core.Game.Chunk.Room.Grid;

namespace Core.Game.View
{
    /// <summary>
    /// 房间编辑器地块面板
    /// 提供地块类型选择、画笔大小和高度设置
    /// </summary>
    public class RoomEditorTilePalette : MonoBehaviour
    {
        #region 事件

        /// <summary>
        /// 地块类型选择事件
        /// </summary>
        public event UnityAction<TileType> OnTileTypeSelected;

        /// <summary>
        /// 画笔大小改变事件
        /// </summary>
        public event UnityAction<int> OnBrushSizeChanged;

        /// <summary>
        /// 高度等级改变事件
        /// </summary>
        public event UnityAction<int> OnHeightLevelChanged;

        #endregion

        #region UI引用

        private RectTransform _rectTransform;
        private VerticalLayoutGroup _layoutGroup;
        private ScrollRect _scrollRect;

        // 地块类型Toggle组
        private ToggleGroup _tileToggleGroup;
        private Dictionary<TileType, Toggle> _tileToggles = new Dictionary<TileType, Toggle>();

        // 画笔大小滑块
        private Slider _brushSizeSlider;
        private Text _brushSizeText;

        // 高度等级滑块
        private Slider _heightLevelSlider;
        private Text _heightLevelText;

        #endregion

        #region 配置

        private readonly Dictionary<TileType, (string name, Color color)> _tileTypeConfig =
            new Dictionary<TileType, (string, Color)>
            {
                { TileType.Grass, ("草地", new Color(0.3f, 0.7f, 0.3f)) },
                { TileType.Dirt, ("泥土", new Color(0.5f, 0.35f, 0.2f)) },
                { TileType.Stone, ("石板", new Color(0.5f, 0.5f, 0.5f)) },
                { TileType.Wood, ("木地板", new Color(0.6f, 0.4f, 0.2f)) },
                { TileType.Sand, ("沙地", new Color(0.9f, 0.85f, 0.6f)) },
                { TileType.Water, ("水", new Color(0.2f, 0.5f, 0.9f)) },
                { TileType.Carpet, ("地毯", new Color(0.7f, 0.2f, 0.2f)) },
                { TileType.Tile, ("瓷砖", new Color(0.9f, 0.9f, 0.9f)) },
                { TileType.Metal, ("金属", new Color(0.7f, 0.7f, 0.8f)) },
                { TileType.Glass, ("玻璃", new Color(0.5f, 0.8f, 1f)) },
                { TileType.Snow, ("雪地", Color.white) },
                { TileType.Lava, ("岩浆", new Color(1f, 0.3f, 0f)) },
                { TileType.Ice, ("冰面", new Color(0.7f, 0.9f, 1f)) },
            };

        private int _currentBrushSize = 1;
        private int _currentHeightLevel = 0;
        private TileType _currentTileType = TileType.Grass;

        #endregion

        #region 初始化

        public void Initialize()
        {
            _rectTransform = GetComponent<RectTransform>();
            if (_rectTransform == null)
            {
                _rectTransform = gameObject.AddComponent<RectTransform>();
            }

            SetupLayout();
            CreateTileTypeSection();
            CreateBrushSizeSection();
            CreateHeightLevelSection();

            // 默认隐藏
            gameObject.SetActive(false);

            Debug.Log("[RoomEditorTilePalette] 初始化完成");
        }

        private void SetupLayout()
        {
            // 设置为左侧面板
            _rectTransform.anchorMin = new Vector2(0, 0);
            _rectTransform.anchorMax = new Vector2(0, 1);
            _rectTransform.pivot = new Vector2(0, 0.5f);
            _rectTransform.anchoredPosition = new Vector2(10, -30);
            _rectTransform.sizeDelta = new Vector2(180, -80);

            // 添加背景
            var bgImage = gameObject.AddComponent<Image>();
            bgImage.color = new Color(0.15f, 0.15f, 0.15f, 0.95f);

            // 添加垂直布局
            _layoutGroup = gameObject.AddComponent<VerticalLayoutGroup>();
            _layoutGroup.padding = new RectOffset(10, 10, 10, 10);
            _layoutGroup.spacing = 10;
            _layoutGroup.childAlignment = TextAnchor.UpperCenter;
            _layoutGroup.childControlWidth = true;
            _layoutGroup.childControlHeight = false;
            _layoutGroup.childForceExpandWidth = true;
            _layoutGroup.childForceExpandHeight = false;
        }

        #endregion

        #region UI创建

        private void CreateTileTypeSection()
        {
            // 标题
            CreateLabel("地块类型", 16, FontStyle.Bold);

            // 创建滚动区域
            var scrollContainer = new GameObject("TileScrollView", typeof(RectTransform));
            scrollContainer.transform.SetParent(transform);

            var scrollRect = scrollContainer.GetComponent<RectTransform>();
            var scrollLayoutElement = scrollContainer.AddComponent<LayoutElement>();
            scrollLayoutElement.preferredHeight = 200;
            scrollLayoutElement.flexibleHeight = 1;

            // 添加Mask
            var mask = scrollContainer.AddComponent<Mask>();
            mask.showMaskGraphic = false;
            var maskImage = scrollContainer.AddComponent<Image>();
            maskImage.color = Color.white;

            // 添加ScrollRect
            _scrollRect = scrollContainer.AddComponent<ScrollRect>();
            _scrollRect.horizontal = false;
            _scrollRect.vertical = true;

            // 创建内容容器
            var content = new GameObject("Content", typeof(RectTransform));
            content.transform.SetParent(scrollContainer.transform);

            var contentRect = content.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.anchoredPosition = Vector2.zero;

            // 添加GridLayoutGroup
            var gridLayout = content.AddComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(70, 50);
            gridLayout.spacing = new Vector2(5, 5);
            gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
            gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
            gridLayout.childAlignment = TextAnchor.UpperLeft;
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = 2;

            // 添加ContentSizeFitter
            var contentFitter = content.AddComponent<ContentSizeFitter>();
            contentFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            _scrollRect.content = contentRect;

            // 创建Toggle组
            _tileToggleGroup = content.AddComponent<ToggleGroup>();
            _tileToggleGroup.allowSwitchOff = false;

            // 创建地块类型按钮
            foreach (var kvp in _tileTypeConfig)
            {
                var toggle = CreateTileToggle(content.transform, kvp.Key, kvp.Value.name, kvp.Value.color);
                _tileToggles[kvp.Key] = toggle;
            }

            // 默认选中草地
            if (_tileToggles.ContainsKey(TileType.Grass))
            {
                _tileToggles[TileType.Grass].isOn = true;
            }
        }

        private void CreateBrushSizeSection()
        {
            // 标题
            CreateLabel("画笔大小", 14, FontStyle.Normal);

            // 滑块容器
            var container = new GameObject("BrushSizeContainer", typeof(RectTransform));
            container.transform.SetParent(transform);

            var containerRect = container.GetComponent<RectTransform>();
            var layoutElement = container.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 30;

            var hLayout = container.AddComponent<HorizontalLayoutGroup>();
            hLayout.spacing = 10;
            hLayout.childAlignment = TextAnchor.MiddleCenter;
            hLayout.childControlWidth = true;
            hLayout.childControlHeight = true;

            // 创建滑块
            _brushSizeSlider = CreateSlider(container.transform, 1, 10, 1);
            _brushSizeSlider.wholeNumbers = true;
            _brushSizeSlider.onValueChanged.AddListener(OnBrushSizeSliderChanged);

            // 数值显示
            _brushSizeText = CreateValueText(container.transform, "1");
        }

        private void CreateHeightLevelSection()
        {
            // 标题
            CreateLabel("高度等级", 14, FontStyle.Normal);

            // 滑块容器
            var container = new GameObject("HeightLevelContainer", typeof(RectTransform));
            container.transform.SetParent(transform);

            var containerRect = container.GetComponent<RectTransform>();
            var layoutElement = container.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 30;

            var hLayout = container.AddComponent<HorizontalLayoutGroup>();
            hLayout.spacing = 10;
            hLayout.childAlignment = TextAnchor.MiddleCenter;
            hLayout.childControlWidth = true;
            hLayout.childControlHeight = true;

            // 创建滑块
            _heightLevelSlider = CreateSlider(container.transform, 0, 10, 0);
            _heightLevelSlider.wholeNumbers = true;
            _heightLevelSlider.onValueChanged.AddListener(OnHeightLevelSliderChanged);

            // 数值显示
            _heightLevelText = CreateValueText(container.transform, "0");
        }

        private Text CreateLabel(string text, int fontSize, FontStyle style)
        {
            var labelGO = new GameObject("Label_" + text, typeof(RectTransform));
            labelGO.transform.SetParent(transform);

            var rect = labelGO.GetComponent<RectTransform>();
            var layoutElement = labelGO.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 25;

            var textComp = labelGO.AddComponent<Text>();
            textComp.text = text;
            textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            textComp.fontSize = fontSize;
            textComp.fontStyle = style;
            textComp.color = Color.white;
            textComp.alignment = TextAnchor.MiddleLeft;

            return textComp;
        }

        private Toggle CreateTileToggle(Transform parent, TileType type, string name, Color color)
        {
            var toggleGO = new GameObject(name + "Toggle", typeof(RectTransform));
            toggleGO.transform.SetParent(parent);

            // 背景
            var bgImage = toggleGO.AddComponent<Image>();
            bgImage.color = new Color(0.3f, 0.3f, 0.3f);

            // Toggle组件
            var toggle = toggleGO.AddComponent<Toggle>();
            toggle.targetGraphic = bgImage;
            toggle.group = _tileToggleGroup;

            // 颜色指示器
            var colorIndicator = new GameObject("ColorIndicator", typeof(RectTransform));
            colorIndicator.transform.SetParent(toggleGO.transform);

            var indicatorRect = colorIndicator.GetComponent<RectTransform>();
            indicatorRect.anchorMin = new Vector2(0, 0);
            indicatorRect.anchorMax = new Vector2(0.3f, 1);
            indicatorRect.offsetMin = new Vector2(3, 3);
            indicatorRect.offsetMax = new Vector2(-3, -3);

            var indicatorImage = colorIndicator.AddComponent<Image>();
            indicatorImage.color = color;

            // 选中标记（边框高亮）
            var checkmark = new GameObject("Checkmark", typeof(RectTransform));
            checkmark.transform.SetParent(toggleGO.transform);

            var checkRect = checkmark.GetComponent<RectTransform>();
            checkRect.anchorMin = Vector2.zero;
            checkRect.anchorMax = Vector2.one;
            checkRect.offsetMin = Vector2.zero;
            checkRect.offsetMax = Vector2.zero;

            var outline = checkmark.AddComponent<Outline>();
            outline.effectColor = new Color(0.4f, 0.8f, 1f);
            outline.effectDistance = new Vector2(2, 2);

            var checkImage = checkmark.AddComponent<Image>();
            checkImage.color = new Color(1, 1, 1, 0); // 透明，只显示边框

            toggle.graphic = checkImage;

            // 文字
            var textGO = new GameObject("Text", typeof(RectTransform));
            textGO.transform.SetParent(toggleGO.transform);

            var textRect = textGO.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.3f, 0);
            textRect.anchorMax = new Vector2(1, 1);
            textRect.offsetMin = new Vector2(5, 0);
            textRect.offsetMax = new Vector2(-3, 0);

            var textComp = textGO.AddComponent<Text>();
            textComp.text = name;
            textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            textComp.fontSize = 12;
            textComp.color = Color.white;
            textComp.alignment = TextAnchor.MiddleLeft;

            // 事件处理
            var tileType = type;
            toggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn) OnTileToggleChanged(tileType);
            });

            return toggle;
        }

        private Slider CreateSlider(Transform parent, float min, float max, float value)
        {
            var sliderGO = new GameObject("Slider", typeof(RectTransform));
            sliderGO.transform.SetParent(parent);

            var sliderRect = sliderGO.GetComponent<RectTransform>();
            var layoutElement = sliderGO.AddComponent<LayoutElement>();
            layoutElement.flexibleWidth = 1;
            layoutElement.preferredHeight = 20;

            // 背景
            var background = new GameObject("Background", typeof(RectTransform));
            background.transform.SetParent(sliderGO.transform);
            var bgRect = background.GetComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0, 0.25f);
            bgRect.anchorMax = new Vector2(1, 0.75f);
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            var bgImage = background.AddComponent<Image>();
            bgImage.color = new Color(0.3f, 0.3f, 0.3f);

            // 填充区域
            var fillArea = new GameObject("FillArea", typeof(RectTransform));
            fillArea.transform.SetParent(sliderGO.transform);
            var fillAreaRect = fillArea.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0, 0.25f);
            fillAreaRect.anchorMax = new Vector2(1, 0.75f);
            fillAreaRect.offsetMin = new Vector2(5, 0);
            fillAreaRect.offsetMax = new Vector2(-5, 0);

            var fill = new GameObject("Fill", typeof(RectTransform));
            fill.transform.SetParent(fillArea.transform);
            var fillRect = fill.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = new Vector2(0, 1);
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            var fillImage = fill.AddComponent<Image>();
            fillImage.color = new Color(0.4f, 0.7f, 1f);

            // 滑块区域
            var handleArea = new GameObject("HandleSlideArea", typeof(RectTransform));
            handleArea.transform.SetParent(sliderGO.transform);
            var handleAreaRect = handleArea.GetComponent<RectTransform>();
            handleAreaRect.anchorMin = Vector2.zero;
            handleAreaRect.anchorMax = Vector2.one;
            handleAreaRect.offsetMin = new Vector2(10, 0);
            handleAreaRect.offsetMax = new Vector2(-10, 0);

            var handle = new GameObject("Handle", typeof(RectTransform));
            handle.transform.SetParent(handleArea.transform);
            var handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(20, 20);
            var handleImage = handle.AddComponent<Image>();
            handleImage.color = Color.white;

            // Slider组件
            var slider = sliderGO.AddComponent<Slider>();
            slider.fillRect = fillRect;
            slider.handleRect = handleRect;
            slider.minValue = min;
            slider.maxValue = max;
            slider.value = value;
            slider.direction = Slider.Direction.LeftToRight;

            return slider;
        }

        private Text CreateValueText(Transform parent, string text)
        {
            var textGO = new GameObject("ValueText", typeof(RectTransform));
            textGO.transform.SetParent(parent);

            var rect = textGO.GetComponent<RectTransform>();
            var layoutElement = textGO.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = 30;
            layoutElement.minWidth = 30;

            var textComp = textGO.AddComponent<Text>();
            textComp.text = text;
            textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            textComp.fontSize = 14;
            textComp.color = Color.white;
            textComp.alignment = TextAnchor.MiddleCenter;

            return textComp;
        }

        #endregion

        #region 事件处理

        private void OnTileToggleChanged(TileType type)
        {
            _currentTileType = type;
            OnTileTypeSelected?.Invoke(type);
        }

        private void OnBrushSizeSliderChanged(float value)
        {
            _currentBrushSize = Mathf.RoundToInt(value);
            _brushSizeText.text = _currentBrushSize.ToString();
            OnBrushSizeChanged?.Invoke(_currentBrushSize);
        }

        private void OnHeightLevelSliderChanged(float value)
        {
            _currentHeightLevel = Mathf.RoundToInt(value);
            _heightLevelText.text = _currentHeightLevel.ToString();
            OnHeightLevelChanged?.Invoke(_currentHeightLevel);
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 设置选中的地块类型
        /// </summary>
        public void SetSelectedTileType(TileType type)
        {
            if (_tileToggles.TryGetValue(type, out var toggle))
            {
                toggle.isOn = true;
            }
            _currentTileType = type;
        }

        /// <summary>
        /// 设置画笔大小
        /// </summary>
        public void SetBrushSize(int size)
        {
            if (_brushSizeSlider != null)
            {
                _brushSizeSlider.value = size;
            }
            _currentBrushSize = size;
            if (_brushSizeText != null)
            {
                _brushSizeText.text = size.ToString();
            }
        }

        /// <summary>
        /// 设置高度等级
        /// </summary>
        public void SetHeightLevel(int level)
        {
            if (_heightLevelSlider != null)
            {
                _heightLevelSlider.value = level;
            }
            _currentHeightLevel = level;
            if (_heightLevelText != null)
            {
                _heightLevelText.text = level.ToString();
            }
        }

        /// <summary>
        /// 设置可见性
        /// </summary>
        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        /// <summary>
        /// 获取当前选中的地块类型
        /// </summary>
        public TileType GetSelectedTileType() => _currentTileType;

        /// <summary>
        /// 获取当前画笔大小
        /// </summary>
        public int GetBrushSize() => _currentBrushSize;

        /// <summary>
        /// 获取当前高度等级
        /// </summary>
        public int GetHeightLevel() => _currentHeightLevel;

        #endregion
    }
}
