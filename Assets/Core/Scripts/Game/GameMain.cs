using Core.Game.Chunk.World;
using Game.Models.Resource;
using GDFrameworkCore;

namespace Core.Game
{
    public class GameMain : Main
    {
        protected override void Register_System()
        {
            base.Register_System();

        }
        
        protected override void Register_Model()
        {
            base.Register_Model();
            this.RegisterModel(new LaunchResourcesDataModel());
            this.RegisterModel(new GameSceneResourcesDataModel());
            this.RegisterModel(new WorldDataModel());
        }

        protected override void Register_Utility()
        {
            base.Register_Utility();
            this.RegisterUtility(new WorldDataUtility());
        }
    }
}