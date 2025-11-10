using Cysharp.Threading.Tasks;
using GDFrameworkCore;
using GDFrameworkExtend.LogKit;
using UnityEngine;

namespace Core.Game.Chunk
{
    /// <summary>
    /// 区块管理器
    /// 流程如下:
    /// 第一次进入游戏:
    /// 所有配置加载完毕后->初始化ChunkManager->初始化固定数据和临时数据->生成ComponentController相关的组件->生成星图面板
    /// 从存档载入游戏(有真正进入房间才算有存档)
    /// 所有配置加载完毕后->初始化ChunkManager->初始化固定数据和临时数据->生成ComponentController相关的组件->进入存档房间
    /// </summary>
    public abstract class ChunkSystem : AbstractSystem
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
        public virtual void SaveAllData()
        {
            SaveDefData();
            SaveTempData();
        }

        /// <summary>
        /// 保存固定数据
        /// 除非是在编辑模式,否则不能保存固定数据
        /// </summary>
        public virtual void SaveDefData()
        {
            if (!ChunkManager.Instance.IsChunkEditor)
            {
                LogKit.Error("非区块编辑模式,无法保存");
                return;
            }
            
            
        }

        /// <summary>
        /// 保存临时数据
        /// </summary>
        public virtual void SaveTempData()
        {
            
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        protected virtual void Cleanup()
        {
            
        }
    }
}