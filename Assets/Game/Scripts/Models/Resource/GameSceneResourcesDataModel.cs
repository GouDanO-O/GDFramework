using GDFrameworkCore;
using UnityEngine;


namespace Game.Models.Resource
{
    public class GameSceneResourcesDataModel : AbstractModel
    {
        public TextAsset WorldDataAsset;

        public AssetBundle AreaBlockAssetBundle;

        public AssetBundle RoomAssetBundleBundle;

        public AssetBundle NodesAssetBundle;
        
        protected override void OnInit()
        {
            
        }
        
        
    }
}