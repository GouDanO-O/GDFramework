using System;
using GDFramework_Core.Models.Enums;

namespace GDFramework_Core.Game.SDK
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