#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

namespace Core.Game.View.Editor
{
    /// <summary>
    /// Chunk编辑器快速搭建工具
    /// 在编辑器中右键 GameObject > UI > Chunk Editor Panel 即可创建
    /// </summary>
    public class ChunkEditorQuickSetup
    {
        [MenuItem("GameObject/UI/Chunk Editor Panel", false, 10)]
        static void CreateChunkEditorPanel(MenuCommand menuCommand)
        {
            // 创建Canvas
            GameObject canvas = new GameObject("UI_ChunkEditorPanel");
            Canvas canvasComponent = canvas.AddComponent<Canvas>();
            canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<CanvasScaler>();
            canvas.AddComponent<GraphicRaycaster>();

            // 创建背景
            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(canvas.transform, false);
            Image bgImage = bg.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
            RectTransform bgRect = bg.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            // 创建Header
            CreateHeader(canvas.transform);

            // 创建左侧列表
            CreateListRoot(canvas.transform);

            // 创建右侧编辑器
            CreateEditorRoot(canvas.transform);

            // 选中创建的对象
            Selection.activeGameObject = canvas;
            Debug.Log("Chunk Editor Panel 创建完成!");
        }

        static void CreateHeader(Transform parent)
        {
            GameObject header = new GameObject("HeaderRoot");
            header.transform.SetParent(parent, false);
            RectTransform headerRect = header.AddComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0, 1);
            headerRect.anchorMax = new Vector2(1, 1);
            headerRect.pivot = new Vector2(0.5f, 1);
            headerRect.anchoredPosition = Vector2.zero;
            headerRect.sizeDelta = new Vector2(0, 60);

            HorizontalLayoutGroup layout = header.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 10;
            layout.padding = new RectOffset(10, 10, 10, 10);
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            CreateButton(header.transform, "UniverseEditorButton", "宇宙编辑器");
            CreateButton(header.transform, "WorldEditorButton", "世界编辑器");
            
            // 添加弹簧占位
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
            listRect.sizeDelta = new Vector2(300, -60);
            listRect.offsetMin = new Vector2(0, 0);
            listRect.offsetMax = new Vector2(300, -60);

            // 创建ScrollView
            GameObject scrollView = CreateScrollView(listRoot.transform, "UniverseListScrollView");
        }

        static void CreateEditorRoot(Transform parent)
        {
            GameObject editorRoot = new GameObject("EditorRoot");
            editorRoot.transform.SetParent(parent, false);
            RectTransform editorRect = editorRoot.AddComponent<RectTransform>();
            editorRect.anchorMin = new Vector2(0, 0);
            editorRect.anchorMax = new Vector2(1, 1);
            editorRect.offsetMin = new Vector2(310, 0);
            editorRect.offsetMax = new Vector2(0, -60);

            // 创建宇宙编辑器
            CreateUniverseEditor(editorRoot.transform);
        }

        static void CreateUniverseEditor(Transform parent)
        {
            GameObject universeEditor = new GameObject("UniverseEditorRoot");
            universeEditor.transform.SetParent(parent, false);
            RectTransform rect = universeEditor.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            VerticalLayoutGroup layout = universeEditor.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 20;
            layout.padding = new RectOffset(20, 20, 20, 20);
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;

            // 基本信息区域
            GameObject basicInfo = CreateSection(universeEditor.transform, "BasicInfo", "基本信息");
            CreateInputField(basicInfo.transform, "DefIdInput", "配置ID (只读)", true);
            CreateInputField(basicInfo.transform, "DefNameInput", "配置名称");
            CreateInputField(basicInfo.transform, "DefDescInput", "配置描述", false, 60);

            // 世界配置区域
            GameObject worldConfig = CreateSection(universeEditor.transform, "WorldConfig", "世界配置");
            CreateInputField(worldConfig.transform, "InitialPlayerWorldIdInput", "初始玩家世界ID");
            CreateStringList(worldConfig.transform, "InitialShowingWorldList", "初始展示世界列表");
            CreateStringList(worldConfig.transform, "WorldIdList", "所有世界ID列表");

            // 按钮区域
            GameObject buttonRoot = new GameObject("ButtonRoot");
            buttonRoot.transform.SetParent(universeEditor.transform, false);
            HorizontalLayoutGroup buttonLayout = buttonRoot.AddComponent<HorizontalLayoutGroup>();
            buttonLayout.spacing = 10;
            buttonLayout.childControlWidth = true;
            buttonLayout.childControlHeight = true;
            buttonLayout.childForceExpandWidth = true;

            CreateButton(buttonRoot.transform, "SaveButton", "保存", new Color(0.2f, 0.6f, 0.2f));
            CreateButton(buttonRoot.transform, "CreateNewButton", "创建新宇宙", new Color(0.2f, 0.4f, 0.8f));
            CreateButton(buttonRoot.transform, "DeleteButton", "删除", new Color(0.8f, 0.2f, 0.2f));
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

            // 添加标题
            GameObject title = new GameObject("Title");
            title.transform.SetParent(section.transform, false);
            TextMeshProUGUI titleText = title.AddComponent<TextMeshProUGUI>();
            titleText.text = label;
            titleText.fontSize = 18;
            titleText.fontStyle = FontStyles.Bold;
            titleText.color = new Color(0.8f, 0.8f, 1f);

            return section;
        }

