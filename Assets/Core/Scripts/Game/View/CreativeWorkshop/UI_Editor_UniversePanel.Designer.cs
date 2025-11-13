using System;
using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;

namespace Core.Game.View
{
	// Generate Id:3a4ef3c6-9f57-4e6b-aa0a-1b02c2d96962
	public partial class UI_Editor_UniversePanel
	{
		public const string Name = "UI_Editor_UniversePanel";
		
		
		private UI_Editor_UniversePanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UI_Editor_UniversePanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UI_Editor_UniversePanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UI_Editor_UniversePanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
