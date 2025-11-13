using System;
using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;

namespace Core.Game.View
{
	// Generate Id:003d37e5-07f5-465c-869f-dbc746934344
	public partial class UI_Editor_WorldPanel
	{
		public const string Name = "UI_Editor_WorldPanel";
		
		
		private UI_Editor_WorldPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UI_Editor_WorldPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UI_Editor_WorldPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UI_Editor_WorldPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
