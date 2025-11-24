using Core.Game.Chunk.Dungeon.Data;
using Core.Game.Chunk.Region.Data;
using Core.Game.View.Details;
using GDFrameworkCore;
using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;

namespace Core.Game.View
{
	public class UI_Editor_RegionPanelData : UIPanelData
	{
	}
	public partial class UI_Editor_RegionPanel : UIPanel,ICanGetSystem
	{
		protected Transform DetailRoot;

		protected Transform OperationButtons;
		
		protected Button AddNewDungeonButton;

		protected Button SaveButton;

		protected Button ExitButton;

		private UI_EditorDetail_RegionDetailShow _regionDetailShow;

		private UI_EditorDetail_RegionMap _regionMap;
		
		private RegionDtoDef _curFocusRegion;

		private EditorDataManager _editorDataManager;
		
		public IArchitecture GetArchitecture()
		{
			return GameMain.Interface;
		}
		
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UI_Editor_RegionPanelData ?? new UI_Editor_RegionPanelData();
			// please add init code here
			GetRelyComponent();
			RegisterEvent();
		}
		
		protected override void GetRelyComponent()
		{
			base.GetRelyComponent();
			DetailRoot = Common.Find("DetailRoot");
			_regionDetailShow = DetailRoot.Find("BasicInfoView").GetComponent<UI_EditorDetail_RegionDetailShow>();
			_regionMap = DetailRoot.Find("Map").GetComponent<UI_EditorDetail_RegionMap>();

			OperationButtons = DetailRoot.Find("OperationButtons");
			AddNewDungeonButton = OperationButtons.Find("AddNewDungeonButton").GetComponent<Button>();
			SaveButton = OperationButtons.Find("SaveButton").GetComponent<Button>();
			ExitButton = OperationButtons.Find("ExitButton").GetComponent<Button>();

			_editorDataManager = this.GetSystem<EditorDataManager>();
		}
		
		protected override void RegisterEvent()
		{
			base.RegisterEvent();
			
			AddNewDungeonButton.onClick.AddListener(AddNewDungeon);
			SaveButton.onClick.AddListener(SaveData);
			ExitButton.onClick.AddListener(ExitThis);
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
			RefreshRegionData();
		}
		
		protected override void OnShow()
		{
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
		}

		/// <summary>
		/// 刷新区域数据
		/// </summary>
		private void RefreshRegionData()
		{
			_curFocusRegion = _editorDataManager.GetFocusedRegion();
			_regionDetailShow.UpdateDetailShow(_curFocusRegion);
			_regionMap.ShowMap(_curFocusRegion);
		}
		
		/// <summary>
		/// 添加新副本
		/// </summary>
		private void AddNewDungeon()
		{
			DungeonDtoDef newDungeonDtoDef = _editorDataManager.AddNewDungeonToFocusRegion();
			_regionMap.AddMapNode(newDungeonDtoDef, _curFocusRegion.InitialPlayerLocateDungeonId,true);
		}

		/// <summary>
		/// 保存数据
		/// </summary>
		private void SaveData()
		{
			_editorDataManager.UpdateWorldTrackedSnapshots();
			_editorDataManager.UpdateRegionTrackedSnapshots();
		}

		/// <summary>
		/// 退出面板
		/// </summary>
		private void ExitThis()
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
						this.CloseSelf();
					},
					CancelAction = () =>
					{
						this.CloseSelf();
					}
				});
			}
		}
	}
}
