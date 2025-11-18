using Core.Game.View.Details;
using GDFrameworkCore;
using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;

namespace Core.Game.View
{
	public class UI_Editor_WorldPanelData : UIPanelData
	{
	}
	public partial class UI_Editor_WorldPanel : UIPanel,ICanGetSystem
	{
		protected Transform DetailRoot;

		protected Transform OperationButtons;
		
		protected Button AddNewRegionButton;

		protected Button SaveButton;

		protected Button ExitButton;
		
		private UI_EditorDetail_WorldDetailShow _worldDetailShow;
		
		private UI_EditorDetail_WorldMap _worldMap;

		private EditorDataManager _editorDataManager;
		
		public IArchitecture GetArchitecture()
		{
			return GameMain.Interface;
		}
		
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UI_Editor_WorldPanelData ?? new UI_Editor_WorldPanelData();
			// please add init code here
			
			GetRelyComponent();
			RegisterEvent();
		}

		protected override void GetRelyComponent()
		{
			base.GetRelyComponent();
			DetailRoot = Common.Find("DetailRoot");
			_worldDetailShow = DetailRoot.Find("BasicInfoView").GetComponent<UI_EditorDetail_WorldDetailShow>();
			_worldMap = DetailRoot.Find("Map").GetComponent<UI_EditorDetail_WorldMap>();

			OperationButtons = Common.Find("OperationButtons");
			AddNewRegionButton = OperationButtons.Find("AddNewRegionButton").GetComponent<Button>();
			SaveButton = OperationButtons.Find("SaveButton").GetComponent<Button>();
			ExitButton = OperationButtons.Find("ExitButton").GetComponent<Button>();

			_editorDataManager = this.GetSystem<EditorDataManager>();
		}

		protected override void RegisterEvent()
		{
			base.RegisterEvent();
			
			AddNewRegionButton.onClick.AddListener(AddNewRegion);
			SaveButton.onClick.AddListener(SaveData);
			ExitButton.onClick.AddListener(ExitThis);
		}

		protected override void OnOpen(IUIData uiData = null)
		{
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
		/// 添加新区域
		/// </summary>
		private void AddNewRegion()
		{
			
		}

		/// <summary>
		/// 保存数据
		/// </summary>
		private void SaveData()
		{
			_editorDataManager.UpdateWorldTrackedSnapshots();
		}

		/// <summary>
		/// 退出面板
		/// </summary>
		private void ExitThis()
		{
			if (_editorDataManager.HasAnyChangeDidNotSave())
			{
				
			}
		}


	}
}
