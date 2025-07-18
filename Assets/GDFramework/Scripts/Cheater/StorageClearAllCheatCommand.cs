using GDFramework.Cheater;
using GDFrameworkCore;
using GDFrameworkExtend.StorageKit;

namespace GDFramework.Scripts.Cheater
{
    public class StorageClearAllCheatCommand: AddCheatCommand
    {
        public override void Execute()
        {
            this.GetSystem<StorageKit>().ClearAllData();
        }
    }
}