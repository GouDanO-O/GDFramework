using System;
using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;

namespace Core.Game.View
{
	// Generate Id:82920e09-68ae-4e89-8e14-8fdbeaf4f921
	public partial class GameObject
	{
		public const string Name = "GameObject";
		
		
		private GameObjectData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public GameObjectData Data
		{
			get
			{
				return mData;
			}
		}
		
		GameObjectData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new GameObjectData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
