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
    public class UI_Editor_UniversePanelData : UIPanelData
    {
        
    }

    public partial class UI_Editor_UniversePanel : UIPanel, ICanGetModel,ICanGetUtility,ICanGetSystem
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

        // 操作按钮
        protected Transform OperationButtonRoot;
        protected Button AddNewWorldButton;
        protected Button SaveButton;
        protected Button ExitButton;

        #endregion

        private EditorDataManager _editorDataManager;
        
        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UI_Editor_UniversePanelData ?? new UI_Editor_UniversePanelData();
            // please add init code here

            _editorDataManager = this.GetSystem<EditorDataManager>();
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

            _universeMap = RightDetailRoot.Find("Map").GetComponent<UI_EditorDetail_UniverseMap>();
            
            // 操作按钮
            OperationButtonRoot = RightDetailRoot.Find("OperationButtons");
            AddNewWorldButton = OperationButtonRoot.Find("AddNewWorldButton").GetComponent<Button>();
            SaveButton = OperationButtonRoot.Find("SaveButton").GetComponent<Button>();
            ExitButton = OperationButtonRoot.Find("ExitButton").GetComponent<Button>();
        }
        

        protected override void RegisterEvent()
        {
            base.RegisterEvent();

            CreateNewUniverseButton.onClick.AddListener(CreateNewUniverse);
            AddNewWorldButton.onClick.AddListener(CreateNewWorld);
            SaveButton.onClick.AddListener(SaveData);
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
            CloseTotalPanel();
        }

        private void CloseTotalPanel()
        {
            _editorDataManager.ClearEditorData();
            ClearUniverseListItems();
        }
        
        #region Universe

         /// <summary>
        /// 刷新宇宙列表
        /// </summary>
        private void RefreshUniverseList()
        {
            ClearUniverseListItems();

            var allUniverses = _editorDataManager.GetAllUniverseDefs();

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

            if (_editorDataManager.HasAnyChangeDidNotSave())
            {
                UIKit.OpenPanel<UI_TipsWindow>(UILevel.PopUI,new UI_TipsWindowData()
                {
                    TipsString = $"当前有未保存的数据\n{_editorDataManager.GetChangeSummary()}",
                    CancelString = "取消",
                    SureString = "保存并打开",
                    SureAction = () =>
                    {
                        SaveData();
                        _editorDataManager.UpdateFocusUniverse(universeDef);
                        LoadUniverseToDetail(universeDef);
                        ShowUniverseWorldMap(universeDef);
                    }
                });
            }
            else
            {
                _editorDataManager.UpdateFocusUniverse(universeDef);
                LoadUniverseToDetail(universeDef);
                ShowUniverseWorldMap(universeDef);
            }
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
        private void ShowUniverseWorldMap(UniverseDtoDef universeDef)
        {
            _universeMap.ShowMap(universeDef);
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
        
        /// <summary>
        /// 创建新宇宙
        /// </summary>
        private void CreateNewUniverse()
        {
            if (_editorDataManager.HasAnyChangeDidNotSave())
            {
                UIKit.OpenPanel<UI_TipsWindow>(UILevel.PopUI,new UI_TipsWindowData()
                {
                    TipsString = $"当前有未保存的数据\n{_editorDataManager.GetChangeSummary()}",
                    CancelString = "取消创建",
                    SureString = "保存并创建",
                    SureAction = () =>
                    {
                        SaveData();
                        CreateNewUniverseInternal();
                    }
                });
            }
            else
            {
                CreateNewUniverseInternal();
            }
        }

        private void CreateNewUniverseInternal()
        {
            if (_editorDataManager.GetFocusedUniverse() == null)
            {
                SelectUniverse(_editorDataManager.AddNewUniverseDtoDef());
            }
            else
            {
                _editorDataManager.AddNewUniverseDtoDef();
            }
            
            RefreshUniverseList();
        }
        
        /// <summary>
        /// 退出面板
        /// </summary>
        private void ExitPanel()
        {
            if (_editorDataManager.HasAnyChangeDidNotSave())
            {
                UIKit.OpenPanel<UI_TipsWindow>(UILevel.PopUI,new UI_TipsWindowData()
                {
                    TipsString = "当前有未保存的数据",
                    CancelString = "不保存就退出",
                    SureString = "保存并退出",
                    SureAction = () =>
                    {
                        SaveData();
                        UIKit.OpenPanel<UI_GameMenuPanel>();
                        this.CloseSelf();
                    },
                    CancelAction = () =>
                    {
                        UIKit.OpenPanel<UI_GameMenuPanel>();
                        this.CloseSelf();
                    }
                });
            }
            else
            {
                UIKit.OpenPanel<UI_GameMenuPanel>();
                this.CloseSelf();
            }

        }

        public string GetCurUniverseName()
        {
            return _universeDetailShow.GetDefName();
        }

        public string GetCurUniverseDes()
        {
            return _universeDetailShow.GetDefDesc();
        }

        #endregion

        #region World

        /// <summary>
        /// 创建一个新世界
        /// </summary>
        public void CreateNewWorld()
        {
            if (_editorDataManager.GetFocusedUniverse() == null)
            {
                LogKit.Error("请先选择一个宇宙");
                return;
            }

            
            _universeMap.AddMapNode(_editorDataManager.AddNewWorldToFocusUniverse(),
                _editorDataManager.GetFocusedUniverse().InitialPlayerLocateWorldId);
        }

        //TODO 从配置中加载一个已经创建的世界
        /// <summary>
        /// 从配置中加载一个已经创建的世界
        /// </summary>
        public void LoadExistWorldDtoDefForCreate()
        {
            
        }
        
        #endregion

        private void SaveData()
        {
            _editorDataManager.UpdateAllTrackedSnapshots();
        }
    }
}