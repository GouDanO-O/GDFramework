using GDFramework.World.Map;
using GDFramework.World.Object;
using GDFramework.World.Times;
using GDFramework.World.Weather;
using GDFrameworkCore;
using GDFrameworkExtend.SingletonKit;

namespace GDFramework.World
{
    /// <summary>
    /// 世界管理器
    /// </summary>
    public class WorldManager : Singleton<WorldManager>,IController
    {
        private WorldObjectManager _worldObjectManager;
        
        private WorldTimeManager _worldTimeManager;
        
        private WorldMapManager _worldMapManager => WorldMapManager.Instance;
        
        private WorldWeatherManager _worldWeatherManager;

        private WorldManager()
        {
            
        }

        public void InitWorldManager()
        {
            InitComponents();
        }

        /// <summary>
        /// 初始化组件
        /// </summary>
        private void InitComponents()
        {
            _worldObjectManager = this.GetSystem<WorldObjectManager>();
        }

        public IArchitecture GetArchitecture()
        {
            return Main.Interface;
        }

        /// <summary>
        /// 固定帧更新世界逻辑
        /// </summary>
        public void FixedUpdateWorldLogic()
        {
            _worldTimeManager.FixedUpdateWorldTime();
            _worldWeatherManager.FixedUpdateWeather();
            _worldObjectManager.FixedUpdateWorldObjsLogic();
            _worldMapManager.FixedUpdateWorldMap();
        }
        
        /// <summary>
        /// 更新世界逻辑
        /// </summary>
        public void UpdateWorld()
        {
            _worldTimeManager.UpdateWorldTime();
            _worldWeatherManager.UpdateWeather();
            _worldObjectManager.UpdateWorldObjsLogic();
            _worldMapManager.UpdateWorldMap();
        }
    }
}