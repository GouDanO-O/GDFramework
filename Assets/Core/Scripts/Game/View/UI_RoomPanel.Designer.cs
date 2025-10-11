using System;
using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;

namespace Core.Game.View
{
	// Generate Id:fb77aaff-be98-4432-9837-f07a53daea49
	public partial class UI_RoomPanel
	{
		public const string Name = "UI_RoomPanel";
		
		
		private UI_RoomPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UI_RoomPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UI_RoomPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UI_RoomPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
