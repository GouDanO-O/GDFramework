using GDFramework.Models.Enums;
using GDFramework.Utility;
using QFramework;

namespace GDFramework.Game.SDK
{
    public class SdkManager : AbstractSystem
    {
        private EPlatform m_Platform = EPlatform.APK;
        
        private Sdk_Utility m_SdkUtility;
        
        protected override void OnInit()
        {
            m_SdkUtility = this.GetUtility<Sdk_Utility>();
            m_SdkUtility.InitSDK();
            this.RegisterEvent<SOnShowADEvent>((data) =>
            {
                m_SdkUtility.GetWillShowAdType(data);
            });
        }
    }
}

