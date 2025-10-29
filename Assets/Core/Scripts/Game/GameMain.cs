using Core.Game.Chunk.Region;
using Core.Game.Chunk.Room;
using Core.Game.Chunk.Universe;
using Core.Game.Chunk.Universe.Data;
using Core.Game.Chunk.World;
using Core.Game.Procedure.Models.Resource;
using GDFrameworkCore;

namespace Core.Game
{
    public class GameMain : Main
    {
        protected override void Register_System()
        {
            base.Register_System();
            this.RegisterSystem(new UniverseSystem());
            this.RegisterSystem(new WorldSystem());
            this.RegisterSystem(new RegionSystem());
            this.RegisterSystem(new RoomSystem());
        }
        
        protected override void Register_Model()
        {
            base.Register_Model();
            this.RegisterModel(new LaunchResourcesDataModel());
            this.RegisterModel(new GameSceneResourcesDataModel());
            
            this.RegisterModel(new UniverseDataModel());
        }

        protected override void Register_Utility()
        {
            base.Register_Utility();
        }
    }
}