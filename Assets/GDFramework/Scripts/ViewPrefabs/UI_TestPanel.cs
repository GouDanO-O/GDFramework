using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;

namespace GDFramework.View
{
	public class UI_TestPanelData : UIPanelData
	{
	}
	public partial class UI_TestPanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UI_TestPanelData ?? new UI_TestPanelData();
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
