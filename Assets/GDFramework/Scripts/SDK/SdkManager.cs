using GDFramework_Core.Models.Enums;
using GDFramework_Core.Scripts.GDFrameworkCore;
using GDFramework_Core.Utility;


namespace GDFramework_Core.SDK
{
    public class SdkManager : AbstractSystem
    {
        private EPlatform m_Platform = EPlatform.APK;

        private Sdk_Utility m_SdkUtility;

        protected override void OnInit()
        {
            m_SdkUtility = this.GetUtility<Sdk_Utility>();
            m_SdkUtility.InitSDK();
            this.RegisterEvent<SOnShowADEvent>((data) => { m_SdkUtility.GetWillShowAdType(data); });
        }
    }
}