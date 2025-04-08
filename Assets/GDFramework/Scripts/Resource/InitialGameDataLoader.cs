using GDFramework_Core.Models;
using GDFrameworkCore;
using UnityEngine;

namespace GDFramework_Core.Resource
{
    public class InitialGameDataLoader : BaseResourcesLoader
    {
        private GameDataModel _gameDataModel;

        protected override void AddLoadingResource()
        {
            _gameDataModel = this.GetModel<GameDataModel>();
            WillLoadResourcesList.Add(new SResourcesLoaderNode()
            {
                dataName = GDAssetBundle.Tbmultilingual_json.TBMULTILINGUAL,
                loaderCallback = textAsset =>
                {
                    this.GetModel<MultilingualDataModel>().SetTextAsset(textAsset as TextAsset);
                    LoadingCheck();
                }
            });
        }
    }
}