using System.Collections.Generic;
using Core.Game.Chunk.Universe.Data;
using GDFrameworkCore;
using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;
using TMPro;

namespace Core.Game.View
{
    public class UI_UniverseEditorListPanelData : UIPanelData
    {
    }

    public partial class UI_UniverseEditorListPanel : UIPanel, ICanGetModel
    {
        #region UI Components

        // 左侧列表区域
        protected Transform LeftListRoot;
        protected ScrollRect UniverseListScrollView;
        protected Transform UniverseListContent;
        protected Button CreateNewUniverseButton;

        // 右侧详情区域
        protected Transform RightDetailRoot;
        protected TMP_InputField UniverseIdText;
        protected TMP_InputField UniverseNameInput;
        protected TMP_InputField UniverseDescInput;
        protected TMP_InputField InitialWorldIdInput;
        protected Transform InitialShowingWorldListContent;
        protected Transform AllWorldIdListContent;
        protected Button AddInitialWorldButton;
        protected Button AddWorldIdButton;

        // 操作按钮
        protected Transform OperationButtonRoot;
        protected Button SaveUniverseButton;
        protected Button DeleteUniverseButton;
        protected Button EnterEditModeButton; // 进入二级编辑面板
        protected Button ExitButton;

        #endregion

        #region Private Fields

        private UniverseDataModel _universeDataModel;
        private UniverseDtoDef _currentSelectedUniverse;
        private List<GameObject> _universeListItems = new List<GameObject>();
        private GameObject _universeListItemPrefab;
        private GameObject _stringListItemPrefab;

        #endregion

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UI_UniverseEditorListPanelData ?? new UI_UniverseEditorListPanelData();
            // please add init code here
            _universeDataModel = this.GetModel<UniverseDataModel>();

            GetRelyComponent();
            InitPrefabs();
            RegisterEvent();
        }

        protected override void GetRelyComponent()
        {
            base.GetRelyComponent();

            // 左侧列表
            LeftListRoot = Common.Find("LeftListRoot");
            UniverseListScrollView = LeftListRoot.Find("UniverseListScrollView").GetComponent<ScrollRect>();
            UniverseListContent = UniverseListScrollView.content;
            CreateNewUniverseButton = LeftListRoot.Find("CreateNewButton").GetComponent<Button>();

            // 右侧详情
            RightDetailRoot = Common.Find("RightDetailRoot");

            var basicInfo = RightDetailRoot.Find("BasicInfo");
            UniverseIdText = basicInfo.Find("UniverseIdText").GetComponent<TMP_InputField>();
            UniverseNameInput = basicInfo.Find("UniverseNameInput").GetComponent<TMP_InputField>();
            UniverseDescInput = basicInfo.Find("UniverseDescInput").GetComponent<TMP_InputField>();

            var worldConfig = RightDetailRoot.Find("WorldConfig");
            InitialWorldIdInput = worldConfig.Find("InitialWorldIdInput").GetComponent<TMP_InputField>();

            var initialShowingList = worldConfig.Find("InitialShowingWorldList");
            InitialShowingWorldListContent = initialShowingList.Find("ScrollView/Viewport/Content");
            AddInitialWorldButton = initialShowingList.Find("AddButton").GetComponent<Button>();

            var allWorldList = worldConfig.Find("AllWorldIdList");
            AllWorldIdListContent = allWorldList.Find("ScrollView/Viewport/Content");
            AddWorldIdButton = allWorldList.Find("AddButton").GetComponent<Button>();

            // 操作按钮
            OperationButtonRoot = RightDetailRoot.Find("OperationButtons");
            SaveUniverseButton = OperationButtonRoot.Find("SaveButton").GetComponent<Button>();
            DeleteUniverseButton = OperationButtonRoot.Find("DeleteButton").GetComponent<Button>();
            EnterEditModeButton = OperationButtonRoot.Find("EnterEditButton").GetComponent<Button>();
            ExitButton = OperationButtonRoot.Find("ExitButton").GetComponent<Button>();
        }

