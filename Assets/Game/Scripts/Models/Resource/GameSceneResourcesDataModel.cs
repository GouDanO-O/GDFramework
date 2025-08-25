using System.Collections.Generic;
using GDFramework.YooAssetKit;
using GDFrameworkCore;
using UnityEngine;
using YooAsset;


namespace Game.Models.Resource
{
    public class GameSceneResourcesDataModel : AbstractModel,ICanGetSystem
    {
        public Dictionary<string,TextAsset> WorldDataTextAssets = new Dictionary<string,TextAsset>();

        public string WorldDataTag = "WorldData";
        
        protected override void OnInit()
        {
            
        }
    }
}