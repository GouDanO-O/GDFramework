using System;
using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;

namespace Core.Game.View
{
	// Generate Id:c95239e4-ae5e-40a6-bf1f-79026df31d2f
	public partial class UI_UniversePanel
	{
		public const string Name = "UI_UniversePanel";
		
		
		private UI_UniversePanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UI_UniversePanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UI_UniversePanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UI_UniversePanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
