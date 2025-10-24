using GDFrameworkCore;

namespace Core.Game.Chunk.Stronghold
{
    public class StrongholdManager : ChunkManager
    {
        protected override string ComponentControllerPath
        {
            get
            {
                return GDFramework.FrameData.DefaultPackage.Prefabs.UniverseControllerAssetGroup.UniverseController;
            }
        }
        protected override void OnInit()
        {
            
        }
        
        protected override void InitManager()
        {
            base.InitManager();
        }

        protected override void InitChunkData()
        {
            base.InitChunkData();
        }

        protected override void InitComponent()
        {
            base.InitComponent();
        }
        
        protected override void SpawnComponentController()
        {
            
        }
    }
}