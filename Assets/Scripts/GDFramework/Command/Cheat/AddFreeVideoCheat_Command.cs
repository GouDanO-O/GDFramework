using System;

namespace GDFramework.Command.Cheat
{
    public class AddFreeVideoCheat_Command : AddCheat_Command
    {
        public bool IsActive { get;private set; }
        
        public Func<bool> freeVideoCheatAction;

        public void InitData(string name, Func<bool> freeVideoCheatAction)
        {
            this.Name = name;
            this.freeVideoCheatAction = freeVideoCheatAction;
        } 

        public override void Execute()
        {
            IsActive = freeVideoCheatAction?.Invoke() ?? false;
        }
    }
}