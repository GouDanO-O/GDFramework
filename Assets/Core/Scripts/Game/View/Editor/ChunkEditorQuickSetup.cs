#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

namespace Core.Game.View.Editor
{
    /// <summary>
    /// Chunk编辑器快速搭建工具
    /// 适配 UIPanel 标准结构（Root/Bg/Ground/Common/Top）
    /// </summary>
    public class ChunkEditorQuickSetup
    {
        [MenuItem("GameObject/UI/Chunk Editor Content", false, 10)]
        static void CreateChunkEditorContent(MenuCommand menuCommand)
        {
            GameObject selectedObj = Selection.activeGameObject;
            if (selectedObj == null || selectedObj.name != "Common")
            {
                EditorUtility.DisplayDialog("提示", 
                    "请先选中 UI_ChunkEditorPanel 的 Common 节点！\n\n" +
                    "标准结构: UI_ChunkEditorPanel > Root > Common", 
                    "确定");
                return;
            }
            
            // 在 Common 下创建 ChunkEditorRoot
            GameObject chunkEditorRoot = new GameObject("ChunkEditorRoot");
            chunkEditorRoot.transform.SetParent(selectedObj.transform, false);
            
            RectTransform rect = chunkEditorRoot.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            // 创建子结构
            CreateHeaderRoot(chunkEditorRoot.transform);
            CreateListRoot(chunkEditorRoot.transform);
            CreateEditorRoot(chunkEditorRoot.transform);
            
            Selection.activeGameObject = chunkEditorRoot;
            EditorUtility.DisplayDialog("完成", "Chunk Editor Content 创建完成！", "确定");
        }

        static void CreateHeaderRoot(Transform parent)
        {
            GameObject header = new GameObject("HeaderRoot");
            header.transform.SetParent(parent, false);
            
            RectTransform headerRect = header.AddComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0, 1);
            headerRect.anchorMax = new Vector2(1, 1);
            headerRect.pivot = new Vector2(0.5f, 1);
            headerRect.anchoredPosition = Vector2.zero;
            headerRect.sizeDelta = new Vector2(0, 60);
            
            Image headerBg = header.AddComponent<Image>();
            headerBg.color = new Color(0.15f, 0.15f, 0.15f, 1f);
            
            HorizontalLayoutGroup layout = header.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 10;
            layout.padding = new RectOffset(10, 10, 10, 10);
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;
            
            CreateButton(header.transform, "UniverseEditorButton", "宇宙编辑器", new Color(0.3f, 0.3f, 0.4f));
            CreateButton(header.transform, "WorldEditorButton", "世界编辑器", new Color(0.3f, 0.3f, 0.4f));
            
            GameObject spacer = new GameObject("Spacer");
            spacer.transform.SetParent(header.transform, false);
            LayoutElement spacerLayout = spacer.AddComponent<LayoutElement>();
            spacerLayout.flexibleWidth = 10;
            
            CreateButton(header.transform, "ExitButton", "退出", new Color(0.8f, 0.2f, 0.2f));
        }

        static void CreateListRoot(Transform parent)
        {
            GameObject listRoot = new GameObject("ListRoot");
            listRoot.transform.SetParent(parent, false);
            
            RectTransform listRect = listRoot.AddComponent<RectTransform>();
            listRect.anchorMin = new Vector2(0, 0);
            listRect.anchorMax = new Vector2(0, 1);
            listRect.pivot = new Vector2(0, 1);
            listRect.anchoredPosition = Vector2.zero;
            listRect.offsetMin = new Vector2(0, 0);
            listRect.offsetMax = new Vector2(300, -60);
            
            Image listBg = listRoot.AddComponent<Image>();
            listBg.color = new Color(0.1f, 0.1f, 0.1f, 1f);
            
            CreateScrollView(listRoot.transform, "UniverseListScrollView");
            CreateScrollView(listRoot.transform, "WorldListScrollView");
        }

