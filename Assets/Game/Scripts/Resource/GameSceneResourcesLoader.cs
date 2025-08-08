using Game.Models.Resource;
using GDFramework.FrameData;
using GDFramework.Resource;
using GDFrameworkCore;
using UnityEngine;

namespace Game.Resource
{
    public class GameSceneResourcesLoader : BaseResourcesLoader
    {
        private GameSceneResourcesDataModel _gameSceneResourcesDataModel;
        
        protected override void AddLoadingResource()
        {
            _gameSceneResourcesDataModel = this.GetModel<GameSceneResourcesDataModel>();
            WillLoadResourcesList.Add(new SResourcesLoaderNode()
            {

            });
        }
    }
}