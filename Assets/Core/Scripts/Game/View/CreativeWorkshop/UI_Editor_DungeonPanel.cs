using Core.Game.Chunk.Dungeon.Data;
using Core.Game.Chunk.Room.Data;
using Core.Game.View.Details;
using GDFrameworkCore;
using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;

namespace Core.Game.View
{
	public class UI_Editor_DungeonPanelData : UIPanelData
	{
	}
	public partial class UI_Editor_DungeonPanel : UIPanel,ICanGetSystem
	{
		protected Transform DetailRoot;

		protected Transform OperationButtons;
		
		protected Button AddNewRoomButton;

		protected Button SaveButton;

		protected Button ExitButton;

		private UI_EditorDetail_DungeonDetailShow _dungeonDetailShow;

		private UI_EditorDetail_DungeonMap _dungeonMap;
		
		private DungeonDtoDef _curFocusDungeon;

		private EditorDataManager _editorDataManager;
		
		public IArchitecture GetArchitecture()
		{
			return GameMain.Interface;
		}
		
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UI_Editor_DungeonPanelData ?? new UI_Editor_DungeonPanelData();
			// please add init code here
			GetRelyComponent();
			RegisterEvent();
		}
		protected override void GetRelyComponent()
		{
			base.GetRelyComponent();
			DetailRoot = Common.Find("DetailRoot");
			_dungeonDetailShow = DetailRoot.Find("BasicInfoView").GetComponent<UI_EditorDetail_DungeonDetailShow>();
			_dungeonMap = DetailRoot.Find("Map").GetComponent<UI_EditorDetail_DungeonMap>();

			OperationButtons = DetailRoot.Find("OperationButtons");
			AddNewRoomButton = OperationButtons.Find("AddNewRoomButton").GetComponent<Button>();
			SaveButton = OperationButtons.Find("SaveButton").GetComponent<Button>();
			ExitButton = OperationButtons.Find("ExitButton").GetComponent<Button>();

			_editorDataManager = this.GetSystem<EditorDataManager>();
		}
		
		protected override void RegisterEvent()
		{
			base.RegisterEvent();
			
			AddNewRoomButton.onClick.AddListener(AddNewRoom);
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
			_curFocusDungeon = _editorDataManager.GetFocusedDungeon();
			_dungeonDetailShow.UpdateDetailShow(_curFocusDungeon);
			_dungeonMap.ShowMap(_curFocusDungeon);
		}
		
		/// <summary>
		/// 添加新副本
		/// </summary>
		private void AddNewRoom()
		{
			RoomDtoDef newRoomDtoDef = _editorDataManager.AddNewRoomToFocusDungeon();
			_dungeonMap.AddMapNode(newRoomDtoDef, _curFocusDungeon.InitialPlayerLocateRoomId,true);
		}

		/// <summary>
		/// 保存数据
		/// </summary>
		private void SaveData()
		{
			_editorDataManager.UpdateWorldTrackedSnapshots();
			_editorDataManager.UpdateRegionTrackedSnapshots();
			_editorDataManager.UpdateDungeonTrackedSnapshots();
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
