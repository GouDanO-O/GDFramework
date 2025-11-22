using System;
using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;

namespace Core.Game.View
{
	// Generate Id:3762b0eb-b9bc-4e38-a1da-091abf5d4327
	public partial class UI_Editor_RegionPanel
	{
		public const string Name = "UI_Editor_RegionPanel";
		
		
		private UI_Editor_RegionPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UI_Editor_RegionPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UI_Editor_RegionPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UI_Editor_RegionPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
