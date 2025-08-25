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
            
        }
        
    }
}
