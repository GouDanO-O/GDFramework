using GDFrameworkCore;

namespace GDFramework.World.Object
{
    /// <summary>
    /// 世界物体管理
    /// </summary>
    public class WorldObjectManager : AbstractSystem
    {
        private WorldObjectModel _worldObjectModel;
        
        protected override void OnInit()
        {
            
        }

        public WorldObjectManager()
        {
            InitManager();
        }

        private void InitManager()
        {
            _worldObjectModel = this.GetModel<WorldObjectModel>();
        }

        /// <summary>
        /// 生成物体
        /// </summary>
        public void SpawnWorldObj()
        {
            
        }

        /// <summary>
        /// 更新世界物体逻辑
        /// </summary>
        public void FixedUpdateWorldObjsLogic()
        {
            
        }
        
        /// <summary>
        /// 更新世界物体逻辑
        /// </summary>
        public void UpdateWorldObjsLogic()
        {
            
        }
        
        /// <summary>
        /// 销毁物体
        /// </summary>
        public void DestroyWorldObj(IObj willDestroyObj)
        {
            _worldObjectModel.DestroyObj(willDestroyObj);
        }
    }
}