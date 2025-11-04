using System;
using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;

namespace Core.Game.View
{
	// Generate Id:e1471d8e-8308-4105-9682-96767bc771a7
	public partial class UI_ChunkEditorPanel
	{
		public const string Name = "UI_ChunkEditorPanel";
		
		
		private UI_ChunkEditorPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public UI_ChunkEditorPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UI_ChunkEditorPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UI_ChunkEditorPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
