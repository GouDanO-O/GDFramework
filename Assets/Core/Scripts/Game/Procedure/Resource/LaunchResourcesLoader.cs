using System;
using Core.Game.Procedure.Models.Resource;
using GDFramework.Resource;
using GDFrameworkCore;
using UnityEngine.Events;


namespace Core.Game.Procedure.Resource
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