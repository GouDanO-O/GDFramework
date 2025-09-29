using System.Linq;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Models.Resource;
using Game.World;
using GDFramework.FrameData;
using GDFramework.Resource;
using GDFramework.Utility;
using GDFramework.YooAssetKit;
using GDFrameworkCore;
using Newtonsoft.Json;
using UnityEngine;
using YooAsset;

namespace Game.Resource
{
    public class GameSceneResourcesLoader : BaseResourcesLoader,ICanGetSystem
    {
        private GameSceneResourcesDataModel _gameSceneResourcesDataModel;

        private WorldDataModel _worldDataModel;

        protected override void AddLoadingResource()
        {
            _gameSceneResourcesDataModel = this.GetModel<GameSceneResourcesDataModel>();
            LoadAllDtosAsync().Forget();
        }

        public async UniTask LoadAllDtosAsync()
        {
            
            var package = this.GetSystem<YooAssetManager>().GetPackage();
            AssetInfo[] assetInfos = package.GetAssetInfos("WorldData");

            for (int i = 0; i < assetInfos.Length; i++)
            {
                var asset = package.LoadAssetAsync<Game.World.Dto>(assetInfos[i].AssetPath);
                await asset;
                if (asset.AssetObject is Game.World.Dto loadedDto && !string.IsNullOrEmpty(loadedDto.dtoId))
                {
                    
                    
                }
            }
            
            LoadingComplete();
        }
    }
    
    
}