        static void CreateEditorRoot(Transform parent)
        {
            GameObject editorRoot = new GameObject("EditorRoot");
            editorRoot.transform.SetParent(parent, false);
            
            RectTransform editorRect = editorRoot.AddComponent<RectTransform>();
            editorRect.anchorMin = new Vector2(0, 0);
            editorRect.anchorMax = new Vector2(1, 1);
            editorRect.offsetMin = new Vector2(310, 0);
            editorRect.offsetMax = new Vector2(-10, -60);
            
            Image editorBg = editorRoot.AddComponent<Image>();
            editorBg.color = new Color(0.12f, 0.12f, 0.12f, 1f);
            
            CreateUniverseEditorRoot(editorRoot.transform);
            CreateUniverseVisualEditorRoot(editorRoot.transform); // 新增：创建可视化编辑器
            CreateWorldEditorRoot(editorRoot.transform);
        }

        static void CreateUniverseEditorRoot(Transform parent)
        {
            GameObject universeEditor = CreateScrollableEditor(parent, "UniverseEditorRoot");
            Transform content = universeEditor.transform.Find("Viewport/Content");
            
            // 基本信息
            GameObject basicInfo = CreateSection(content, "BasicInfo", "基本信息");
            CreateInputField(basicInfo.transform, "DefIdInput", "配置ID (只读)", true, 30);
            CreateInputField(basicInfo.transform, "DefNameInput", "配置名称", false, 30);
            CreateInputField(basicInfo.transform, "DefDescInput", "配置描述", false, 80);
            
            // 世界配置
            GameObject worldConfig = CreateSection(content, "WorldConfig", "世界配置");
            CreateInputField(worldConfig.transform, "InitialPlayerWorldIdInput", "初始玩家世界ID", false, 30);
            CreateStringListSection(worldConfig.transform, "InitialShowingWorldList", "初始展示世界列表");
            CreateStringListSection(worldConfig.transform, "WorldIdList", "所有世界ID列表");
            
            // 按钮
            CreateButtonRoot(content, 
                new string[]{"SaveButton", "CreateNewButton", "DeleteButton"}, 
                new string[]{"保存", "创建新宇宙", "删除"},
                new Color[]{new Color(0.2f, 0.6f, 0.2f), new Color(0.2f, 0.4f, 0.8f), new Color(0.8f, 0.2f, 0.2f)});
        }

        // 新增：创建宇宙可视化编辑器
        static void CreateUniverseVisualEditorRoot(Transform parent)
        {
            GameObject visualEditor = new GameObject("UniverseVisualEditorRoot");
            visualEditor.transform.SetParent(parent, false);
            
            RectTransform rect = visualEditor.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            // 控制面板（顶部）
            GameObject controlPanel = CreateVisualEditorControlPanel(visualEditor.transform);
            
            // 地图画布（底部主区域）
            GameObject mapCanvas = CreateUniverseMapCanvas(visualEditor.transform);
            
            visualEditor.SetActive(false); // 默认隐藏
        }

