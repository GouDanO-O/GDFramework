using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;

namespace Core.Game.View
{
	public class UI_UniverseEditorPanelData : UIPanelData
	{
	}
	public partial class UI_UniverseEditorPanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UI_UniverseEditorPanelData ?? new UI_UniverseEditorPanelData();
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
		
		#region 宇宙编辑

		private void EditUniverseName(string willChangeName)
		{
			
		}

		private void ChangeUniversePosition(Vector2 position)
		{
			
		}

		/// <summary>
		/// 打开世界
		/// </summary>
		private void OpenUniverseWorld()
		{
			
		}

		private void ReturnToChunkEditor()
		{
			
		}
		
		#endregion
	}
}
