using System;
using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;

namespace Core.Game.View
{
	// Generate Id:c00434ef-3936-41ba-bbef-d3f3a28c16c4
	public partial class UI_Editor_RoomPanel
	{
		public const string Name = "UI_Editor_RoomPanel";
		
		
		private UI_Editor_RoomPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UI_Editor_RoomPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UI_Editor_RoomPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UI_Editor_RoomPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
