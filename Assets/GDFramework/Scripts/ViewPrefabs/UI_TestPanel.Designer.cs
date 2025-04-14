using System;
using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;

namespace GDFramework.View
{
	// Generate Id:0d44ce71-64be-4667-b7e8-a58fbdf9b439
	public partial class UI_TestPanel
	{
		public const string Name = "UI_TestPanel";
		
		
		private UI_TestPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UI_TestPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UI_TestPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UI_TestPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
