using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Core.Game.Chunk.Room.Grid;

namespace Core.Game.View
{
    /// <summary>
    /// 房间编辑器物品面板
    /// 提供物品类别选择和物品列表
    /// </summary>
    public class RoomEditorObjectPalette : MonoBehaviour
    {
        #region 事件

        /// <summary>
        /// 物品选择事件
        /// </summary>
        public event UnityAction<string> OnObjectSelected;

        /// <summary>
        /// 类别改变事件
        /// </summary>
        public event UnityAction<ObjectCategory> OnCategoryChanged;

        #endregion

        #region UI引用

        private RectTransform _rectTransform;
        private VerticalLayoutGroup _layoutGroup;

        // 类别选择
        private Dropdown _categoryDropdown;

        // 搜索框
        private InputField _searchInput;

        // 物品列表
        private ScrollRect _scrollRect;
        private RectTransform _contentRoot;
        private ToggleGroup _objectToggleGroup;
        private Dictionary<string, Toggle> _objectToggles = new Dictionary<string, Toggle>();

        // 物品详情
        private Text _detailNameText;
        private Text _detailDescText;
        private Text _detailSizeText;

        #endregion

        #region 配置

        private ObjectDefinitionManager _defManager;
        private ObjectCategory _currentCategory = ObjectCategory.Furniture;
        private string _currentObjectId;
        private string _searchKeyword = "";

        private readonly Dictionary<ObjectCategory, (string name, Color color)> _categoryConfig =
            new Dictionary<ObjectCategory, (string, Color)>
            {
                { ObjectCategory.Furniture, ("家具", new Color(0.6f, 0.4f, 0.2f)) },
                { ObjectCategory.Decoration, ("装饰", new Color(0.8f, 0.6f, 0.8f)) },
                { ObjectCategory.Plant, ("植物", new Color(0.3f, 0.7f, 0.3f)) },
                { ObjectCategory.Lighting, ("照明", new Color(1f, 0.9f, 0.5f)) },
                { ObjectCategory.Storage, ("存储", new Color(0.5f, 0.5f, 0.6f)) },
                { ObjectCategory.Interactive, ("交互", new Color(0.4f, 0.6f, 0.9f)) },
                { ObjectCategory.Teleport, ("传送", new Color(0.7f, 0.4f, 0.9f)) },
                { ObjectCategory.NPC, ("NPC", new Color(0.9f, 0.6f, 0.4f)) },
                { ObjectCategory.Other, ("其他", new Color(0.6f, 0.6f, 0.6f)) },
            };

        #endregion

        #region 初始化

        public void Initialize(ObjectDefinitionManager defManager)
        {
            _defManager = defManager;

            _rectTransform = GetComponent<RectTransform>();
            if (_rectTransform == null)
            {
                _rectTransform = gameObject.AddComponent<RectTransform>();
            }

            SetupLayout();
            CreateCategoryDropdown();
            CreateSearchBox();
            CreateObjectList();
            CreateDetailPanel();

            // 加载初始列表
            RefreshObjectList(_currentCategory);

            // 默认隐藏
            gameObject.SetActive(false);

            Debug.Log("[RoomEditorObjectPalette] 初始化完成");
        }

        private void SetupLayout()
        {
            // 设置为右侧面板
            _rectTransform.anchorMin = new Vector2(1, 0);
            _rectTransform.anchorMax = new Vector2(1, 1);
            _rectTransform.pivot = new Vector2(1, 0.5f);
            _rectTransform.anchoredPosition = new Vector2(-10, -30);
            _rectTransform.sizeDelta = new Vector2(220, -80);

            // 添加背景
            var bgImage = gameObject.AddComponent<Image>();
            bgImage.color = new Color(0.15f, 0.15f, 0.15f, 0.95f);

            // 添加垂直布局
            _layoutGroup = gameObject.AddComponent<VerticalLayoutGroup>();
            _layoutGroup.padding = new RectOffset(10, 10, 10, 10);
            _layoutGroup.spacing = 8;
            _layoutGroup.childAlignment = TextAnchor.UpperCenter;
            _layoutGroup.childControlWidth = true;
            _layoutGroup.childControlHeight = false;
            _layoutGroup.childForceExpandWidth = true;
            _layoutGroup.childForceExpandHeight = false;
        }

        #endregion

        #region UI创建

