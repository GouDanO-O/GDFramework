using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;
using GDFrameworkCore;
using Core.Game.Chunk.Universe.Data;
using System.Collections.Generic;
using TMPro;
using System.Linq;

namespace Core.Game.View
{
    public class UI_ChunkEditorPanelData : UIPanelData
    {
    }
    
    public partial class UI_ChunkEditorPanel : UIPanel, ICanGetModel
    {
        #region UI Components

        protected Transform ChunkEditorRoot;
        
        protected Transform HeaderRoot;
        protected Button UniverseEditorButton;
        protected Button WorldEditorButton;
        protected Button ExitButton;

        // 左侧列表区域
        protected Transform ListRoot;
        protected Transform UniverseListContent;
        protected ScrollRect UniverseListScrollView;
        
        // 右侧编辑区域
        protected Transform EditorRoot;
        protected Transform UniverseEditorRoot;
        
        // 宇宙编辑器组件
        protected TMP_InputField UniverseDefIdInput;
        protected TMP_InputField UniverseDefNameInput;
        protected TMP_InputField UniverseDefDescInput;
        protected TMP_InputField InitialPlayerWorldIdInput;
        protected Transform InitialShowingWorldListContent;
        protected Transform WorldIdListContent;
        protected Button AddInitialShowingWorldButton;
        protected Button AddWorldIdButton;
        protected Button SaveUniverseButton;
        protected Button CreateNewUniverseButton;
        protected Button DeleteUniverseButton;

        // 当前编辑状态
        private EditorMode _currentMode = EditorMode.Universe;
        private UniverseDtoDef _currentEditingUniverseDef;
        private UniverseDataModel _universeDataModel;
        
        // 预制体
        private GameObject _universeListItemPrefab;
        private GameObject _stringListItemPrefab;

        private enum EditorMode
        {
            Universe,
            World,
            Region,
            Dungeon,
            Room
        }
        #endregion

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UI_ChunkEditorPanelData ?? new UI_ChunkEditorPanelData();
            
            _universeDataModel = this.GetModel<UniverseDataModel>();
            
            GetRelyComponent();
            InitPrefabs();
            RegisterEvent();
        }

        protected override void GetRelyComponent()
        {
            base.GetRelyComponent();

            ChunkEditorRoot = Common.Find("ChunkEditorRoot");
            // Header
            HeaderRoot = ChunkEditorRoot.Find("HeaderRoot");
            UniverseEditorButton = HeaderRoot.Find("UniverseEditorButton").GetComponent<Button>();
            WorldEditorButton = HeaderRoot.Find("WorldEditorButton").GetComponent<Button>();
            ExitButton = HeaderRoot.Find("ExitButton").GetComponent<Button>();

            // 左侧列表
            ListRoot = ChunkEditorRoot.Find("ListRoot");
            UniverseListScrollView = ListRoot.Find("UniverseListScrollView").GetComponent<ScrollRect>();
            UniverseListContent = UniverseListScrollView.content;

            // 右侧编辑器
            EditorRoot = ChunkEditorRoot.Find("EditorRoot");
            UniverseEditorRoot = EditorRoot.Find("UniverseEditorRoot");

            // 宇宙编辑器组件
            var basicInfoRoot = UniverseEditorRoot.Find("BasicInfo");
            UniverseDefIdInput = basicInfoRoot.Find("DefIdInput").GetComponent<TMP_InputField>();
            UniverseDefNameInput = basicInfoRoot.Find("DefNameInput").GetComponent<TMP_InputField>();
            UniverseDefDescInput = basicInfoRoot.Find("DefDescInput").GetComponent<TMP_InputField>();
            
            var worldConfigRoot = UniverseEditorRoot.Find("WorldConfig");
            InitialPlayerWorldIdInput = worldConfigRoot.Find("InitialPlayerWorldIdInput").GetComponent<TMP_InputField>();
            
            var initialShowingRoot = worldConfigRoot.Find("InitialShowingWorldList");
            InitialShowingWorldListContent = initialShowingRoot.Find("Viewport/Content");
            AddInitialShowingWorldButton = initialShowingRoot.Find("AddButton").GetComponent<Button>();
            
            var worldIdListRoot = worldConfigRoot.Find("WorldIdList");
            WorldIdListContent = worldIdListRoot.Find("Viewport/Content");
            AddWorldIdButton = worldIdListRoot.Find("AddButton").GetComponent<Button>();

            var buttonRoot = UniverseEditorRoot.Find("ButtonRoot");
            SaveUniverseButton = buttonRoot.Find("SaveButton").GetComponent<Button>();
            CreateNewUniverseButton = buttonRoot.Find("CreateNewButton").GetComponent<Button>();
            DeleteUniverseButton = buttonRoot.Find("DeleteButton").GetComponent<Button>();
        }

