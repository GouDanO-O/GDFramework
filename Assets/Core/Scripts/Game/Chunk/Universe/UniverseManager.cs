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