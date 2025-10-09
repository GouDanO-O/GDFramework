using Core.Game.Chunk.World;
using Core.Game.Chunk.World.Data;
using GDFrameworkCore;

namespace Core.Game.Chunk.Universe.Data
{
    public class UniverseDataUtility : IUtility
    {
        private UniverseDataModel _universeData;

        /// <summary>
        /// 验证是否能够进行切换世界
        /// </summary>
        public bool VerifyCanChangeWorld(WorldData willChangeWorld)
        {
            return true;
        }
    }
}