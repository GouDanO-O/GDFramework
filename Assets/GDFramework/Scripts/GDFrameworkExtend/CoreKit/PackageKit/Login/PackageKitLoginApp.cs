using GDFramework_Core.Scripts.GDFrameworkCore;

#if UNITY_EDITOR
namespace GDFrameworkExtend.CoreKit
{
    internal class PackageKitLoginApp : Architecture<PackageKitLoginApp>
    {
        protected override void Init()
        {
            RegisterModel<IPackageLoginService>(new PacakgeLoginService());
        }
    }
}
#endif