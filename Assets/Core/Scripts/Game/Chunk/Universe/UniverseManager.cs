using Core.Game.Chunk.Universe.Data;
using Core.Game.Chunk.World;
using Core.Game.Chunk.World.Data;
using GDFrameworkCore;
using GDFrameworkExtend.SingletonKit;

namespace Core.Game.Chunk.Universe
{
    /// <summary>
    /// 宇宙管理器
    /// 一个宇宙里面会有多个世界
    /// </summary>
    public class UniverseManager : AbstractSystem
    {
        private UniverseDataModel _universeData;
        
        private WorldManager _worldManager;
        
        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        protected override void OnInit()
        {
            this.InitUniverseManager();
        }

        private void InitUniverseManager()
        {
            _universeData = this.GetModel<UniverseDataModel>();
        }



        #region 宇宙管理
        
        /// <summary>
        /// 设置登录进来的初始宇宙里面的世界
        /// 如果是第一次登录,那么就以默认世界为焦点
        /// 如果非第一次登录,那么就以上一次存档中的世界为焦点
        /// </summary>
        public void SetInitialWorld()
        {
            this._worldManager = this.GetSystem<WorldManager>();
            this._worldManager.SetFocusRegion();
        }
        
        /// <summary>
        /// 切换世界
        /// </summary>
        public void ChangeWorld(WorldData willChangeWorld,WorldData lastWorld)
        {
            _universeData.ChangeWorld(willChangeWorld, lastWorld);
        }

        private void SaveWorld()
        {
            
        }

        #endregion
    }
}