        private void CreateCategoryDropdown()
        {
            // 标题
            CreateLabel("物品类别", 14, FontStyle.Bold);

            // Dropdown容器
            var dropdownGO = new GameObject("CategoryDropdown", typeof(RectTransform));
            dropdownGO.transform.SetParent(transform);

            var rect = dropdownGO.GetComponent<RectTransform>();
            var layoutElement = dropdownGO.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 30;

            // 背景
            var bgImage = dropdownGO.AddComponent<Image>();
            bgImage.color = new Color(0.3f, 0.3f, 0.3f);

            // Dropdown组件
            _categoryDropdown = dropdownGO.AddComponent<Dropdown>();
            _categoryDropdown.targetGraphic = bgImage;

            // 创建Label
            var labelGO = new GameObject("Label", typeof(RectTransform));
            labelGO.transform.SetParent(dropdownGO.transform);
            var labelRect = labelGO.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 0);
            labelRect.anchorMax = new Vector2(1, 1);
            labelRect.offsetMin = new Vector2(10, 0);
            labelRect.offsetMax = new Vector2(-25, 0);

            var labelText = labelGO.AddComponent<Text>();
            labelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            labelText.fontSize = 14;
            labelText.color = Color.white;
            labelText.alignment = TextAnchor.MiddleLeft;
            _categoryDropdown.captionText = labelText;

            // 创建箭头
            var arrowGO = new GameObject("Arrow", typeof(RectTransform));
            arrowGO.transform.SetParent(dropdownGO.transform);
            var arrowRect = arrowGO.GetComponent<RectTransform>();
            arrowRect.anchorMin = new Vector2(1, 0.5f);
            arrowRect.anchorMax = new Vector2(1, 0.5f);
            arrowRect.sizeDelta = new Vector2(20, 20);
            arrowRect.anchoredPosition = new Vector2(-15, 0);
            var arrowImage = arrowGO.AddComponent<Image>();
            arrowImage.color = Color.white;

            // 创建Template
            var templateGO = new GameObject("Template", typeof(RectTransform));
            templateGO.transform.SetParent(dropdownGO.transform);
            var templateRect = templateGO.GetComponent<RectTransform>();
            templateRect.anchorMin = new Vector2(0, 0);
            templateRect.anchorMax = new Vector2(1, 0);
            templateRect.pivot = new Vector2(0.5f, 1);
            templateRect.anchoredPosition = Vector2.zero;
            templateRect.sizeDelta = new Vector2(0, 150);
            templateGO.SetActive(false);

            var templateImage = templateGO.AddComponent<Image>();
            templateImage.color = new Color(0.25f, 0.25f, 0.25f);

            // Template ScrollRect
            var templateScrollRect = templateGO.AddComponent<ScrollRect>();
            templateScrollRect.horizontal = false;
            templateScrollRect.movementType = ScrollRect.MovementType.Clamped;

