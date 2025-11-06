using System;
using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;

namespace Core.Game.View
{
	// Generate Id:6730435d-cec9-4f49-9b10-7939e5d09342
	public partial class UI_WorldPanel
	{
		public const string Name = "UI_WorldPanel";
		
		
		private UI_WorldPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UI_WorldPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UI_WorldPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UI_WorldPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
