using System.Collections.Generic;
using Core.Game.Chunk.Universe.Data;
using GDFramework.FrameData;
using GDFramework.Resource;
using GDFramework.Utility;
using GDFrameworkCore;
using GDFrameworkExtend.ActionKit;
using GDFrameworkExtend.ResKit;
using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;
using TMPro;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

namespace Core.Game.View
{
    public class UI_UniverseEditorListPanelData : UIPanelData
    {
    }

    public partial class UI_UniverseEditorListPanel : UIPanel, ICanGetModel,ICanGetUtility
    {
        #region UI Components

        protected Transform UniverseListContainer;
        
        // 左侧列表区域
        protected Transform LeftListRoot;
        protected ScrollRect UniverseListScrollView;
        protected Transform UniverseListContent;
        protected Button CreateNewUniverseButton;

        // 宇宙数据编辑区域
        protected Transform RightDetailRoot;
        protected Transform BasicInfoViewContent;
        protected TextMeshProUGUI UniverseIdText;
        protected TMP_InputField UniverseNameInput;
        protected TMP_InputField UniverseDescInput;
        
        //宇宙星图编辑区域
        protected Transform UniverseWorldMap;
        

        // 操作按钮
        protected Transform OperationButtonRoot;
        protected Button SaveUniverseButton;
        protected Button EditCurrentWorldButton;
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

            UniverseListContainer = Common.Find("UniverseListContainer");
            // 左侧列表
            LeftListRoot = UniverseListContainer.Find("LeftListRoot");
            UniverseListScrollView = LeftListRoot.Find("UniverseListScrollView").GetComponent<ScrollRect>();
            UniverseListContent = UniverseListScrollView.content;
            CreateNewUniverseButton = LeftListRoot.Find("CreateNewButton").GetComponent<Button>();

            //右侧 宇宙详细数据编辑面板
            RightDetailRoot = UniverseListContainer.Find("RightDetailRoot");
            BasicInfoViewContent = RightDetailRoot.Find("BasicInfoView").GetComponent<ScrollRect>().content;
            UniverseIdText = BasicInfoViewContent.Find("UniverseIdText/Text").GetComponent<TextMeshProUGUI>();
            UniverseNameInput = BasicInfoViewContent.Find("UniverseNameInput").GetComponent<TMP_InputField>();
            UniverseDescInput = BasicInfoViewContent.Find("UniverseDescInput").GetComponent<TMP_InputField>();

            UniverseWorldMap = RightDetailRoot.Find("UniverseWorldMap");
            

            // 操作按钮
            OperationButtonRoot = RightDetailRoot.Find("OperationButtons");
            SaveUniverseButton = OperationButtonRoot.Find("SaveButton").GetComponent<Button>();
            EditCurrentWorldButton = OperationButtonRoot.Find("EditCurrentWorldButton").GetComponent<Button>();
            ExitButton = OperationButtonRoot.Find("ExitButton").GetComponent<Button>();
        }

        protected async void InitPrefabs()
        {
            _universeListItemPrefab = await this.GetUtility<ResourcesUtility>()
                .LoadPrefabAsync(DefaultPackage.UIDetails.EditorDetailsAssetGroup.EditorDetail_UniverseListItem);
            
            _stringListItemPrefab = await this.GetUtility<ResourcesUtility>()
                .LoadPrefabAsync(DefaultPackage.UIDetails.EditorDetailsAssetGroup.EditorDetail_UniverseStringListItem);
        }

        protected override void RegisterEvent()
        {
            base.RegisterEvent();

            CreateNewUniverseButton.onClick.AddListener(CreateNewUniverse);
            SaveUniverseButton.onClick.AddListener(SaveCurrentUniverse);
            EditCurrentWorldButton.onClick.AddListener(EditCurrentWorld);
            ExitButton.onClick.AddListener(ExitPanel);
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            ActionKit.DelayFrame(1, () =>
            {
                RefreshUniverseList();
                RightDetailRoot.gameObject.SetActive(false); // 默认隐藏详情面板
            }).Start(this);

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
            if (universeDef == null) 
                return;

            UniverseIdText.text = universeDef.DefId;
            UniverseNameInput.text = universeDef.DefName;
            UniverseDescInput.text = universeDef.DefDescription;
        }

        /// <summary>
        /// 显示宇宙星图
        /// </summary>
        private void ShowUniverseWorldMap()
        {
            
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

            _currentSelectedUniverse.SaveThisDef();

            Debug.Log($"<color=green>✓ 保存宇宙配置: {_currentSelectedUniverse.DefName}</color>");

            RefreshUniverseList();
        }

        private void EditCurrentWorld()
        {
            
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
    }
}