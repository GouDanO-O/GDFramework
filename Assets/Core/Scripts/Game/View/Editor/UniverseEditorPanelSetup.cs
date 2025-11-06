#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

namespace Core.Game.View.Editor
{
    /// <summary>
    /// 宇宙编辑面板UI生成器
    /// 二级面板：基于 UI_UniversePanel 结构，用于编辑世界星图
    /// </summary>
    public class UniverseEditorPanelSetup
    {
        [MenuItem("Tools/UI/Universe Editor Panel Content", false, 11)]
        static void CreateUniverseEditorPanelContent(MenuCommand menuCommand)
        {
            GameObject selectedObj = Selection.activeGameObject;
            if (selectedObj == null || selectedObj.name != "Common")
            {
                EditorUtility.DisplayDialog("提示", 
                    "请先选中 UI_UniverseEditorPanel 的 Common 节点！\n\n" +
                    "标准结构: UI_UniverseEditorPanel > Root > Common", 
                    "确定");
                return;
            }
            
            // 创建 UniverseMap 主容器
            GameObject universeMap = new GameObject("UniverseMap");
            universeMap.transform.SetParent(selectedObj.transform, false);
            
            RectTransform mapRect = universeMap.AddComponent<RectTransform>();
            mapRect.anchorMin = Vector2.zero;
            mapRect.anchorMax = Vector2.one;
            mapRect.offsetMin = Vector2.zero;
            mapRect.offsetMax = Vector2.zero;
            
            // 创建各个区域
            CreateUniverseMapHeader(universeMap.transform);
            CreateUniverseMapCenter(universeMap.transform);
            CreateUniverseMapDowner(universeMap.transform);
            
            Selection.activeGameObject = universeMap;
            EditorUtility.DisplayDialog("完成", "Universe Editor Panel Content 创建完成！", "确定");
        }

        #region Header Area

        static void CreateUniverseMapHeader(Transform parent)
        {
            GameObject header = new GameObject("UniverseMapHeader");
            header.transform.SetParent(parent, false);
            
            RectTransform rect = header.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(0, 80);
            
            Image bg = header.AddComponent<Image>();
            bg.color = new Color(0.15f, 0.15f, 0.2f, 1f);
            
            HorizontalLayoutGroup layout = header.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 20;
            layout.padding = new RectOffset(20, 20, 15, 15);
            layout.childControlWidth = false;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;
            layout.childAlignment = TextAnchor.MiddleLeft;
            
            // 宇宙名称区域
            CreateUniverseNameArea(header.transform);
            
            // 间隔
            CreateSpacer(header.transform, 50);
            
            // 选中世界区域
            CreateFocusWorldArea(header.transform);
        }

        static void CreateUniverseNameArea(Transform parent)
        {
            GameObject nameArea = new GameObject("UniverseMapName");
            nameArea.transform.SetParent(parent, false);
            
            HorizontalLayoutGroup layout = nameArea.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 10;
            layout.childControlWidth = false;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;
            
            LayoutElement areaLayout = nameArea.AddComponent<LayoutElement>();
            areaLayout.preferredWidth = 400;
            
            // 标签
            GameObject label = new GameObject("Label");
            label.transform.SetParent(nameArea.transform, false);
            TextMeshProUGUI labelText = label.AddComponent<TextMeshProUGUI>();
            labelText.text = "🌌 宇宙:";
            labelText.fontSize = 18;
            labelText.fontStyle = FontStyles.Bold;
            labelText.color = new Color(0.8f, 0.8f, 1f);
            labelText.alignment = TextAlignmentOptions.Left;
            
            LayoutElement labelLayout = label.AddComponent<LayoutElement>();
            labelLayout.preferredWidth = 80;
            
            // 名称文本
            GameObject nameText = new GameObject("NameText");
            nameText.transform.SetParent(nameArea.transform, false);
            TextMeshProUGUI nameTextComp = nameText.AddComponent<TextMeshProUGUI>();
            nameTextComp.text = "未加载";
            nameTextComp.fontSize = 20;
            nameTextComp.fontStyle = FontStyles.Bold;
            nameTextComp.color = new Color(0.3f, 0.8f, 1f);
            nameTextComp.alignment = TextAlignmentOptions.Left;
            
            LayoutElement nameLayout = nameText.AddComponent<LayoutElement>();
            nameLayout.flexibleWidth = 1;
        }