        protected void InitPrefabs()
        {
            // 从Resources加载或从场景中获取预制体
            _universeListItemPrefab = Resources.Load<GameObject>("UI/Prefabs/UniverseListItem");
            _stringListItemPrefab = Resources.Load<GameObject>("UI/Prefabs/StringListItem");
            
            // 如果没有预制体,创建简单的默认预制体
            if (_universeListItemPrefab == null)
            {
                _universeListItemPrefab = CreateDefaultListItemPrefab();
            }
            if (_stringListItemPrefab == null)
            {
                _stringListItemPrefab = CreateDefaultStringListItemPrefab();
            }
        }

        protected override void RegisterEvent()
        {
            base.RegisterEvent();
            
            ExitButton.onClick.AddListener(ExitThisPanel);
            UniverseEditorButton.onClick.AddListener(() => SwitchEditorMode(EditorMode.Universe));
            WorldEditorButton.onClick.AddListener(() => SwitchEditorMode(EditorMode.World));
            
            // 宇宙编辑器事件
            SaveUniverseButton.onClick.AddListener(SaveCurrentUniverse);
            CreateNewUniverseButton.onClick.AddListener(CreateNewUniverse);
            DeleteUniverseButton.onClick.AddListener(DeleteCurrentUniverse);
            AddInitialShowingWorldButton.onClick.AddListener(() => AddStringToList(InitialShowingWorldListContent, ""));
            AddWorldIdButton.onClick.AddListener(() => AddStringToList(WorldIdListContent, ""));
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            SwitchEditorMode(EditorMode.Universe);
        }

        protected override void OnShow()
        {
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
            _currentEditingUniverseDef = null;
        }

        protected void ExitThisPanel()
        {
            this.CloseSelf();
        }

        #region Editor Mode Switching

        private void SwitchEditorMode(EditorMode mode)
        {
            _currentMode = mode;
            
            // 隐藏所有编辑器
            UniverseEditorRoot.gameObject.SetActive(false);
            // WorldEditorRoot.gameObject.SetActive(false);
            // ... 其他编辑器
            
            // 显示对应编辑器
            switch (mode)
            {
                case EditorMode.Universe:
                    ShowUniverseEditor();
                    break;
                case EditorMode.World:
                    // ShowWorldEditor();
                    Debug.Log("世界编辑器待实现");
                    break;
            }
        }

        #endregion

        #region Universe Editor

        private void ShowUniverseEditor()
        {
            UniverseEditorRoot.gameObject.SetActive(true);
            RefreshUniverseList();
        }

        private void RefreshUniverseList()
        {
            // 清空现有列表
            ClearChildren(UniverseListContent);

            // 获取所有宇宙配置
            var allUniverses = _universeDataModel.GetAllUniverseDefs();

            if (allUniverses == null || allUniverses.Count == 0)
            {
                Debug.Log("没有可用的宇宙配置");
                return;
            }

            // 创建列表项
            foreach (var universeDef in allUniverses)
            {
                CreateUniverseListItem(universeDef);
            }

            // 如果当前没有选中的,选中第一个
            if (_currentEditingUniverseDef == null && allUniverses.Count > 0)
            {
                SelectUniverse(allUniverses[0]);
            }
        }

