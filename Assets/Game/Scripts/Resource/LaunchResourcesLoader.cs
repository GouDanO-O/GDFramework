using System;
using Game.Models.Resource;
using GDFramework_Core.Models;
using GDFramework_Core.Resource;
using GDFrameworkCore;
using UnityEngine.Events;


namespace Game.Resource
{
    public class LaunchResourcesLoader : BaseResourcesLoader
    {
        private LaunchResourcesDataModel _launchResourcesDataModel;

        protected override void AddLoadingResource()
        {
            _launchResourcesDataModel = this.GetModel<LaunchResourcesDataModel>();
        }
    }
}