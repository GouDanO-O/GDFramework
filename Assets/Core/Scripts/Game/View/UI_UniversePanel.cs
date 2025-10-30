using System.Collections.Generic;
using Core.Game.Chunk.Universe;
using Core.Game.Chunk.Universe.Data;
using Core.Game.Chunk.World.Data;
using Core.Game.View.Details;
using Cysharp.Threading.Tasks;
using GDFramework.FrameData;
using GDFramework.Utility;
using GDFrameworkCore;
using UnityEngine;
using UnityEngine.UI;
using GDFrameworkExtend.UIKit;
using TMPro;

namespace Core.Game.View
{
	public class UI_UniversePanelData : UIPanelData
	{
	}
	public partial class UI_UniversePanel : UIPanel,ICanGetSystem,ICanGetUtility
	{
		protected Transform UniverseMap;

		protected Transform UniverseMapCenter;

		protected Transform UniverseMapHeader;
		
		protected UI_UniverseWorldDetailShow UniverseWorldDetailShow;

		protected GameObject UniverseSingleWorld;

		protected Button ManageUniverseWorldDetailShowButton;

		protected Button ContinueButton;

		protected Transform UniverseMapCenterContentRoot;

		protected List<UI_UniverseSingleWorld> CurUniverseOwnedWorldList = new List<UI_UniverseSingleWorld>();

		protected UniverseData UniverseData;
		
		protected TMP_InputField UniverseMapNameInputField;

		protected TextMeshProUGUI FocusWorldName;


		private WorldData _curSelectingWorldData;
		
		public IArchitecture GetArchitecture()
		{
			return GameMain.Interface;
		}
		
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UI_UniversePanelData ?? new UI_UniversePanelData();
			// please add init code here
			UniverseData = this.GetSystem<UniverseSystem>().GetCurrentUniverseData();
			UniverseMap = Common.Find("UniverseMap");
			UniverseMapCenter = UniverseMap.Find("UniverseMapCenter");
			UniverseMapHeader = UniverseMap.Find("UniverseMapHeader");

			UniverseWorldDetailShow = UniverseMap.Find("UniverseWorldDetailShow").GetComponent<UI_UniverseWorldDetailShow>();

			UniverseMapCenterContentRoot = UniverseMapCenter.Find("Contents");

			ContinueButton = UniverseMapHeader.Find("ContinueButton").GetComponent<Button>();
			
			ManageUniverseWorldDetailShowButton =
				UniverseMapHeader.Find("ManageUniverseWorldDetailShowButton").GetComponent<Button>();
			
			UniverseMapNameInputField =
				UniverseMapHeader.Find("UniverseMapName/NameInput").GetComponent<TMP_InputField>();
			FocusWorldName = UniverseMapHeader.Find("FocusWorld/WorldName").GetComponent<TextMeshProUGUI>();
			
			ContinueButton.onClick.AddListener(ContinueToNext);
			ManageUniverseWorldDetailShowButton.onClick.AddListener(OpenOrCloseDetailWorldShow);
			UniverseMapNameInputField.onValueChanged.AddListener(ListenUniverseMapNameChange);
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

		protected void ListenUniverseMapNameChange(string changeName)
		{
			if (UniverseData != null)
			{
				UniverseData.UniverseDef.ChangeWorldName(changeName);
			}
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

		/// <summary>
		/// 更新当前选中的世界数据
		/// </summary>
		/// <param name="curSelectingData"></param>
		public void UpdateCurSelectingWorld(WorldData curSelectingData)
		{
			_curSelectingWorldData = curSelectingData;
			UniverseWorldDetailShow.UpdateShow(curSelectingData);
		}

		/// <summary>
		/// 进入下一级
		/// </summary>
		private void ContinueToNext()
		{
			
		}

		/// <summary>
		/// 是否打开世界详情
		/// </summary>
		private void OpenOrCloseDetailWorldShow()
		{
			UniverseWorldDetailShow.gameObject.SetActive(!UniverseWorldDetailShow.gameObject.activeSelf);
			UniverseWorldDetailShow.UpdateShow(_curSelectingWorldData);
		}
	}
}
