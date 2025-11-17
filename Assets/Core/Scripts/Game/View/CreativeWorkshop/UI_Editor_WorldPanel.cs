using Core.Game.View.Details;
using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;

namespace Core.Game.View
{
	public class UI_Editor_WorldPanelData : UIPanelData
	{
	}
	public partial class UI_Editor_WorldPanel : UIPanel
	{
		private UI_EditorDetail_WorldDetailShow _worldDetailShow;
		
		private UI_EditorDetail_WorldMap _worldMap;
		
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
			
		}

		protected override void RegisterEvent()
		{
			base.RegisterEvent();
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
	}
}