        static GameObject CreateVisualEditorControlPanel(Transform parent)
        {
            GameObject controlPanel = new GameObject("ControlPanel");
            controlPanel.transform.SetParent(parent, false);
            
            RectTransform rect = controlPanel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(0, 80);
            
            Image bg = controlPanel.AddComponent<Image>();
            bg.color = new Color(0.08f, 0.08f, 0.08f, 1f);
            
            HorizontalLayoutGroup layout = controlPanel.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 10;
            layout.padding = new RectOffset(10, 10, 10, 10);
            layout.childControlWidth = false;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;
            
            // 保存按钮
            CreateButton(controlPanel.transform, "SaveButton", "保存地图", new Color(0.2f, 0.6f, 0.2f));
            
            // 添加世界按钮
            CreateButton(controlPanel.transform, "AddWorldButton", "添加世界", new Color(0.2f, 0.4f, 0.8f));
            
            // 清空按钮
            CreateButton(controlPanel.transform, "ClearButton", "清空所有", new Color(0.8f, 0.2f, 0.2f));
            
            // 间隔
            GameObject spacer = new GameObject("Spacer");
            spacer.transform.SetParent(controlPanel.transform, false);
            LayoutElement spacerLayout = spacer.AddComponent<LayoutElement>();
            spacerLayout.preferredWidth = 20;
            
            // 网格大小输入
            GameObject gridSizeGroup = new GameObject("GridSizeGroup");
            gridSizeGroup.transform.SetParent(controlPanel.transform, false);
            HorizontalLayoutGroup gridLayout = gridSizeGroup.AddComponent<HorizontalLayoutGroup>();
            gridLayout.spacing = 5;
            
            GameObject gridLabel = new GameObject("Label");
            gridLabel.transform.SetParent(gridSizeGroup.transform, false);
            TextMeshProUGUI labelText = gridLabel.AddComponent<TextMeshProUGUI>();
            labelText.text = "网格:";
            labelText.fontSize = 14;
            labelText.color = Color.white;
            labelText.alignment = TextAlignmentOptions.Left;
            LayoutElement labelLayout = gridLabel.AddComponent<LayoutElement>();
            labelLayout.preferredWidth = 50;
            
            GameObject gridSizeInput = new GameObject("GridSizeInput");
            gridSizeInput.transform.SetParent(gridSizeGroup.transform, false);
            Image inputBg = gridSizeInput.AddComponent<Image>();
            inputBg.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
            TMP_InputField inputField = gridSizeInput.AddComponent<TMP_InputField>();
            inputField.text = "50";
            LayoutElement inputLayout = gridSizeInput.AddComponent<LayoutElement>();
            inputLayout.preferredWidth = 80;
            
            GameObject inputText = new GameObject("Text");
            inputText.transform.SetParent(gridSizeInput.transform, false);
            TextMeshProUGUI text = inputText.AddComponent<TextMeshProUGUI>();
            text.fontSize = 14;
            text.color = Color.white;
            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(5, 0);
            textRect.offsetMax = new Vector2(-5, 0);
            inputField.textComponent = text;
            
            // 网格吸附开关
            GameObject toggleGroup = new GameObject("GridSnapGroup");
            toggleGroup.transform.SetParent(controlPanel.transform, false);
            HorizontalLayoutGroup toggleLayout = toggleGroup.AddComponent<HorizontalLayoutGroup>();
            toggleLayout.spacing = 5;
            
            GameObject toggleObj = new GameObject("GridSnapToggle");
            toggleObj.transform.SetParent(toggleGroup.transform, false);
            Toggle toggle = toggleObj.AddComponent<Toggle>();
            toggle.isOn = true;
            Image toggleBg = toggleObj.AddComponent<Image>();
            toggleBg.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
            LayoutElement toggleElem = toggleObj.AddComponent<LayoutElement>();
            toggleElem.preferredWidth = 30;
            
            GameObject checkmark = new GameObject("Checkmark");
            checkmark.transform.SetParent(toggleObj.transform, false);
            Image checkImg = checkmark.AddComponent<Image>();
            checkImg.color = new Color(0.2f, 0.8f, 0.2f);
            RectTransform checkRect = checkImg.GetComponent<RectTransform>();
            checkRect.anchorMin = new Vector2(0.2f, 0.2f);
            checkRect.anchorMax = new Vector2(0.8f, 0.8f);
            checkRect.offsetMin = Vector2.zero;
            checkRect.offsetMax = Vector2.zero;
            toggle.graphic = checkImg;
            
            GameObject toggleLabel = new GameObject("Label");
            toggleLabel.transform.SetParent(toggleGroup.transform, false);
            TextMeshProUGUI toggleText = toggleLabel.AddComponent<TextMeshProUGUI>();
            toggleText.text = "吸附";
            toggleText.fontSize = 14;
            toggleText.color = Color.white;
            
            // 坐标显示
            GameObject coordDisplay = new GameObject("CoordinateDisplay");
            coordDisplay.transform.SetParent(controlPanel.transform, false);
            TextMeshProUGUI coordText = coordDisplay.AddComponent<TextMeshProUGUI>();
            coordText.text = "X: 0, Y: 0";
            coordText.fontSize = 14;
            coordText.color = new Color(0.7f, 0.7f, 0.7f);
            coordText.alignment = TextAlignmentOptions.Left;
            LayoutElement coordLayout = coordDisplay.AddComponent<LayoutElement>();
            coordLayout.preferredWidth = 150;
            
            return controlPanel;
        }

        static GameObject CreateUniverseMapCanvas(Transform parent)
        {
            GameObject mapCanvas = new GameObject("UniverseMapCanvas");
            mapCanvas.transform.SetParent(parent, false);
            
            RectTransform rect = mapCanvas.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(0, 0);
            rect.offsetMax = new Vector2(0, -90); // 为控制面板留空间
            
            Image bg = mapCanvas.AddComponent<Image>();
            bg.color = new Color(0.05f, 0.05f, 0.05f, 1f);
            
            // 创建世界节点容器
            GameObject nodesContainer = new GameObject("WorldNodesContainer");
            nodesContainer.transform.SetParent(mapCanvas.transform, false);
            
            RectTransform containerRect = nodesContainer.AddComponent<RectTransform>();
            containerRect.anchorMin = Vector2.zero;
            containerRect.anchorMax = Vector2.one;
            containerRect.offsetMin = Vector2.zero;
            containerRect.offsetMax = Vector2.zero;
            
            return mapCanvas;
        }