        protected void InitPrefabs()
        {
            _universeListItemPrefab = Resources.Load<GameObject>("UI/Prefabs/UniverseListItem");
            _stringListItemPrefab = Resources.Load<GameObject>("UI/Prefabs/StringListItem");

            if (_universeListItemPrefab == null)
                _universeListItemPrefab = CreateDefaultUniverseListItemPrefab();
            if (_stringListItemPrefab == null)
                _stringListItemPrefab = CreateDefaultStringListItemPrefab();
        }

        protected override void RegisterEvent()
        {
            base.RegisterEvent();

            CreateNewUniverseButton.onClick.AddListener(CreateNewUniverse);
            SaveUniverseButton.onClick.AddListener(SaveCurrentUniverse);
            DeleteUniverseButton.onClick.AddListener(DeleteCurrentUniverse);
            EnterEditModeButton.onClick.AddListener(EnterUniverseEditMode);
            ExitButton.onClick.AddListener(ExitPanel);

            AddInitialWorldButton.onClick.AddListener(() => AddStringToList(InitialShowingWorldListContent, ""));
            AddWorldIdButton.onClick.AddListener(() => AddStringToList(AllWorldIdListContent, ""));
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            RefreshUniverseList();
            RightDetailRoot.gameObject.SetActive(false); // 默认隐藏详情面板
        }

        protected override void OnShow()
        {
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
            _currentSelectedUniverse = null;
            ClearUniverseListItems();
        }

        #region Universe List Management

        /// <summary>
        /// 刷新宇宙列表
        /// </summary>
        private void RefreshUniverseList()
        {
            ClearUniverseListItems();

            var allUniverses = _universeDataModel.GetAllUniverseDefs();

            if (allUniverses == null || allUniverses.Count == 0)
            {
                Debug.Log("没有可用的宇宙配置");
                return;
            }

            foreach (var universeDef in allUniverses)
            {
                CreateUniverseListItem(universeDef);
            }
        }

