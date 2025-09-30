using System.Linq;
using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Interface;
using Core.Game.Chunk.World;
using Cysharp.Threading.Tasks;
using Game.Models.Resource;
using GDFramework.FrameData;
using GDFramework.Resource;
using GDFramework.YooAssetKit;
using GDFrameworkCore;
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
                var asset = package.LoadAssetAsync<ChunkDto>(assetInfos[i].AssetPath);
                await asset;
            }
            
            LoadingComplete();
        }
    }
    
    
}