        static GameObject CreateInputField(Transform parent, string name, string placeholder, 
            bool readOnly = false, float height = 30)
        {
            GameObject inputObj = new GameObject(name);
            inputObj.transform.SetParent(parent, false);
            
            TMP_InputField inputField = inputObj.AddComponent<TMP_InputField>();
            Image bg = inputObj.AddComponent<Image>();
            bg.color = readOnly ? new Color(0.15f, 0.15f, 0.15f, 0.5f) : new Color(0.1f, 0.1f, 0.1f, 0.8f);

            LayoutElement layoutElement = inputObj.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = height;

            // Text组件
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(inputObj.transform, false);
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.fontSize = 14;
            text.color = Color.white;
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
            placeholderText.color = new Color(0.5f, 0.5f, 0.5f);
            RectTransform placeholderRect = placeholderText.GetComponent<RectTransform>();
            placeholderRect.anchorMin = Vector2.zero;
            placeholderRect.anchorMax = Vector2.one;
            placeholderRect.offsetMin = new Vector2(10, 5);
            placeholderRect.offsetMax = new Vector2(-10, -5);

            inputField.textComponent = text;
            inputField.placeholder = placeholderText;
            inputField.interactable = !readOnly;

            return inputObj;
        }

        static GameObject CreateStringList(Transform parent, string name, string label)
        {
            GameObject listObj = new GameObject(name);
            listObj.transform.SetParent(parent, false);
            
            VerticalLayoutGroup layout = listObj.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 5;
            layout.childControlWidth = true;
            layout.childControlHeight = false;

            // 标签
            GameObject labelObj = new GameObject("Label");
            labelObj.transform.SetParent(listObj.transform, false);
            TextMeshProUGUI labelText = labelObj.AddComponent<TextMeshProUGUI>();
            labelText.text = label;
            labelText.fontSize = 14;
            labelText.color = new Color(0.7f, 0.7f, 0.7f);

            // ScrollView
            GameObject scrollView = CreateScrollView(listObj.transform, "ScrollView", 150);

            // 添加按钮
            CreateButton(listObj.transform, "AddButton", "+ 添加", new Color(0.2f, 0.5f, 0.2f));

            return listObj;
        }

        static GameObject CreateScrollView(Transform parent, string name, float height = -1)
        {
            GameObject scrollView = new GameObject(name);
            scrollView.transform.SetParent(parent, false);
            
            ScrollRect scrollRect = scrollView.AddComponent<ScrollRect>();
            Image scrollBg = scrollView.AddComponent<Image>();
            scrollBg.color = new Color(0.05f, 0.05f, 0.05f, 0.8f);

            if (height > 0)
            {
                LayoutElement layoutElement = scrollView.AddComponent<LayoutElement>();
                layoutElement.preferredHeight = height;
            }

            // Viewport
            GameObject viewport = new GameObject("Viewport");
            viewport.transform.SetParent(scrollView.transform, false);
            Mask mask = viewport.AddComponent<Mask>();
            Image viewportImage = viewport.AddComponent<Image>();
            viewportImage.color = Color.clear;
            RectTransform viewportRect = viewport.GetComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = Vector2.zero;

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

            ContentSizeFitter contentFitter = content.AddComponent<ContentSizeFitter>();
            contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scrollRect.viewport = viewportRect;
            scrollRect.content = contentRect;
            scrollRect.vertical = true;
            scrollRect.horizontal = false;

            return scrollView;
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