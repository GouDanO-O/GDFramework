using System;

namespace GDFramework_Core.Command.Cheat
{
    public class AddFreeVideoCheat_Command : AddCheat_Command
    {
        public bool IsActive { get;private set; }
        
        public Func<bool> FreeVideoCheatAction;

        public AddFreeVideoCheat_Command(string name, Func<bool> freeVideoCheatAction)
        {
            this.Name = name;
            this.FreeVideoCheatAction = freeVideoCheatAction;
        } 

        public override void Execute()
        {
            IsActive = FreeVideoCheatAction?.Invoke() ?? false;
        }
    }
}