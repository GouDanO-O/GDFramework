using GDFramework_Core.Models.Enums;
using GDFrameworkCore;
using GDFramework_Core.Utility;


namespace GDFramework_Core.SDK
{
    public class SdkManager : AbstractSystem
    {
        private EPlatform m_Platform = EPlatform.APK;

        private SdkUtility m_SdkUtility;

        protected override void OnInit()
        {
            m_SdkUtility = this.GetUtility<SdkUtility>();
            m_SdkUtility.InitSDK();
            this.RegisterEvent<SOnShowADEvent>((data) => { m_SdkUtility.GetWillShowAdType(data); });
        }
    }
}