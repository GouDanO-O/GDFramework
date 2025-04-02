using System;
using GDFramework.Models;
using QFramework;

namespace GDFramework.Command.Cheat
{
    /// <summary>
    /// 添加作弊模块
    /// </summary>
    public abstract class AddCheat_Command : AbstractCommand
    {
        public string Name { get;protected set; }

        public virtual void InitData(string name, Action action)
        {
            Name = name;
        }
        
        protected override void OnExecute()
        {
            this.GetModel<CheatData_Model>().AddCheatModule(Name,this);
        }

        public abstract void Execute();
    }
    
}