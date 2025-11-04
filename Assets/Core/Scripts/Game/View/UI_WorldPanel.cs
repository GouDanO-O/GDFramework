using Core.Game.Chunk.World.Data;
using GDFrameworkExtend.ActionKit;
using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;

namespace Core.Game.View
{
	public class UI_WorldPanelData : UIPanelData
	{
		public WorldData CurSelectingWorld;

	}
	public partial class UI_WorldPanel : UIPanel
	{
		private Button ReturnBeforeButton;
		
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UI_WorldPanelData ?? new UI_WorldPanelData();
			// please add init code here
			
			//ReturnBeforeButton.onClick.AddListener(ReturnToBefore);
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

		private void ReturnToBefore()
		{
			CloseSelf();
		}
	}
}
