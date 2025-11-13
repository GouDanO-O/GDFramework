using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;

namespace Core.Game.View
{
	public class TestRootData : UIPanelData
	{
	}
	public partial class TestRoot : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as TestRootData ?? new TestRootData();
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
