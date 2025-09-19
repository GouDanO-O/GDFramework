using System.Linq;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Models.Resource;
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
        private GameSceneResourcesDataModel _model;

        protected override void AddLoadingResource()
        {
            _model = this.GetModel<GameSceneResourcesDataModel>();
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
                    if (!_model.DtoRegistry.ContainsKey(loadedDto.dtoId))
                    {
                        _model.DtoRegistry.Add(loadedDto.dtoId, loadedDto);
                        if (loadedDto is Game.World.WorldDto worldDto)
                        {
                            _model.AllWorlds.Add(worldDto);
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"[DataManager] 发现重复的 dtoId: {loadedDto.dtoId}");
                    }
                }


            }
            
            LoadingComplete();
        }
    }
}
