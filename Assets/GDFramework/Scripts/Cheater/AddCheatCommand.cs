using System;
using GDFramework.Models;
using GDFrameworkCore;

namespace GDFramework.Cheater
{
    /// <summary>
    /// 添加作弊模块
    /// </summary>
    public abstract class AddCheatCommand : ICanGetSystem,ICanGetModel,ICanGetUtility
    {
        public IArchitecture GetArchitecture()
        {
            return Main.Interface;
        }
        
        public abstract void Execute();

    }
}