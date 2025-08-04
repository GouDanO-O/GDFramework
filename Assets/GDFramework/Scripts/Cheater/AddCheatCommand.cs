using System;
using GDFramework.Models;
using GDFrameworkCore;

namespace GDFramework.Cheater
{
    /// <summary>
    /// 添加作弊模块
    /// </summary>
    public abstract class AddCheatCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            ExecuteCommand();
        }

        public abstract void ExecuteCommand();
    }
}