            // Viewport
            var viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Mask), typeof(Image));
            viewport.transform.SetParent(templateGO.transform);
            var viewportRect = viewport.GetComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.sizeDelta = Vector2.zero;
            viewportRect.pivot = new Vector2(0, 1);
            viewport.GetComponent<Image>().color = Color.white;
            viewport.GetComponent<Mask>().showMaskGraphic = false;
            templateScrollRect.viewport = viewportRect;

            // Content
            var content = new GameObject("Content", typeof(RectTransform));
            content.transform.SetParent(viewport.transform);
            var contentRect = content.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = new Vector2(0, 28);
            templateScrollRect.content = contentRect;

            // Item Template
            var itemGO = new GameObject("Item", typeof(RectTransform));
            itemGO.transform.SetParent(content.transform);
            var itemRect = itemGO.GetComponent<RectTransform>();
            itemRect.anchorMin = new Vector2(0, 0.5f);
            itemRect.anchorMax = new Vector2(1, 0.5f);
            itemRect.sizeDelta = new Vector2(0, 28);

            var itemToggle = itemGO.AddComponent<Toggle>();
            itemToggle.targetGraphic = itemGO.AddComponent<Image>();
            itemToggle.targetGraphic.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f);

            // Item Background (checkmark)
            var itemBg = new GameObject("Item Background", typeof(RectTransform));
            itemBg.transform.SetParent(itemGO.transform);
            var itemBgRect = itemBg.GetComponent<RectTransform>();
            itemBgRect.anchorMin = Vector2.zero;
            itemBgRect.anchorMax = Vector2.one;
            itemBgRect.sizeDelta = Vector2.zero;
            var itemBgImage = itemBg.AddComponent<Image>();
            itemBgImage.color = new Color(0.4f, 0.6f, 0.8f);
            itemToggle.graphic = itemBgImage;

            // Item Label
            var itemLabel = new GameObject("Item Label", typeof(RectTransform));
            itemLabel.transform.SetParent(itemGO.transform);
            var itemLabelRect = itemLabel.GetComponent<RectTransform>();
            itemLabelRect.anchorMin = Vector2.zero;
            itemLabelRect.anchorMax = Vector2.one;
            itemLabelRect.offsetMin = new Vector2(10, 0);
            itemLabelRect.offsetMax = new Vector2(-10, 0);

            var itemLabelText = itemLabel.AddComponent<Text>();
            itemLabelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            itemLabelText.fontSize = 14;
            itemLabelText.color = Color.white;
            itemLabelText.alignment = TextAnchor.MiddleLeft;

            _categoryDropdown.template = templateRect;
            _categoryDropdown.itemText = itemLabelText;

            // 添加选项
            var options = new List<Dropdown.OptionData>();
            foreach (var kvp in _categoryConfig)
            {
                options.Add(new Dropdown.OptionData(kvp.Value.name));
            }
            _categoryDropdown.AddOptions(options);

            // 事件
            _categoryDropdown.onValueChanged.AddListener(OnCategoryDropdownChanged);
        }

        private void CreateSearchBox()
        {
            // 搜索框容器
            var searchGO = new GameObject("SearchBox", typeof(RectTransform));
            searchGO.transform.SetParent(transform);

            var rect = searchGO.GetComponent<RectTransform>();
            var layoutElement = searchGO.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 30;

            // 背景
            var bgImage = searchGO.AddComponent<Image>();
            bgImage.color = new Color(0.25f, 0.25f, 0.25f);

            // InputField
            _searchInput = searchGO.AddComponent<InputField>();

            // 占位符
            var placeholderGO = new GameObject("Placeholder", typeof(RectTransform));
            placeholderGO.transform.SetParent(searchGO.transform);
            var placeholderRect = placeholderGO.GetComponent<RectTransform>();
            placeholderRect.anchorMin = Vector2.zero;
            placeholderRect.anchorMax = Vector2.one;
            placeholderRect.offsetMin = new Vector2(10, 0);
            placeholderRect.offsetMax = new Vector2(-10, 0);

            var placeholderText = placeholderGO.AddComponent<Text>();
            placeholderText.text = "搜索物品...";
            placeholderText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            placeholderText.fontSize = 12;
            placeholderText.color = new Color(0.6f, 0.6f, 0.6f);
            placeholderText.alignment = TextAnchor.MiddleLeft;
            placeholderText.fontStyle = FontStyle.Italic;

            // 文本
            var textGO = new GameObject("Text", typeof(RectTransform));
            textGO.transform.SetParent(searchGO.transform);
            var textRect = textGO.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(10, 0);
            textRect.offsetMax = new Vector2(-10, 0);

            var inputText = textGO.AddComponent<Text>();
            inputText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            inputText.fontSize = 12;
            inputText.color = Color.white;
            inputText.alignment = TextAnchor.MiddleLeft;
            inputText.supportRichText = false;

            _searchInput.textComponent = inputText;
            _searchInput.placeholder = placeholderText;
            _searchInput.onValueChanged.AddListener(OnSearchInputChanged);
        }

        private void CreateObjectList()
        {
            // 标题
            CreateLabel("物品列表", 14, FontStyle.Bold);

            // 滚动区域
            var scrollContainer = new GameObject("ObjectScrollView", typeof(RectTransform));
            scrollContainer.transform.SetParent(transform);

            var scrollRect = scrollContainer.GetComponent<RectTransform>();
            var scrollLayoutElement = scrollContainer.AddComponent<LayoutElement>();
            scrollLayoutElement.preferredHeight = 300;
            scrollLayoutElement.flexibleHeight = 1;

            // Mask
            var mask = scrollContainer.AddComponent<Mask>();
            mask.showMaskGraphic = false;
            var maskImage = scrollContainer.AddComponent<Image>();
            maskImage.color = Color.white;

            // ScrollRect
            _scrollRect = scrollContainer.AddComponent<ScrollRect>();
            _scrollRect.horizontal = false;
            _scrollRect.vertical = true;

            // Content
            var content = new GameObject("Content", typeof(RectTransform));
            content.transform.SetParent(scrollContainer.transform);

            _contentRoot = content.GetComponent<RectTransform>();
            _contentRoot.anchorMin = new Vector2(0, 1);
            _contentRoot.anchorMax = new Vector2(1, 1);
            _contentRoot.pivot = new Vector2(0.5f, 1);
            _contentRoot.anchoredPosition = Vector2.zero;

            // 垂直布局
            var layout = content.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(5, 5, 5, 5);
            layout.spacing = 5;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            // ContentSizeFitter
            var fitter = content.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            _scrollRect.content = _contentRoot;

            // Toggle组
            _objectToggleGroup = content.AddComponent<ToggleGroup>();
            _objectToggleGroup.allowSwitchOff = true;
        }

        private void CreateDetailPanel()
        {
            // 详情面板
            var detailPanel = new GameObject("DetailPanel", typeof(RectTransform));
            detailPanel.transform.SetParent(transform);

            var rect = detailPanel.GetComponent<RectTransform>();
            var layoutElement = detailPanel.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 80;

            var bgImage = detailPanel.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.2f);

            var layout = detailPanel.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(8, 8, 5, 5);
            layout.spacing = 3;
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = false;

            // 名称
            var nameGO = new GameObject("NameText", typeof(RectTransform));
            nameGO.transform.SetParent(detailPanel.transform);
            var nameLayoutElement = nameGO.AddComponent<LayoutElement>();
            nameLayoutElement.preferredHeight = 20;

            _detailNameText = nameGO.AddComponent<Text>();
            _detailNameText.text = "选择一个物品";
            _detailNameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            _detailNameText.fontSize = 14;
            _detailNameText.fontStyle = FontStyle.Bold;
            _detailNameText.color = Color.white;

            // 描述
            var descGO = new GameObject("DescText", typeof(RectTransform));
            descGO.transform.SetParent(detailPanel.transform);
            var descLayoutElement = descGO.AddComponent<LayoutElement>();
            descLayoutElement.preferredHeight = 30;

            _detailDescText = descGO.AddComponent<Text>();
            _detailDescText.text = "";
            _detailDescText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            _detailDescText.fontSize = 11;
            _detailDescText.color = new Color(0.8f, 0.8f, 0.8f);

            // 尺寸
            var sizeGO = new GameObject("SizeText", typeof(RectTransform));
            sizeGO.transform.SetParent(detailPanel.transform);
            var sizeLayoutElement = sizeGO.AddComponent<LayoutElement>();
            sizeLayoutElement.preferredHeight = 15;

            _detailSizeText = sizeGO.AddComponent<Text>();
            _detailSizeText.text = "";
            _detailSizeText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            _detailSizeText.fontSize = 11;
            _detailSizeText.color = new Color(0.6f, 0.8f, 1f);
        }

        private Text CreateLabel(string text, int fontSize, FontStyle style)
        {
            var labelGO = new GameObject("Label_" + text, typeof(RectTransform));
            labelGO.transform.SetParent(transform);

            var rect = labelGO.GetComponent<RectTransform>();
            var layoutElement = labelGO.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 20;

            var textComp = labelGO.AddComponent<Text>();
            textComp.text = text;
            textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            textComp.fontSize = fontSize;
            textComp.fontStyle = style;
            textComp.color = Color.white;
            textComp.alignment = TextAnchor.MiddleLeft;

            return textComp;
        }

        private Toggle CreateObjectItem(ObjectDefinition def)
        {
            var itemGO = new GameObject(def.Id, typeof(RectTransform));
            itemGO.transform.SetParent(_contentRoot);

            var rect = itemGO.GetComponent<RectTransform>();
            var layoutElement = itemGO.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 40;

            // 背景
            var bgImage = itemGO.AddComponent<Image>();
            bgImage.color = new Color(0.25f, 0.25f, 0.25f);

            // Toggle
            var toggle = itemGO.AddComponent<Toggle>();
            toggle.targetGraphic = bgImage;
            toggle.group = _objectToggleGroup;

            // 选中标记
            var checkmark = new GameObject("Checkmark", typeof(RectTransform));
            checkmark.transform.SetParent(itemGO.transform);
            var checkRect = checkmark.GetComponent<RectTransform>();
            checkRect.anchorMin = Vector2.zero;
            checkRect.anchorMax = Vector2.one;
            checkRect.offsetMin = Vector2.zero;
            checkRect.offsetMax = Vector2.zero;

            var checkImage = checkmark.AddComponent<Image>();
            checkImage.color = new Color(0.4f, 0.7f, 1f, 0.3f);
            toggle.graphic = checkImage;

            // 类别颜色条
            if (_categoryConfig.TryGetValue(def.Category, out var catConfig))
            {
                var colorBar = new GameObject("ColorBar", typeof(RectTransform));
                colorBar.transform.SetParent(itemGO.transform);
                var colorRect = colorBar.GetComponent<RectTransform>();
                colorRect.anchorMin = new Vector2(0, 0);
                colorRect.anchorMax = new Vector2(0, 1);
                colorRect.sizeDelta = new Vector2(4, 0);
                colorRect.anchoredPosition = new Vector2(2, 0);

                var colorImage = colorBar.AddComponent<Image>();
                colorImage.color = catConfig.color;
            }

            // 名称
            var nameGO = new GameObject("Name", typeof(RectTransform));
            nameGO.transform.SetParent(itemGO.transform);
            var nameRect = nameGO.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 0.5f);
            nameRect.anchorMax = new Vector2(1, 1);
            nameRect.offsetMin = new Vector2(10, 0);
            nameRect.offsetMax = new Vector2(-10, 0);

            var nameText = nameGO.AddComponent<Text>();
            nameText.text = def.Name;
            nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            nameText.fontSize = 13;
            nameText.color = Color.white;
            nameText.alignment = TextAnchor.MiddleLeft;

            // 尺寸信息
            var sizeGO = new GameObject("Size", typeof(RectTransform));
            sizeGO.transform.SetParent(itemGO.transform);
            var sizeRect = sizeGO.GetComponent<RectTransform>();
            sizeRect.anchorMin = new Vector2(0, 0);
            sizeRect.anchorMax = new Vector2(1, 0.5f);
            sizeRect.offsetMin = new Vector2(10, 0);
            sizeRect.offsetMax = new Vector2(-10, 0);

            var sizeText = sizeGO.AddComponent<Text>();
            sizeText.text = $"{def.Size.Width}x{def.Size.Depth} | {def.Price}G";
            sizeText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            sizeText.fontSize = 10;
            sizeText.color = new Color(0.7f, 0.7f, 0.7f);
            sizeText.alignment = TextAnchor.MiddleLeft;

            // 事件
            var objectId = def.Id;
            toggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn) OnObjectItemSelected(objectId);
            });

            return toggle;
        }

        #endregion

        #region 事件处理

        private void OnCategoryDropdownChanged(int index)
        {
            var categories = new List<ObjectCategory>(_categoryConfig.Keys);
            if (index >= 0 && index < categories.Count)
            {
                _currentCategory = categories[index];
                RefreshObjectList(_currentCategory);
                OnCategoryChanged?.Invoke(_currentCategory);
            }
        }

        private void OnSearchInputChanged(string keyword)
        {
            _searchKeyword = keyword;
            RefreshObjectList(_currentCategory);
        }

        private void OnObjectItemSelected(string objectId)
        {
            _currentObjectId = objectId;
            UpdateDetailPanel(objectId);
            OnObjectSelected?.Invoke(objectId);
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 刷新物品列表
        /// </summary>
        public void RefreshObjectList(ObjectCategory category)
        {
            if (_defManager == null) return;

            // 清除现有列表
            foreach (Transform child in _contentRoot)
            {
                Destroy(child.gameObject);
            }
            _objectToggles.Clear();

            // 获取该类别的物品
            var definitions = _defManager.GetDefinitionsByCategory(category);

            // 如果有搜索关键词，进行过滤
            if (!string.IsNullOrEmpty(_searchKeyword))
            {
                var keyword = _searchKeyword.ToLower();
                definitions = definitions.FindAll(d =>
                    d.Name.ToLower().Contains(keyword) ||
                    d.Id.ToLower().Contains(keyword) ||
                    (d.Description != null && d.Description.ToLower().Contains(keyword)));
            }

            // 创建列表项
            foreach (var def in definitions)
            {
                var toggle = CreateObjectItem(def);
                _objectToggles[def.Id] = toggle;
            }

            // 清除详情
            if (_objectToggles.Count == 0)
            {
                UpdateDetailPanel(null);
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
        /// 设置当前类别
        /// </summary>
        public void SetCategory(ObjectCategory category)
        {
            _currentCategory = category;

            // 更新Dropdown
            var categories = new List<ObjectCategory>(_categoryConfig.Keys);
            int index = categories.IndexOf(category);
            if (index >= 0 && _categoryDropdown != null)
            {
                _categoryDropdown.value = index;
            }

            RefreshObjectList(category);
        }

        /// <summary>
        /// 获取当前选中的物品ID
        /// </summary>
        public string GetSelectedObjectId() => _currentObjectId;

        #endregion

        #region 私有方法

        private void UpdateDetailPanel(string objectId)
        {
            if (string.IsNullOrEmpty(objectId) || _defManager == null)
            {
                _detailNameText.text = "选择一个物品";
                _detailDescText.text = "";
                _detailSizeText.text = "";
                return;
            }

            var def = _defManager.GetDefinition(objectId);
            if (def == null) return;

            _detailNameText.text = def.Name;
            _detailDescText.text = def.Description ?? "";
            _detailSizeText.text = $"尺寸: {def.Size.Width}x{def.Size.Depth}x{def.Size.Height:F1} | 价格: {def.Price}G";
        }

        #endregion
    }
}
