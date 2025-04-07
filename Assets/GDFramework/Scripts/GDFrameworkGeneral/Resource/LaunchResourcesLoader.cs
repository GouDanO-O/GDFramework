using GDFramework_Core.Models;
using GDFramework_Core.Resource;
using GDFramework_Core.Scripts.GDFrameworkCore;
using GDFramework_General.Models.Resource;


namespace GDFramework_General.Resource
{
    public class LaunchResourcesLoader : BaseResourcesLoader
    {
        private LaunchResourcesDataModel _launchResourcesDataModel;

        protected override void StartLoading()
        {
            _launchResourcesDataModel = this.GetModel<LaunchResourcesDataModel>();
            //WillLoadResourcesList.Add(QAssetBundle.Tbmultilingual_json.tbmultilingual);

            base.StartLoading();
        }

        protected override void LoadingResources()
        {
            for (var i = 0; i < WillLoadResourcesList.Count; i++)
                _loader.LoadJsonAsync(WillLoadResourcesList[i], (data) =>
                {
                    this.GetModel<MultilingualDataModel>().SetTextAsset(data);
                    LoadingCheck();
                });
        }
    }
}