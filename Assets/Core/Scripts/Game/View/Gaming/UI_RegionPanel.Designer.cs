using System;
using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;

namespace Core.Game.View
{
	// Generate Id:83fa5b97-20c6-4141-8e09-0b179f3741d7
	public partial class UI_RegionPanel
	{
		public const string Name = "UI_RegionPanel";
		
		
		private UI_RegionPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UI_RegionPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UI_RegionPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UI_RegionPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
