using System.Collections.Generic;
using Core.Game.Chunk.Universe.Data;
using Core.Game.Chunk.World.Data;
using Core.Game.View.Details;
using Cysharp.Threading.Tasks;
using GDFramework.FrameData;
using GDFramework.Resource;
using GDFramework.Utility;
using GDFrameworkCore;
using GDFrameworkExtend.ActionKit;
using GDFrameworkExtend.FluentAPI;
using GDFrameworkExtend.LogKit;
using GDFrameworkExtend.ResKit;
using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;
using TMPro;

namespace Core.Game.View
{
    public class UI_Editor_TotalPanelData : UIPanelData
    {
    }

    public partial class UI_Editor_TotalPanel : UIPanel, ICanGetModel,ICanGetUtility
    {
        #region UI Components

        protected Transform UniverseListContainer;
        
        // 左侧列表区域
        protected Transform LeftListRoot;
        protected ScrollRect UniverseListScrollView;
        protected Transform UniverseListContent;
        protected Button CreateNewUniverseButton;
        private List<GameObject> _universeListItems = new List<GameObject>();
        private GameObject _universeListItemPrefab;

        // 宇宙数据编辑区域
        protected Transform RightDetailRoot;
        private UI_EditorDetail_UniverseDetailShow _universeDetailShow;
        
        //宇宙星图编辑区域
        private UI_EditorDetail_UniverseMap _universeMap;

        //当前星图中的世界
        protected List<UI_EditorDetail_UniverseMapWorldNode> curUniverseMapWorldNodeList;

        // 操作按钮
        protected Transform OperationButtonRoot;
        protected Button SaveUniverseButton;
        protected Button EditCurrentWorldButton;
        protected Button ExitButton;

        #endregion

        private UniverseDataModel _universeDataModel;
        private UniverseDtoDef _currentSelectedUniverse;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UI_Editor_TotalPanelData ?? new UI_Editor_TotalPanelData();
            // please add init code here
            _universeDataModel = this.GetModel<UniverseDataModel>();

            GetRelyComponent(); 
            RegisterEvent();
        }

        protected override void GetRelyComponent()
        {
            base.GetRelyComponent();

            UniverseListContainer = Common.Find("UniverseListContainer");
            // 左侧列表
            LeftListRoot = UniverseListContainer.Find("LeftListRoot");
            _universeListItemPrefab = LeftListRoot.Find("EditorDetail_UniverseListItem").gameObject;
            
            UniverseListScrollView = LeftListRoot.Find("UniverseListScrollView").GetComponent<ScrollRect>();
            UniverseListContent = UniverseListScrollView.content;
            CreateNewUniverseButton = LeftListRoot.Find("CreateNewButton").GetComponent<Button>();

            //右侧 宇宙详细数据编辑面板
            RightDetailRoot = UniverseListContainer.Find("RightDetailRoot");
            _universeDetailShow = RightDetailRoot.Find("BasicInfoView").GetComponent<UI_EditorDetail_UniverseDetailShow>();

            _universeMap = RightDetailRoot.Find("UniverseWorldMap").GetComponent<UI_EditorDetail_UniverseMap>();
            
            // 操作按钮
            OperationButtonRoot = RightDetailRoot.Find("OperationButtons");
            SaveUniverseButton = OperationButtonRoot.Find("SaveButton").GetComponent<Button>();
            EditCurrentWorldButton = OperationButtonRoot.Find("EditCurrentWorldButton").GetComponent<Button>();
            ExitButton = OperationButtonRoot.Find("ExitButton").GetComponent<Button>();
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
            RefreshUniverseList();
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
                LogKit.Error("没有可用的宇宙配置");
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
            
            UI_EditorDetail_UniverseListItem curListItem = itemObj.GetComponent<UI_EditorDetail_UniverseListItem>();
            curListItem.SetThisUniverseData(universeDef);
            curListItem.Show();
        }

        /// <summary>
        /// 选择宇宙
        /// </summary>
        public void SelectUniverse(UniverseDtoDef universeDef)
        {
            if (universeDef == null) 
                return;
            _currentSelectedUniverse = universeDef;
            LoadUniverseToDetail(universeDef);
            ShowUniverseWorldMap();
        }

        /// <summary>
        /// 加载宇宙详情到右侧面板
        /// </summary>
        private void LoadUniverseToDetail(UniverseDtoDef universeDef)
        {
            _universeDetailShow.UpdateDetailShow(universeDef);
        }

        /// <summary>
        /// 显示宇宙星图
        /// </summary>
        private void ShowUniverseWorldMap()
        {
            if (_currentSelectedUniverse == null)
            {
                LogKit.Error("没有选中的宇宙");
                return;
            }
            
            _universeMap.ShowUniverseMap(_currentSelectedUniverse);
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

            LogKit.Log($"<color=green>✓ 创建新宇宙: {newUniverse.DefName} ({newUniverse.DefId})</color>");

            RefreshUniverseList();
        }

        /// <summary>
        /// 保存当前宇宙
        /// </summary>
        private void SaveCurrentUniverse()
        {
            if (_currentSelectedUniverse == null)
            {
                LogKit.Error("没有选中要保存的宇宙");
                return;
            }

            _currentSelectedUniverse.DefName = _universeDetailShow.GetUniverseName();
            _currentSelectedUniverse.DefDescription = _universeDetailShow.GetUniverseDesc();

            _currentSelectedUniverse.InitialPlayerLocateWorldId = _universeMap.GetCurInitialWorldDtoDef().DefId;
            _currentSelectedUniverse.InitialShowingWorldIdList.Clear();
            _currentSelectedUniverse.InitialShowingWorldIdList = _universeMap.GetCurIsLockingWorldDtoDefID(false);
            _currentSelectedUniverse.WorldIdList.Clear();
            _currentSelectedUniverse.WorldIdList = _universeMap.GetCurOwnedWorldDtoDefId();
            
            _currentSelectedUniverse.SaveThisDef();

            LogKit.Log($"<color=green>✓ 保存宇宙配置: {_currentSelectedUniverse.DefName}</color>");

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

        #region World

        /// <summary>
        /// 展开世界详情
        /// </summary>
        /// <param name="worldDtoDef"></param>
        public void OpenWorldDetail(WorldDtoDef worldDtoDef)
        {
            
        }

        #endregion
    }
}