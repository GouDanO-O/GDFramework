using System;
using GDFramework.Utility;
using GDFrameworkCore;
using GDFrameworkExtend.ResKit;
using YooAsset;
using Object = UnityEngine.Object;

namespace GDFrameworkExtend.YooAssetKit
{
    public class YooAssetLoader : Res,IController
    {
        private string _location;
        
        public IArchitecture GetArchitecture()
        {
            return Main.Interface;
        }
    }
}