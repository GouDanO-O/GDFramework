using System.Collections.Generic;
using Core.Game.Chunk.Universe;
using Core.Game.Chunk.Universe.Data;
using Core.Game.Chunk.World.Data;
using Core.Game.View.Details;
using GDFramework.FrameData;
using GDFramework.Utility;
using GDFrameworkCore;
using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;

namespace Core.Game.View
{
	public class UI_UniversePanelData : UIPanelData
	{
	}
	public partial class UI_UniversePanel : UIPanel,ICanGetSystem,ICanGetUtility
	{
		protected Transform UniverseMap;

		protected GameObject UniverseSingleWorld;

		protected List<UI_UniverseSingleWorld> CurUniverseOwnedWorldList = new List<UI_UniverseSingleWorld>();

		protected UniverseData UniverseData;
		
		public IArchitecture GetArchitecture()
		{
			return GameMain.Interface;
		}
		
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UI_UniversePanelData ?? new UI_UniversePanelData();
			// please add init code here
			UniverseData = this.GetSystem<UniverseManager>().GetCurrentUniverseData();
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
			SpawnUniverseWorld();
		}
		
		protected override void OnShow()
		{
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
			
		}

		/// <summary>
		/// 根据配置来生成宇宙中的世界
		/// </summary>
		protected async void SpawnUniverseWorld()
		{
			if (UniverseData != null)
			{
				if (UniverseSingleWorld == null)
				{
					UniverseSingleWorld = await this.GetUtility<ResourcesUtility>()
						.LoadPrefabAsync(DefaultPackage.UIDetails.DetailsAssetGroup.UniverseSingleWorld);
				}

				List<WorldData> universeWorldDatas = UniverseData.GetAllWorlds();
				for (int i = 0; i < universeWorldDatas.Count; i++)
				{
					WorldData curWorldData = universeWorldDatas[i];
					GameObject spawnedWorld = Object.Instantiate(UniverseSingleWorld);
					UI_UniverseSingleWorld singleWorld = spawnedWorld.GetComponent<UI_UniverseSingleWorld>();
					singleWorld.SetWorldData(curWorldData);
					CurUniverseOwnedWorldList.Add(singleWorld);
				}
				
			}
		}

		public void UpdateCurSelectingWorld(WorldData curSelectingData)
		{
			
		}
	}
}
