#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

namespace Core.Game.View.Editor
{
    /// <summary>
    /// 宇宙列表面板UI生成器
    /// 一级面板：用于管理所有宇宙配置
    /// </summary>
    public class UniverseEditorListPanelSetup
    {
        [MenuItem("Tools/UI/Universe List Panel Content", false, 10)]
        static void CreateUniverseListPanelContent(MenuCommand menuCommand)
        {
            GameObject selectedObj = Selection.activeGameObject;
            if (selectedObj == null || selectedObj.name != "Common")
            {
                EditorUtility.DisplayDialog("提示", 
                    "请先选中 UI_UniverseListPanel 的 Common 节点！\n\n" +
                    "标准结构: UI_UniverseListPanel > Root > Common", 
                    "确定");
                return;
            }
            
            // 创建主容器
            GameObject mainContainer = new GameObject("UniverseListContainer");
            mainContainer.transform.SetParent(selectedObj.transform, false);
            
            RectTransform mainRect = mainContainer.AddComponent<RectTransform>();
            mainRect.anchorMin = Vector2.zero;
            mainRect.anchorMax = Vector2.one;
            mainRect.offsetMin = Vector2.zero;
            mainRect.offsetMax = Vector2.zero;
            
            // 创建左侧列表区域
            CreateLeftListRoot(mainContainer.transform);
            
            // 创建右侧详情区域
            CreateRightDetailRoot(mainContainer.transform);
            
            Selection.activeGameObject = mainContainer;
            EditorUtility.DisplayDialog("完成", "Universe List Panel Content 创建完成！", "确定");
        }

        #region Left List Root

        static void CreateLeftListRoot(Transform parent)
        {
            GameObject leftRoot = new GameObject("LeftListRoot");
            leftRoot.transform.SetParent(parent, false);
            
            RectTransform rect = leftRoot.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(400, 0);
            
            Image bg = leftRoot.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.1f, 1f);
            
            VerticalLayoutGroup layout = leftRoot.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 10;
            layout.padding = new RectOffset(10, 10, 10, 10);
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            
            // 标题
            GameObject title = new GameObject("Title");
            title.transform.SetParent(leftRoot.transform, false);
            TextMeshProUGUI titleText = title.AddComponent<TextMeshProUGUI>();
            titleText.text = "宇宙列表";
            titleText.fontSize = 20;
            titleText.fontStyle = FontStyles.Bold;
            titleText.color = new Color(0.8f, 0.8f, 1f);
            titleText.alignment = TextAlignmentOptions.Center;
            
            LayoutElement titleLayout = title.AddComponent<LayoutElement>();
            titleLayout.preferredHeight = 40;
            
            // 创建新宇宙按钮
            CreateButton(leftRoot.transform, "CreateNewButton", "➕ 创建新宇宙", new Color(0.2f, 0.6f, 0.2f));
            
            // 滚动列表
            GameObject scrollView = CreateScrollView(leftRoot.transform, "UniverseListScrollView");
            LayoutElement scrollLayout = scrollView.AddComponent<LayoutElement>();
            scrollLayout.flexibleHeight = 1;
        }

        #endregion

        #region Right Detail Root

        static void CreateRightDetailRoot(Transform parent)
        {
            GameObject rightRoot = new GameObject("RightDetailRoot");
            rightRoot.transform.SetParent(parent, false);
            
            RectTransform rect = rightRoot.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.offsetMin = new Vector2(410, 0);
            rect.offsetMax = new Vector2(0, 0);
            
            Image bg = rightRoot.AddComponent<Image>();
            bg.color = new Color(0.12f, 0.12f, 0.12f, 1f);
            
            // 创建可滚动的内容区域
            GameObject scrollView = new GameObject("DetailScrollView");
            scrollView.transform.SetParent(rightRoot.transform, false);
            
            RectTransform scrollRect = scrollView.AddComponent<RectTransform>();
            scrollRect.anchorMin = Vector2.zero;
            scrollRect.anchorMax = Vector2.one;
            scrollRect.offsetMin = Vector2.zero;
            scrollRect.offsetMax = Vector2.zero;
            
            ScrollRect scroll = scrollView.AddComponent<ScrollRect>();
            scroll.horizontal = false;
            scroll.vertical = true;
            
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
            contentLayout.spacing = 20;
            contentLayout.padding = new RectOffset(20, 20, 20, 20);
            contentLayout.childControlWidth = true;
            contentLayout.childControlHeight = false;
            contentLayout.childForceExpandWidth = true;
            
            ContentSizeFitter contentFitter = content.AddComponent<ContentSizeFitter>();
            contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            scroll.viewport = viewportRect;
            scroll.content = contentRect;
            
            // 创建详情内容
            CreateDetailContent(content.transform);
        }

