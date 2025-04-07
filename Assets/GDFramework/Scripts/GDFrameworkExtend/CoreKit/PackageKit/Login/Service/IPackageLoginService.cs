using System;
using GDFramework_Core.Scripts.GDFrameworkCore;

namespace GDFrameworkExtend.CoreKit
{
    internal interface IPackageLoginService: IModel
    {
        void DoGetToken(string username, string password, Action<string> onTokenGetted);
    }
}