        static void CreateWorldEditorRoot(Transform parent)
        {
            GameObject worldEditor = CreateScrollableEditor(parent, "WorldEditorRoot");
            Transform content = worldEditor.transform.Find("Viewport/Content");
            
            // 基本信息
            GameObject basicInfo = CreateSection(content, "BasicInfo", "基本信息");
            CreateInputField(basicInfo.transform, "DefIdInput", "配置ID (只读)", true, 30);
            CreateInputField(basicInfo.transform, "DefNameInput", "配置名称", false, 30);
            CreateInputField(basicInfo.transform, "DefDescInput", "配置描述", false, 80);
            CreateDropdown(basicInfo.transform, "UniverseDropdown", "所属宇宙");
            
            // 区域配置
            GameObject regionConfig = CreateSection(content, "RegionConfig", "区域配置");
            CreateInputField(regionConfig.transform, "InitialPlayerRegionIdInput", "初始玩家区域ID", false, 30);
            CreateStringListSection(regionConfig.transform, "InitialShowingRegionList", "初始展示区域列表");
            CreateStringListSection(regionConfig.transform, "RegionIdList", "所有区域ID列表");
            
            // 按钮
            CreateButtonRoot(content, 
                new string[]{"SaveButton", "CreateNewButton", "DeleteButton"}, 
                new string[]{"保存", "创建新世界", "删除"},
                new Color[]{new Color(0.2f, 0.6f, 0.2f), new Color(0.2f, 0.4f, 0.8f), new Color(0.8f, 0.2f, 0.2f)});
        }

        static GameObject CreateScrollableEditor(Transform parent, string name)
        {
            GameObject editor = new GameObject(name);
            editor.transform.SetParent(parent, false);
            
            RectTransform rect = editor.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            ScrollRect scrollRect = editor.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            
            // Viewport
            GameObject viewport = new GameObject("Viewport");
            viewport.transform.SetParent(editor.transform, false);
            RectTransform viewportRect = viewport.AddComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = Vector2.zero;
            
            Mask mask = viewport.AddComponent<Mask>();
            mask.showMaskGraphic = false;
            Image maskImage = viewport.AddComponent<Image>();
            maskImage.color = Color.clear;
            
            // Content
            GameObject content = new GameObject("Content");
            content.transform.SetParent(viewport.transform, false);
            RectTransform contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = new Vector2(0, 0);
            
            VerticalLayoutGroup layout = content.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 20;
            layout.padding = new RectOffset(20, 20, 20, 20);
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            
            ContentSizeFitter fitter = content.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            scrollRect.viewport = viewportRect;
            scrollRect.content = contentRect;
            
            editor.SetActive(false);
            
            return editor;
        }

        static GameObject CreateScrollView(Transform parent, string name)
        {
            GameObject scrollView = new GameObject(name);
            scrollView.transform.SetParent(parent, false);
            
            RectTransform rect = scrollView.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            ScrollRect scrollRect = scrollView.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            
            Image scrollBg = scrollView.AddComponent<Image>();
            scrollBg.color = new Color(0.05f, 0.05f, 0.05f, 0f);
            
            // Viewport
            GameObject viewport = new GameObject("Viewport");
            viewport.transform.SetParent(scrollView.transform, false);
            RectTransform viewportRect = viewport.AddComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = Vector2.zero;
            
            Mask mask = viewport.AddComponent<Mask>();
            mask.showMaskGraphic = false;
            Image maskImage = viewport.AddComponent<Image>();
            maskImage.color = Color.clear;
            
            // Content
            GameObject content = new GameObject("Content");
            content.transform.SetParent(viewport.transform, false);
            RectTransform contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.anchoredPosition = Vector2.zero;
            
            VerticalLayoutGroup contentLayout = content.AddComponent<VerticalLayoutGroup>();
            contentLayout.spacing = 5;
            contentLayout.padding = new RectOffset(5, 5, 5, 5);
            contentLayout.childControlWidth = true;
            contentLayout.childControlHeight = false;
            contentLayout.childForceExpandWidth = true;
            
            ContentSizeFitter contentFitter = content.AddComponent<ContentSizeFitter>();
            contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            scrollRect.viewport = viewportRect;
            scrollRect.content = contentRect;
            
            scrollView.SetActive(false);
            
            return scrollView;
        }

