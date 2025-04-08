using GDFramework.Models;
using GDFrameworkCore;
using UnityEngine;

namespace GDFramework.Resource
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