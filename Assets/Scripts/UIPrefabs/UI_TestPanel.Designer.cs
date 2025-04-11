using System;
using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;

namespace GDFramework.Example
{
	// Generate Id:5e5ccfee-1a68-418e-8f92-d2cfd5ec6c3e
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