        static GameObject CreateSection(Transform parent, string name, string label)
        {
            GameObject section = new GameObject(name);
            section.transform.SetParent(parent, false);
            
            VerticalLayoutGroup layout = section.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 10;
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            
            // 标题
            GameObject title = new GameObject("Title");
            title.transform.SetParent(section.transform, false);
            TextMeshProUGUI titleText = title.AddComponent<TextMeshProUGUI>();
            titleText.text = label;
            titleText.fontSize = 18;
            titleText.fontStyle = FontStyles.Bold;
            titleText.color = new Color(0.8f, 0.8f, 1f);
            
            LayoutElement titleLayout = title.AddComponent<LayoutElement>();
            titleLayout.preferredHeight = 30;
            
            return section;
        }

        static void CreateInputField(Transform parent, string name, string placeholder, bool readOnly = false, float height = 30)
        {
            GameObject inputObj = new GameObject(name);
            inputObj.transform.SetParent(parent, false);
            
            Image bg = inputObj.AddComponent<Image>();
            bg.color = readOnly ? new Color(0.15f, 0.15f, 0.15f, 0.8f) : new Color(0.1f, 0.1f, 0.1f, 0.8f);
            
            TMP_InputField inputField = inputObj.AddComponent<TMP_InputField>();
            inputField.interactable = !readOnly;
            
            LayoutElement layoutElement = inputObj.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = height;
            layoutElement.minHeight = height;
            
            // Text
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(inputObj.transform, false);
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.fontSize = 14;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Left;
            
            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(10, 5);
            textRect.offsetMax = new Vector2(-10, -5);
            
            // Placeholder
            GameObject placeholderObj = new GameObject("Placeholder");
            placeholderObj.transform.SetParent(inputObj.transform, false);
            TextMeshProUGUI placeholderText = placeholderObj.AddComponent<TextMeshProUGUI>();
            placeholderText.text = placeholder;
            placeholderText.fontSize = 14;
            placeholderText.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
            placeholderText.alignment = TextAlignmentOptions.Left;
            
            RectTransform placeholderRect = placeholderText.GetComponent<RectTransform>();
            placeholderRect.anchorMin = Vector2.zero;
            placeholderRect.anchorMax = Vector2.one;
            placeholderRect.offsetMin = new Vector2(10, 5);
            placeholderRect.offsetMax = new Vector2(-10, -5);
            
            inputField.textComponent = text;
            inputField.placeholder = placeholderText;
        }

        static void CreateDropdown(Transform parent, string name, string label)
        {
            GameObject dropdownObj = new GameObject(name);
            dropdownObj.transform.SetParent(parent, false);
            
            Image bg = dropdownObj.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
            
            TMP_Dropdown dropdown = dropdownObj.AddComponent<TMP_Dropdown>();
            
            LayoutElement layoutElement = dropdownObj.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 35;
            layoutElement.minHeight = 35;
            
            // Label
            GameObject labelObj = new GameObject("Label");
            labelObj.transform.SetParent(dropdownObj.transform, false);
            TextMeshProUGUI labelText = labelObj.AddComponent<TextMeshProUGUI>();
            labelText.text = label;
            labelText.fontSize = 14;
            labelText.color = Color.white;
            labelText.alignment = TextAlignmentOptions.Left;
            
            RectTransform labelRect = labelText.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(10, 5);
            labelRect.offsetMax = new Vector2(-25, -5);
            
            dropdown.captionText = labelText;
            
            // Arrow
            GameObject arrow = new GameObject("Arrow");
            arrow.transform.SetParent(dropdownObj.transform, false);
            TextMeshProUGUI arrowText = arrow.AddComponent<TextMeshProUGUI>();
            arrowText.text = "▼";
            arrowText.fontSize = 12;
            arrowText.color = Color.white;
            arrowText.alignment = TextAlignmentOptions.Center;
            
            RectTransform arrowRect = arrowText.GetComponent<RectTransform>();
            arrowRect.anchorMin = new Vector2(1, 0);
            arrowRect.anchorMax = new Vector2(1, 1);
            arrowRect.pivot = new Vector2(1, 0.5f);
            arrowRect.sizeDelta = new Vector2(20, 0);
            arrowRect.anchoredPosition = new Vector2(-5, 0);
        }

