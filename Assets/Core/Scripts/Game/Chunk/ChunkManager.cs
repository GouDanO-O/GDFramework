using GDFrameworkCore;

namespace Core.Game.Chunk
{
    public class ChunkManager : AbstractSystem
    {
        protected override void OnInit()
        {
            
        }

        protected virtual void InitManager()
        {
            RegisterEvents();
            InitChunkData();
            InitComponent();
        }

        protected virtual void RegisterEvents()
        {
            
        }

        protected virtual void InitChunkData()
        {
            
        }

        protected virtual void InitComponent()
        {
            
        }
    }
}