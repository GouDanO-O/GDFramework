using System;
using GDFramework.Models.Enums;

namespace GDFramework.Game.SDK
{
    /// <summary>
    /// 显示广告
    /// </summary>
    public struct SOnShowADEvent
    {
        public EADType willShowAdType;

        public string showAdDes;
        
        public Action successCallback;
        
        public Action failCallback;
    }
}