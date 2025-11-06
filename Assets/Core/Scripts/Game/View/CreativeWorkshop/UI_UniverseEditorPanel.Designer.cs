using System;
using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;

namespace Core.Game.View
{
	// Generate Id:06507b35-998a-4ef6-9487-bd1d8cd291e4
	public partial class UI_UniverseEditorPanel
	{
		public const string Name = "UI_UniverseEditorPanel";
		
		
		private UI_UniverseEditorPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UI_UniverseEditorPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UI_UniverseEditorPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UI_UniverseEditorPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
