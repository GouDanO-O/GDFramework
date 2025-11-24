using System;
using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;

namespace Core.Game.View
{
	// Generate Id:437834e8-8b9e-47aa-aeb6-0b54cb347455
	public partial class UI_Editor_DungeonPanel
	{
		public const string Name = "UI_Editor_DungeonPanel";
		
		
		private UI_Editor_DungeonPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UI_Editor_DungeonPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UI_Editor_DungeonPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UI_Editor_DungeonPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
