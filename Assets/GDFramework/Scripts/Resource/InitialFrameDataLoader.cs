using GDFramework.FrameData;
using GDFramework.Input;
using GDFramework.Models;
using GDFramework.Multilingual;
using GDFrameworkCore;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GDFramework.Resource
{
    /// <summary>
    /// 框架初始化资源加载器
    /// 主要初始化一些框架内部配置
    /// </summary>
    public class InitialFrameDataLoader : BaseResourcesLoader
    {
        private GameDataModel _gameDataModel;

        protected override void AddLoadingResource()
        {
            _gameDataModel = this.GetModel<GameDataModel>();
            WillLoadResourcesList.Add(new SResourcesLoaderNode()
            {
                dataName = DefaultPackage.GameModelData.defaultpackage_gdframework_gdframeworkdata_gameinput_inputactions.GameInput,
                loaderCallback = data =>
                {
                    _gameDataModel.InputActionAsset = data as InputActionAsset;
                    LoadingCheck();
                }
            });
            WillLoadResourcesList.Add(new SResourcesLoaderNode()
            {
                dataName = DefaultPackage.GameModelData.defaultpackage_gdframework_res_mul_tbmultilingual_json.tbmultilingual,
                loaderCallback = data =>
                {
                    this.GetModel<MultilingualDataModel>().SetTextAsset(data as TextAsset);
                    LoadingCheck();
                }
            });
        }
    }
}