        private void CreateUniverseListItem(UniverseDtoDef universeDef)
        {
            var itemObj = Instantiate(_universeListItemPrefab, UniverseListContent);
            
            // 设置显示文本
            var nameText = itemObj.transform.Find("NameText")?.GetComponent<TMP_Text>();
            if (nameText != null)
            {
                nameText.text = $"{universeDef.DefName}\n<size=12><color=#888888>{universeDef.DefId}</color></size>";
            }

            // 设置选中高亮
            var bgImage = itemObj.GetComponent<Image>();
            if (bgImage != null)
            {
                bgImage.color = _currentEditingUniverseDef == universeDef ? 
                    new Color(0.3f, 0.5f, 0.8f, 0.3f) : 
                    new Color(0.2f, 0.2f, 0.2f, 0.3f);
            }

            // 添加点击事件
            var button = itemObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => SelectUniverse(universeDef));
            }
        }

        private void SelectUniverse(UniverseDtoDef universeDef)
        {
            _currentEditingUniverseDef = universeDef;
            
            // 刷新列表高亮
            RefreshUniverseList();
            
            // 加载数据到编辑器
            LoadUniverseToEditor(universeDef);
        }

        private void LoadUniverseToEditor(UniverseDtoDef universeDef)
        {
            if (universeDef == null) return;

            // 基本信息
            UniverseDefIdInput.text = universeDef.DefId;
            UniverseDefNameInput.text = universeDef.DefName;
            UniverseDefDescInput.text = universeDef.DefDescription;
            InitialPlayerWorldIdInput.text = universeDef.InitialPlayerLocateWorldId ?? "";

            // DefId 设为只读
            UniverseDefIdInput.interactable = false;

            // 初始展示世界列表
            ClearChildren(InitialShowingWorldListContent);
            if (universeDef.InitialShowingWorldIdList != null)
            {
                foreach (var worldId in universeDef.InitialShowingWorldIdList)
                {
                    AddStringToList(InitialShowingWorldListContent, worldId);
                }
            }

            // 世界ID列表
            ClearChildren(WorldIdListContent);
            if (universeDef.WorldIdList != null)
            {
                foreach (var worldId in universeDef.WorldIdList)
                {
                    AddStringToList(WorldIdListContent, worldId);
                }
            }
        }

        private void SaveCurrentUniverse()
        {
            if (_currentEditingUniverseDef == null)
            {
                Debug.LogWarning("没有选中要保存的宇宙");
                return;
            }

            // 更新数据
            _currentEditingUniverseDef.DefName = UniverseDefNameInput.text;
            _currentEditingUniverseDef.DefDescription = UniverseDefDescInput.text;
            _currentEditingUniverseDef.InitialPlayerLocateWorldId = InitialPlayerWorldIdInput.text;

            // 更新列表
            _currentEditingUniverseDef.InitialShowingWorldIdList = GetStringListFromContent(InitialShowingWorldListContent);
            _currentEditingUniverseDef.WorldIdList = GetStringListFromContent(WorldIdListContent);

            // 保存到文件
            _currentEditingUniverseDef.SaveThisDef();

            Debug.Log($"保存宇宙配置: {_currentEditingUniverseDef.DefName}");
            
            // 刷新显示
            RefreshUniverseList();
        }

        private void CreateNewUniverse()
        {
            var newUniverse = new UniverseDtoDef
            {
                DefName = "新宇宙",
                DefDescription = "这是一个新的宇宙",
                InitialPlayerLocateWorldId = "",
                InitialShowingWorldIdList = new List<string>(),
                WorldIdList = new List<string>()
            };

            // 添加到数据模型
            _universeDataModel.AddDtoDef(newUniverse);

            // 保存到文件
            newUniverse.SaveThisDef();

            Debug.Log($"创建新宇宙: {newUniverse.DefName} ({newUniverse.DefId})");

            // 刷新列表并选中
            RefreshUniverseList();
            SelectUniverse(newUniverse);
        }

        private void DeleteCurrentUniverse()
        {
            if (_currentEditingUniverseDef == null)
            {
                Debug.LogWarning("没有选中要删除的宇宙");
                return;
            }

            // 确认对话框 (简化版,实际项目应该使用UI对话框)
            Debug.LogWarning($"准备删除宇宙: {_currentEditingUniverseDef.DefName}");
            
            // 删除文件
            _currentEditingUniverseDef.DeleteThisDef();

            // TODO: 从 DataModel 中移除 (需要在 UniverseDataModel 中添加 RemoveDtoDef 方法)
            
            _currentEditingUniverseDef = null;

            // 刷新列表
            RefreshUniverseList();
        }

        #endregion

        #region String List Helpers

        private void AddStringToList(Transform listContent, string value)
        {
            var itemObj = Instantiate(_stringListItemPrefab, listContent);
            
            var inputField = itemObj.transform.Find("InputField")?.GetComponent<TMP_InputField>();
            if (inputField != null)
            {
                inputField.text = value;
            }

            var deleteButton = itemObj.transform.Find("DeleteButton")?.GetComponent<Button>();
            if (deleteButton != null)
            {
                deleteButton.onClick.AddListener(() => Destroy(itemObj));
            }
        }

        private List<string> GetStringListFromContent(Transform listContent)
        {
            var result = new List<string>();
            
            for (int i = 0; i < listContent.childCount; i++)
            {
                var child = listContent.GetChild(i);
                var inputField = child.Find("InputField")?.GetComponent<TMP_InputField>();
                if (inputField != null && !string.IsNullOrEmpty(inputField.text))
                {
                    result.Add(inputField.text);
                }
            }

            return result;
        }

        #endregion

        #region Utility

        private void ClearChildren(Transform parent)
        {
            if (parent == null) return;
            
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }

        private GameObject CreateDefaultListItemPrefab()
        {
            var obj = new GameObject("UniverseListItem");
            
            var layout = obj.AddComponent<LayoutElement>();
            layout.minHeight = 60;

            var button = obj.AddComponent<Button>();
            var image = obj.AddComponent<Image>();
            image.color = new Color(0.2f, 0.2f, 0.2f, 0.3f);

            var textObj = new GameObject("NameText");
            textObj.transform.SetParent(obj.transform);
            var text = textObj.AddComponent<TMP_Text>();
            text.alignment = TextAlignmentOptions.Left;
            text.fontSize = 16;
            
            var rectTransform = textObj.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = new Vector2(10, 5);
            rectTransform.offsetMax = new Vector2(-10, -5);

            return obj;
        }

        private GameObject CreateDefaultStringListItemPrefab()
        {
            var obj = new GameObject("StringListItem");
            
            var layout = obj.AddComponent<LayoutElement>();
            layout.minHeight = 40;
            layout.preferredHeight = 40;

            var horizontalLayout = obj.AddComponent<HorizontalLayoutGroup>();
            horizontalLayout.childControlWidth = true;
            horizontalLayout.childControlHeight = true;
            horizontalLayout.childForceExpandWidth = true;
            horizontalLayout.childForceExpandHeight = false;
            horizontalLayout.spacing = 5;
            horizontalLayout.padding = new RectOffset(5, 5, 5, 5);

            // InputField
            var inputObj = new GameObject("InputField");
            inputObj.transform.SetParent(obj.transform);
            var inputField = inputObj.AddComponent<TMP_InputField>();
            var inputText = new GameObject("Text").AddComponent<TMP_Text>();
            inputText.transform.SetParent(inputObj.transform);
            inputField.textComponent = inputText;
            
            var inputBg = inputObj.AddComponent<Image>();
            inputBg.color = new Color(0.1f, 0.1f, 0.1f, 0.5f);

            // Delete Button
            var deleteObj = new GameObject("DeleteButton");
            deleteObj.transform.SetParent(obj.transform);
            var deleteButton = deleteObj.AddComponent<Button>();
            var deleteBg = deleteObj.AddComponent<Image>();
            deleteBg.color = new Color(0.8f, 0.2f, 0.2f, 0.8f);
            
            var deleteText = new GameObject("Text").AddComponent<TMP_Text>();
            deleteText.transform.SetParent(deleteObj.transform);
            deleteText.text = "X";
            deleteText.alignment = TextAlignmentOptions.Center;
            
            var deleteLayout = deleteObj.AddComponent<LayoutElement>();
            deleteLayout.preferredWidth = 40;

            return obj;
        }

        #endregion
    }
}