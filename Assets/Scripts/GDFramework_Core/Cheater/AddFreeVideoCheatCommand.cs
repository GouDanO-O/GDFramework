using System;

namespace GDFramework_Core.Cheater
{
    public class AddFreeVideoCheatCommand : AddCheatCommand
    {
        public bool IsActive { get;private set; }
        
        public Func<bool> FreeVideoCheatAction;

        public AddFreeVideoCheatCommand(string name, Func<bool> freeVideoCheatAction)
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