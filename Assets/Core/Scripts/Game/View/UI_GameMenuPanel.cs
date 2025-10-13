using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;

namespace Core.Game.View
{
	public class UI_GameMenuPanelData : UIPanelData
	{
	}
	public partial class UI_GameMenuPanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UI_GameMenuPanelData ?? new UI_GameMenuPanelData();
			// please add init code here
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