        static void CreateStringListSection(Transform parent, string name, string label)
        {
            GameObject section = new GameObject(name);
            section.transform.SetParent(parent, false);
            
            VerticalLayoutGroup layout = section.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 5;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            
            // 标签
            GameObject labelObj = new GameObject("Label");
            labelObj.transform.SetParent(section.transform, false);
            TextMeshProUGUI labelText = labelObj.AddComponent<TextMeshProUGUI>();
            labelText.text = label;
            labelText.fontSize = 14;
            labelText.color = new Color(0.7f, 0.7f, 0.7f);
            
            LayoutElement labelLayout = labelObj.AddComponent<LayoutElement>();
            labelLayout.preferredHeight = 25;
            
            // ScrollView
            GameObject scrollView = new GameObject("ScrollView");
            scrollView.transform.SetParent(section.transform, false);
            
            RectTransform scrollRect = scrollView.AddComponent<RectTransform>();
            Image scrollBg = scrollView.AddComponent<Image>();
            scrollBg.color = new Color(0.05f, 0.05f, 0.05f, 0.8f);
            
            ScrollRect scroll = scrollView.AddComponent<ScrollRect>();
            scroll.horizontal = false;
            scroll.vertical = true;
            
            LayoutElement scrollLayout = scrollView.AddComponent<LayoutElement>();
            scrollLayout.preferredHeight = 150;
            scrollLayout.minHeight = 100;
            
            // Viewport
            GameObject viewport = new GameObject("Viewport");
            viewport.transform.SetParent(scrollView.transform, false);
            RectTransform viewportRect = viewport.AddComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = Vector2.zero;
            
            Mask mask = viewport.AddComponent<Mask>();
            mask.showMaskGraphic = false;
            Image maskImage = viewport.AddComponent<Image>();
            maskImage.color = Color.clear;
            
            // Content
            GameObject content = new GameObject("Content");
            content.transform.SetParent(viewport.transform, false);
            RectTransform contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.anchoredPosition = Vector2.zero;
            
            VerticalLayoutGroup contentLayout = content.AddComponent<VerticalLayoutGroup>();
            contentLayout.spacing = 5;
            contentLayout.padding = new RectOffset(5, 5, 5, 5);
            contentLayout.childControlWidth = true;
            contentLayout.childControlHeight = false;
            contentLayout.childForceExpandWidth = true;
            
            ContentSizeFitter contentFitter = content.AddComponent<ContentSizeFitter>();
            contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            scroll.viewport = viewportRect;
            scroll.content = contentRect;
            
            // AddButton
            CreateButton(section.transform, "AddButton", "+ 添加", new Color(0.2f, 0.5f, 0.2f));
        }

        static void CreateButtonRoot(Transform parent, string[] buttonNames, string[] buttonLabels, Color[] buttonColors)
        {
            GameObject buttonRoot = new GameObject("ButtonRoot");
            buttonRoot.transform.SetParent(parent, false);
            
            HorizontalLayoutGroup layout = buttonRoot.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 10;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            
            LayoutElement rootLayout = buttonRoot.AddComponent<LayoutElement>();
            rootLayout.preferredHeight = 50;
            rootLayout.minHeight = 50;
            
            for (int i = 0; i < buttonNames.Length; i++)
            {
                CreateButton(buttonRoot.transform, buttonNames[i], buttonLabels[i], buttonColors[i]);
            }
        }

        static GameObject CreateButton(Transform parent, string name, string label, Color? color = null)
        {
            GameObject button = new GameObject(name);
            button.transform.SetParent(parent, false);
            
            Button buttonComponent = button.AddComponent<Button>();
            Image buttonImage = button.AddComponent<Image>();
            buttonImage.color = color ?? new Color(0.2f, 0.2f, 0.2f);
            
            LayoutElement layoutElement = button.AddComponent<LayoutElement>();
            layoutElement.minHeight = 40;
            
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(button.transform, false);
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = label;
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 16;
            text.color = Color.white;
            
            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            return button;
        }
    }
}
#endif