        static void CreateFocusWorldArea(Transform parent)
        {
            GameObject focusArea = new GameObject("FocusWorld");
            focusArea.transform.SetParent(parent, false);
            
            HorizontalLayoutGroup layout = focusArea.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 10;
            layout.childControlWidth = false;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;
            
            LayoutElement areaLayout = focusArea.AddComponent<LayoutElement>();
            areaLayout.preferredWidth = 400;
            
            // 标签
            GameObject label = new GameObject("Label");
            label.transform.SetParent(focusArea.transform, false);
            TextMeshProUGUI labelText = label.AddComponent<TextMeshProUGUI>();
            labelText.text = "🎯 选中:";
            labelText.fontSize = 16;
            labelText.color = new Color(0.7f, 0.7f, 0.7f);
            labelText.alignment = TextAlignmentOptions.Left;
            
            LayoutElement labelLayout = label.AddComponent<LayoutElement>();
            labelLayout.preferredWidth = 80;
            
            // 世界名称
            GameObject worldName = new GameObject("WorldName");
            worldName.transform.SetParent(focusArea.transform, false);
            TextMeshProUGUI worldNameText = worldName.AddComponent<TextMeshProUGUI>();
            worldNameText.text = "未选择";
            worldNameText.fontSize = 18;
            worldNameText.color = new Color(0.3f, 1f, 0.3f);
            worldNameText.alignment = TextAlignmentOptions.Left;
            
            LayoutElement worldLayout = worldName.AddComponent<LayoutElement>();
            worldLayout.flexibleWidth = 1;
        }

        #endregion

        #region Center Area (World Map)

        static void CreateUniverseMapCenter(Transform parent)
        {
            GameObject center = new GameObject("UniverseMapCenter");
            center.transform.SetParent(parent, false);
            
            RectTransform rect = center.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.offsetMin = new Vector2(0, 140);
            rect.offsetMax = new Vector2(0, -80);
            
            Image bg = center.AddComponent<Image>();
            bg.color = new Color(0.05f, 0.05f, 0.08f, 1f);
            
            // 网格背景（可选）
            CreateGridBackground(center.transform);
            
            // 世界节点容器
            GameObject contents = new GameObject("Contents");
            contents.transform.SetParent(center.transform, false);
            
            RectTransform contentsRect = contents.AddComponent<RectTransform>();
            contentsRect.anchorMin = Vector2.zero;
            contentsRect.anchorMax = Vector2.one;
            contentsRect.offsetMin = Vector2.zero;
            contentsRect.offsetMax = Vector2.zero;
        }

        static void CreateGridBackground(Transform parent)
        {
            GameObject grid = new GameObject("GridBackground");
            grid.transform.SetParent(parent, false);
            
            RectTransform rect = grid.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            Image gridImage = grid.AddComponent<Image>();
            gridImage.color = new Color(0.1f, 0.1f, 0.15f, 0.3f);
            
            // 可以在这里添加网格线纹理
        }

        #endregion

        #region Downer Area (Editor Controls)

        static void CreateUniverseMapDowner(Transform parent)
        {
            GameObject downer = new GameObject("UniverseMapDowner");
            downer.transform.SetParent(parent, false);
            
            RectTransform rect = downer.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 0);
            rect.pivot = new Vector2(0.5f, 0);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(0, 140);
            
            Image bg = downer.AddComponent<Image>();
            bg.color = new Color(0.12f, 0.12f, 0.15f, 1f);
            
            VerticalLayoutGroup layout = downer.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 10;
            layout.padding = new RectOffset(20, 20, 15, 15);
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            
            // 控制面板
            CreateEditorControlPanel(downer.transform);
            
            // 操作按钮行
            CreateOperationButtons(downer.transform);
        }

        static void CreateEditorControlPanel(Transform parent)
        {
            GameObject controlPanel = new GameObject("EditorControlPanel");
            controlPanel.transform.SetParent(parent, false);
            
            HorizontalLayoutGroup layout = controlPanel.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 15;
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.childControlWidth = false;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
            layout.childAlignment = TextAnchor.MiddleLeft;
            
            LayoutElement panelLayout = controlPanel.AddComponent<LayoutElement>();
            panelLayout.preferredHeight = 50;
            
            // 网格大小控制
            CreateGridSizeControl(controlPanel.transform);
            
            // 网格吸附开关
            CreateGridSnapToggle(controlPanel.transform);
            
            // 间隔
            CreateSpacer(controlPanel.transform, 20);
            
            // 坐标显示
            CreateCoordinateDisplay(controlPanel.transform);
        }

        static void CreateGridSizeControl(Transform parent)
        {
            GameObject gridSizeGroup = new GameObject("GridSizeGroup");
            gridSizeGroup.transform.SetParent(parent, false);
            
            HorizontalLayoutGroup layout = gridSizeGroup.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 8;
            layout.childControlWidth = false;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
            
            LayoutElement groupLayout = gridSizeGroup.AddComponent<LayoutElement>();
            groupLayout.preferredWidth = 180;
            
            // 标签
            GameObject label = new GameObject("Label");
            label.transform.SetParent(gridSizeGroup.transform, false);
            TextMeshProUGUI labelText = label.AddComponent<TextMeshProUGUI>();
            labelText.text = "📐 网格大小:";
            labelText.fontSize = 14;
            labelText.color = Color.white;
            labelText.alignment = TextAlignmentOptions.Left;
            
            LayoutElement labelLayout = label.AddComponent<LayoutElement>();
            labelLayout.preferredWidth = 100;
            
            // 输入框
            GameObject input = new GameObject("GridSizeInput");
            input.transform.SetParent(gridSizeGroup.transform, false);
            
            Image inputBg = input.AddComponent<Image>();
            inputBg.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
            
            TMP_InputField inputField = input.AddComponent<TMP_InputField>();
            inputField.text = "50";
            inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
            
            LayoutElement inputLayout = input.AddComponent<LayoutElement>();
            inputLayout.preferredWidth = 70;
            
            // 输入框文本
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(input.transform, false);
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.fontSize = 14;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;
            
            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(5, 0);
            textRect.offsetMax = new Vector2(-5, 0);
            
            inputField.textComponent = text;
        }

