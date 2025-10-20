using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;

namespace Core.Game.View
{
	public class UI_ChunkEditorPanelData : UIPanelData
	{
	}
	
	public partial class UI_ChunkEditorPanel : UIPanel
	{
		protected Transform HeaderRoot;
		
		protected Button ExitButton;
		
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UI_ChunkEditorPanelData ?? new UI_ChunkEditorPanelData();
			// please add init code here
			GetRelyComponent();
			RegisterEvent();
		}

		protected override void GetRelyComponent()
		{
			base.GetRelyComponent();

			HeaderRoot = Top.Find("HeaderRoot");
			ExitButton = HeaderRoot.Find("ExitButton").GetComponent<Button>();
		}

		protected override void RegisterEvent()
		{
			base.RegisterEvent();
			ExitButton.onClick.AddListener(ExitThisPanel);
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

		protected void ExitThisPanel()
		{
			this.CloseSelf();
		}


	}
}
