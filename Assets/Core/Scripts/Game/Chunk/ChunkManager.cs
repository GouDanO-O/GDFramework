using Cysharp.Threading.Tasks;
using GDFrameworkCore;

namespace Core.Game.Chunk
{
    public abstract class ChunkManager : AbstractSystem
    {
        /// <summary>
        /// 控制器预制体的AB路径
        /// </summary>
        protected abstract string ComponentControllerPath { get; }

        protected override void OnInit()
        {
            
        }

        /// <summary>
        /// 初始化管理器
        /// </summary>
        public void InitManager()
        {
            RegisterEvents();
            InitChunkDataModel();
            InitComponent();
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        protected virtual void RegisterEvents()
        {
            
        }

        /// <summary>
        /// 初始化数据模型(从Model层获取数据)
        /// </summary>
        protected abstract void InitChunkDataModel();

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
        /// 组件控制器创建成功后的回调
        /// </summary>
        protected virtual async UniTask OnComponentControllerCreated()
        {
            await UniTask.NextFrame();
        }

        /// <summary>
        /// 保存所有数据
        /// </summary>
        public abstract void SaveAllData();

        /// <summary>
        /// 清理资源
        /// </summary>
        protected virtual void Cleanup()
        {
            
        }
    }
}