        static void CreateGridSnapToggle(Transform parent)
        {
            GameObject toggleGroup = new GameObject("GridSnapGroup");
            toggleGroup.transform.SetParent(parent, false);
            
            HorizontalLayoutGroup layout = toggleGroup.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 8;
            layout.childControlWidth = false;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
            
            LayoutElement groupLayout = toggleGroup.AddComponent<LayoutElement>();
            groupLayout.preferredWidth = 150;
            
            // Toggle
            GameObject toggleObj = new GameObject("GridSnapToggle");
            toggleObj.transform.SetParent(toggleGroup.transform, false);
            
            Toggle toggle = toggleObj.AddComponent<Toggle>();
            toggle.isOn = true;
            
            Image toggleBg = toggleObj.AddComponent<Image>();
            toggleBg.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
            
            RectTransform toggleRect = toggleObj.GetComponent<RectTransform>();
            toggleRect.sizeDelta = new Vector2(35, 35);
            
            // Checkmark
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
            
            // 标签
            GameObject label = new GameObject("Label");
            label.transform.SetParent(toggleGroup.transform, false);
            TextMeshProUGUI labelText = label.AddComponent<TextMeshProUGUI>();
            labelText.text = "🧲 网格吸附";
            labelText.fontSize = 14;
            labelText.color = Color.white;
            labelText.alignment = TextAlignmentOptions.Left;
            
            LayoutElement labelLayout = label.AddComponent<LayoutElement>();
            labelLayout.preferredWidth = 100;
        }

        static void CreateCoordinateDisplay(Transform parent)
        {
            GameObject coordDisplay = new GameObject("CoordinateDisplay");
            coordDisplay.transform.SetParent(parent, false);
            
            TextMeshProUGUI coordText = coordDisplay.AddComponent<TextMeshProUGUI>();
            coordText.text = "📍 X: 0, Y: 0";
            coordText.fontSize = 14;
            coordText.color = new Color(0.7f, 0.7f, 0.7f);
            coordText.alignment = TextAlignmentOptions.Left;
            
            LayoutElement coordLayout = coordDisplay.AddComponent<LayoutElement>();
            coordLayout.preferredWidth = 150;
        }

        static void CreateOperationButtons(Transform parent)
        {
            GameObject buttonRoot = new GameObject("OperationButtons");
            buttonRoot.transform.SetParent(parent, false);
            
            HorizontalLayoutGroup layout = buttonRoot.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 10;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            
            LayoutElement rootLayout = buttonRoot.AddComponent<LayoutElement>();
            rootLayout.preferredHeight = 50;
            
            CreateButton(buttonRoot.transform, "SaveMapButton", "💾 保存地图", new Color(0.2f, 0.6f, 0.2f));
            CreateButton(buttonRoot.transform, "AddWorldButton", "➕ 添加世界", new Color(0.2f, 0.4f, 0.8f));
            CreateButton(buttonRoot.transform, "EditWorldDetailButton", "✏️ 编辑世界", new Color(0.6f, 0.4f, 0.8f));
            CreateButton(buttonRoot.transform, "ClearButton", "🗑️ 清空所有", new Color(0.8f, 0.4f, 0.2f));
            CreateButton(buttonRoot.transform, "BackToListButton", "◀️ 返回列表", new Color(0.4f, 0.4f, 0.4f));
        }

        #endregion

        #region Helper Methods

        static GameObject CreateButton(Transform parent, string name, string label, Color? color = null)
        {
            GameObject button = new GameObject(name);
            button.transform.SetParent(parent, false);
            
            Button buttonComponent = button.AddComponent<Button>();
            Image buttonImage = button.AddComponent<Image>();
            buttonImage.color = color ?? new Color(0.2f, 0.2f, 0.2f);
            
            LayoutElement layoutElement = button.AddComponent<LayoutElement>();
            layoutElement.minHeight = 45;
            
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(button.transform, false);
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = label;
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 15;
            text.color = Color.white;
            
            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            return button;
        }

        static void CreateSpacer(Transform parent, float width)
        {
            GameObject spacer = new GameObject("Spacer");
            spacer.transform.SetParent(parent, false);
            
            LayoutElement spacerLayout = spacer.AddComponent<LayoutElement>();
            spacerLayout.preferredWidth = width;
            spacerLayout.flexibleWidth = 0;
        }

        #endregion
    }
}
#endif