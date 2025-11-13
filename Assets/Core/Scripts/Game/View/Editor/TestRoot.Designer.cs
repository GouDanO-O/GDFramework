using System;
using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;

namespace Core.Game.View
{
	// Generate Id:b5e743bc-5c83-4299-b0ae-b2637509d105
	public partial class TestRoot
	{
		public const string Name = "TestRoot";
		
		
		private TestRootData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public TestRootData Data
		{
			get
			{
				return mData;
			}
		}
		
		TestRootData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new TestRootData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