        static void CreateDetailContent(Transform parent)
        {
            // 标题
            GameObject title = new GameObject("DetailTitle");
            title.transform.SetParent(parent, false);
            TextMeshProUGUI titleText = title.AddComponent<TextMeshProUGUI>();
            titleText.text = "宇宙详情";
            titleText.fontSize = 22;
            titleText.fontStyle = FontStyles.Bold;
            titleText.color = new Color(0.8f, 0.8f, 1f);
            titleText.alignment = TextAlignmentOptions.Center;
            
            LayoutElement titleLayout = title.AddComponent<LayoutElement>();
            titleLayout.preferredHeight = 50;
            
            // 基本信息区
            GameObject basicInfo = CreateSection(parent, "BasicInfo", "📋 基本信息");
            CreateInputField(basicInfo.transform, "UniverseIdText", "宇宙ID (只读)", true, 35);
            CreateInputField(basicInfo.transform, "UniverseNameInput", "宇宙名称", false, 35);
            CreateInputField(basicInfo.transform, "UniverseDescInput", "宇宙描述", false, 80);
            
            // 世界配置区
            GameObject worldConfig = CreateSection(parent, "WorldConfig", "🌍 世界配置");
            CreateInputField(worldConfig.transform, "InitialWorldIdInput", "初始玩家世界ID", false, 35);
            CreateStringListSection(worldConfig.transform, "InitialShowingWorldList", "初始展示世界列表");
            CreateStringListSection(worldConfig.transform, "AllWorldIdList", "所有世界ID列表");
            
            // 操作按钮区
            GameObject operationButtons = new GameObject("OperationButtons");
            operationButtons.transform.SetParent(parent, false);
            
            VerticalLayoutGroup btnLayout = operationButtons.AddComponent<VerticalLayoutGroup>();
            btnLayout.spacing = 10;
            btnLayout.childControlWidth = true;
            btnLayout.childControlHeight = false;
            btnLayout.childForceExpandWidth = true;
            
            LayoutElement btnRootLayout = operationButtons.AddComponent<LayoutElement>();
            btnRootLayout.preferredHeight = 230;
            
            CreateButton(operationButtons.transform, "SaveButton", "💾 保存修改", new Color(0.2f, 0.6f, 0.2f));
            CreateButton(operationButtons.transform, "DeleteButton", "🗑️ 删除宇宙", new Color(0.8f, 0.2f, 0.2f));
            CreateButton(operationButtons.transform, "EnterEditButton", "🎨 进入编辑模式", new Color(0.2f, 0.4f, 0.8f));
            CreateButton(operationButtons.transform, "ExitButton", "❌ 退出", new Color(0.4f, 0.4f, 0.4f));
        }

        #endregion

        #region Helper Methods

        static GameObject CreateScrollView(Transform parent, string name)
        {
            GameObject scrollView = new GameObject(name);
            scrollView.transform.SetParent(parent, false);
            
            RectTransform rect = scrollView.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            
            ScrollRect scrollRect = scrollView.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            
            Image scrollBg = scrollView.AddComponent<Image>();
            scrollBg.color = new Color(0.05f, 0.05f, 0.05f, 0.5f);
            
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
            
            return scrollView;
        }

        static GameObject CreateSection(Transform parent, string name, string label)
        {
            GameObject section = new GameObject(name);
            section.transform.SetParent(parent, false);
            
            VerticalLayoutGroup layout = section.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 10;
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
            titleText.color = new Color(0.7f, 0.8f, 1f);
            
            LayoutElement titleLayout = title.AddComponent<LayoutElement>();
            titleLayout.preferredHeight = 35;
            
            return section;
        }

        static void CreateInputField(Transform parent, string name, string placeholder, bool readOnly = false, float height = 35)
        {
            GameObject inputObj = new GameObject(name);
            inputObj.transform.SetParent(parent, false);
            
            Image bg = inputObj.AddComponent<Image>();
            bg.color = readOnly ? new Color(0.15f, 0.15f, 0.15f, 0.9f) : new Color(0.1f, 0.1f, 0.1f, 0.9f);
            
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
            scrollBg.color = new Color(0.05f, 0.05f, 0.05f, 0.9f);
            
            ScrollRect scroll = scrollView.AddComponent<ScrollRect>();
            scroll.horizontal = false;
            scroll.vertical = true;
            
            LayoutElement scrollLayout = scrollView.AddComponent<LayoutElement>();
            scrollLayout.preferredHeight = 120;
            scrollLayout.minHeight = 80;
            
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
            CreateButton(section.transform, "AddButton", "➕ 添加", new Color(0.2f, 0.5f, 0.2f));
        }

        static GameObject CreateButton(Transform parent, string name, string label, Color? color = null)
        {
            GameObject button = new GameObject(name);
            button.transform.SetParent(parent, false);
            
            Button buttonComponent = button.AddComponent<Button>();
            Image buttonImage = button.AddComponent<Image>();
            buttonImage.color = color ?? new Color(0.2f, 0.2f, 0.2f);
            
            LayoutElement layoutElement = button.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 45;
            layoutElement.minHeight = 45;
            
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

        #endregion
    }
}
#endif