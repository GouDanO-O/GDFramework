using System;
using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;

namespace Core.Game.View
{
	// Generate Id:3a4ef3c6-9f57-4e6b-aa0a-1b02c2d96962
	public partial class UI_Editor_TotalPanel
	{
		public const string Name = "UI_Editor_TotalPanel";
		
		
		private UI_Editor_TotalPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UI_Editor_TotalPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UI_Editor_TotalPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UI_Editor_TotalPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
