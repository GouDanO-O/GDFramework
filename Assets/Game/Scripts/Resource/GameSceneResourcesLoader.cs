using System.Linq;
using System.Collections.Generic;
using Game.Models.Resource;
using GDFramework.FrameData;
using GDFramework.Resource;
using GDFrameworkCore;
using UnityEngine;
using YooAsset;

public class GameSceneResourcesLoader : BaseResourcesLoader
{
    private GameSceneResourcesDataModel _model;

    protected override void AddLoadingResource()
    {
        _model = this.GetModel<GameSceneResourcesDataModel>();
    }
    
}