        /// <summary>
        /// 创建宇宙列表项
        /// </summary>
        private void CreateUniverseListItem(UniverseDtoDef universeDef)
        {
            var itemObj = Instantiate(_universeListItemPrefab, UniverseListContent);
            _universeListItems.Add(itemObj);

            var nameText = itemObj.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
            if (nameText != null)
            {
                nameText.text = $"<b>{universeDef.DefName}</b>\n" +
                                $"<size=12><color=#888888>{universeDef.DefId}</color></size>\n" +
                                $"<size=11><color=#666666>世界数: {universeDef.WorldIdList?.Count ?? 0}</color></size>";
            }

            var bgImage = itemObj.GetComponent<Image>();
            if (bgImage != null)
            {
                bgImage.color = _currentSelectedUniverse == universeDef
                    ? new Color(0.3f, 0.5f, 0.8f, 0.8f)
                    : new Color(0.2f, 0.2f, 0.2f, 0.5f);
            }

            var button = itemObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => SelectUniverse(universeDef));
            }
        }

        /// <summary>
        /// 选择宇宙
        /// </summary>
        private void SelectUniverse(UniverseDtoDef universeDef)
        {
            _currentSelectedUniverse = universeDef;
            RefreshUniverseList();
            LoadUniverseToDetail(universeDef);
            RightDetailRoot.gameObject.SetActive(true);
        }

        /// <summary>
        /// 加载宇宙详情到右侧面板
        /// </summary>
        private void LoadUniverseToDetail(UniverseDtoDef universeDef)
        {
            if (universeDef == null) return;

            UniverseIdText.text = universeDef.DefId;
            UniverseIdText.interactable = false;
            UniverseNameInput.text = universeDef.DefName;
            UniverseDescInput.text = universeDef.DefDescription;
            InitialWorldIdInput.text = universeDef.InitialPlayerLocateWorldId ?? "";

            // 初始显示世界列表
            ClearChildren(InitialShowingWorldListContent);
            if (universeDef.InitialShowingWorldIdList != null)
            {
                foreach (var worldId in universeDef.InitialShowingWorldIdList)
                {
                    AddStringToList(InitialShowingWorldListContent, worldId);
                }
            }

            // 所有世界ID列表
            ClearChildren(AllWorldIdListContent);
            if (universeDef.WorldIdList != null)
            {
                foreach (var worldId in universeDef.WorldIdList)
                {
                    AddStringToList(AllWorldIdListContent, worldId);
                }
            }
        }

        /// <summary>
        /// 清空列表项
        /// </summary>
        private void ClearUniverseListItems()
        {
            foreach (var item in _universeListItems)
            {
                if (item != null)
                    Destroy(item);
            }

            _universeListItems.Clear();
        }

        #endregion

        #region Universe Operations

        /// <summary>
        /// 创建新宇宙
        /// </summary>
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

            _universeDataModel.AddDtoDef(newUniverse);
            newUniverse.SaveThisDef();

            Debug.Log($"<color=green>✓ 创建新宇宙: {newUniverse.DefName} ({newUniverse.DefId})</color>");

            RefreshUniverseList();
            SelectUniverse(newUniverse);
        }

        /// <summary>
        /// 保存当前宇宙
        /// </summary>
        private void SaveCurrentUniverse()
        {
            if (_currentSelectedUniverse == null)
            {
                Debug.LogWarning("没有选中要保存的宇宙");
                return;
            }

            _currentSelectedUniverse.DefName = UniverseNameInput.text;
            _currentSelectedUniverse.DefDescription = UniverseDescInput.text;
            _currentSelectedUniverse.InitialPlayerLocateWorldId = InitialWorldIdInput.text;
            _currentSelectedUniverse.InitialShowingWorldIdList =
                GetStringListFromContent(InitialShowingWorldListContent);
            _currentSelectedUniverse.WorldIdList = GetStringListFromContent(AllWorldIdListContent);

            _currentSelectedUniverse.SaveThisDef();

            Debug.Log($"<color=green>✓ 保存宇宙配置: {_currentSelectedUniverse.DefName}</color>");

            RefreshUniverseList();
        }

        /// <summary>
        /// 删除当前宇宙
        /// </summary>
        private void DeleteCurrentUniverse()
        {
            if (_currentSelectedUniverse == null)
            {
                Debug.LogWarning("没有选中要删除的宇宙");
                return;
            }

            var defName = _currentSelectedUniverse.DefName;
            _currentSelectedUniverse.DeleteThisDef();

            Debug.Log($"<color=yellow>✗ 删除宇宙配置: {defName}</color>");

            _currentSelectedUniverse = null;
            RightDetailRoot.gameObject.SetActive(false);
            RefreshUniverseList();
        }

        /// <summary>
        /// 进入宇宙编辑模式（打开二级面板）
        /// </summary>
        private void EnterUniverseEditMode()
        {
            if (_currentSelectedUniverse == null)
            {
                Debug.LogWarning("请先选择一个宇宙");
                return;
            }

            // 打开宇宙编辑面板（二级面板）
            UIKit.OpenPanel<UI_UniverseEditorPanel>(new UI_UniverseEditorPanelData()
            {
                EditingUniverse = _currentSelectedUniverse
            });
        }

        /// <summary>
        /// 退出面板
        /// </summary>
        private void ExitPanel()
        {
            UIKit.OpenPanel<UI_GameMenuPanel>();
            this.CloseSelf();
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

        private void ClearChildren(Transform parent)
        {
            if (parent == null) return;

            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }

        #endregion

        #region Prefab Creation

        private GameObject CreateDefaultUniverseListItemPrefab()
        {
            var obj = new GameObject("UniverseListItem");

            var layout = obj.AddComponent<LayoutElement>();
            layout.minHeight = 80;
            layout.preferredHeight = 80;

            var button = obj.AddComponent<Button>();
            var image = obj.AddComponent<Image>();
            image.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);

            var textObj = new GameObject("NameText");
            textObj.transform.SetParent(obj.transform);
            var text = textObj.AddComponent<TextMeshProUGUI>();
            text.alignment = TextAlignmentOptions.Left;
            text.fontSize = 16;
            text.margin = new Vector4(10, 5, 10, 5);

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
            layout.minHeight = 35;
            layout.preferredHeight = 35;

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
            var inputText = new GameObject("Text").AddComponent<TextMeshProUGUI>();
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

            var deleteText = new GameObject("Text").AddComponent<TextMeshProUGUI>();
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