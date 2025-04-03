using System;
using GDFramework_Core.Models;
using QFramework;

namespace GDFramework_Core.Cheater
{
    /// <summary>
    /// 添加作弊模块
    /// </summary>
    public abstract class AddCheatCommand : AbstractCommand
    {
        public string Name { get;protected set; }

        public virtual void InitData(string name, Action action)
        {
            Name = name;
        }
        
        protected override void OnExecute()
        {
            this.GetModel<CheatDataModel>().AddCheatModule(Name,this);
        }

        public abstract void Execute();
    }
    
}