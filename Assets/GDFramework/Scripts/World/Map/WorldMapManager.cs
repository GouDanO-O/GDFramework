using GDFrameworkCore;
using GDFrameworkExtend.SingletonKit;

namespace GDFramework.World.Map
{
    /// <summary>
    /// 世界地图管理器
    /// </summary>
    public class WorldMapManager : MonoSingleton<WorldMapManager>,IController
    {
        private WorldMapModel _worldMapModel;
        
        public IArchitecture GetArchitecture()
        {
            return Main.Interface;
        }

        public void FixedUpdateWorldMap()
        {
            
        }
        
        public void UpdateWorldMap()
        {
            
        }

        private void UpdateWorldMapGird()
        {
            
        }

        /// <summary>
        /// 更新主玩家的位置
        /// </summary>
        private void UpdateHostPlayerWorldPos()
        {
            
        }
    }
}