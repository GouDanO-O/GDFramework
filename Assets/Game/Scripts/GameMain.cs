using Game.Models.Resource;
using Game.World.Player;
using GDFrameworkCore;
using GDFrameworkExtend.StorageKit;

namespace Game
{
    public class GameMain : Main
    {
        protected override void Register_System()
        {
            base.Register_System();
            this.RegisterSystem(new PlayerHealthy());
            this.RegisterSystem(new PlayerInventory());
        }
        
        protected override void Register_Model()
        {
            base.Register_Model();
            this.RegisterModel(new LaunchResourcesDataModel());
            this.RegisterModel(new GameSceneResourcesDataModel());
            this.RegisterModel(new PlayerHealthyModel());
            this.RegisterModel(new PlayerInventoryModel());
        }

        protected override void Register_Utility()
        {
            base.Register_Utility();
        }
    }
}