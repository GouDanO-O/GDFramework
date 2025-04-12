using System;
using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;

namespace GDFramework.View
{
	// Generate Id:a0c92853-a5e6-4aaf-8b04-4450cf72c558
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
