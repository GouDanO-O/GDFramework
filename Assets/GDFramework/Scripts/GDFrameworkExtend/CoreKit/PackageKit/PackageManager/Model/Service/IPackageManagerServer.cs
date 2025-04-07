using System;
using System.Collections.Generic;
using GDFramework_Core.Scripts.GDFrameworkCore;

namespace GDFrameworkExtend.CoreKit
{
    internal interface IPackageManagerServer : IModel
    {
        void DeletePackage(string packageId, System.Action onResponse);
        void GetAllRemotePackageInfoV5(Action<List<PackageRepository>, List<string>> onResponse);
    }
}