using Cysharp.Threading.Tasks;
using GDFrameworkCore;

namespace Core.Game.Chunk
{
    public abstract class ChunkManager : AbstractSystem
    {
        /// <summary>
        /// 控制器预制体的AB路径
        /// </summary>
        protected abstract string ComponentControllerPath
        {
            get;
        }
        
        protected override void OnInit()
        {
            
        }

        /// <summary>
        /// 初始化管理器
        /// </summary>
        protected virtual void InitManager()
        {
            RegisterEvents();
            InitChunkData();
            InitComponent();
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        protected virtual void RegisterEvents()
        {
            
        }

        /// <summary>
        /// 初始化区块数据
        /// </summary>
        protected virtual void InitChunkData()
        {
            
        }

        /// <summary>
        /// 初始化组件
        /// </summary>
        protected virtual void InitComponent()
        {
            SpawnComponentController();
        }

        /// <summary>
        /// 生成组件控制器
        /// </summary>
        protected abstract void SpawnComponentController();

        /// <summary>
        /// 组件控制器创建成功后
        /// </summary>
        protected virtual async UniTask OnUniverseControllerCreated()
        {
            await UniTask.NextFrame();
        }
    }
}