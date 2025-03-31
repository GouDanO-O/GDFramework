using Frame.Models;
using Frame.Models.Resource;
using QFramework;

namespace Frame.Game.Resource
{
    public class LaunchResourcesLoader : BaseResourcesLoader
    {
        private LaunchResourcesData_Model _launchResourcesDataModel;
        
        protected override void StartLoading()
        {
            _launchResourcesDataModel = this.GetModel<LaunchResourcesData_Model>();
            WillLoadResourcesList.Add(QAssetBundle.Tbmultilingual_json.tbmultilingual);
            base.StartLoading();
        }

        protected override void LoadingResources()
        {
            for (int i = 0; i < WillLoadResourcesList.Count; i++)
            {
                _loader.LoadJsonAsync(WillLoadResourcesList[i], (data) =>
                {
                    _launchResourcesDataModel.Multilingual = data;
                    LoadingCheck();
                });
            }
        }
    }
}