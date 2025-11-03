using Core.Game.Chunk.Universe.Data;
using Core.Game.Chunk.World;
using Core.Game.Chunk.World.Data;
using Core.Game.Storage;
using Cysharp.Threading.Tasks;
using GDFramework.Utility;
using GDFrameworkCore;
using UnityEngine;

namespace Core.Game.Chunk.Universe
{
    /// <summary>
    /// 宇宙管理器---专注于游戏
    /// 一个宇宙里面会有多个世界
    /// </summary>
    public class UniverseSystem : ChunkSystem
    {
        private UniverseDataModel _universeDataModel;
        
        private WorldSystem _worldSystem;
        
        private UniverseComponentController _universeComponentController;

        private StorageSystem _storageSystem;

        // 当前激活的宇宙数据
        private UniverseData _currentUniverseData;

        protected override string ComponentControllerPath
        {
            get
            {
                return GDFramework.FrameData.DefaultPackage.Prefabs
                    .UniverseControllerAssetGroup.UniverseController;
            }
        }

        protected override void InitChunkDataModel()
        {
            _universeDataModel = this.GetModel<UniverseDataModel>();
            LoadOrCreateInitialUniverse();
        }

        /// <summary>
        /// 加载或创建初始宇宙
        /// </summary>
        private void LoadOrCreateInitialUniverse()
        {
            
        }

        protected override async void SpawnComponentController()
        {
            try
            {
                GameObject prefab = await this.GetUtility<ResourcesUtility>()
                    .LoadPrefabAsync(ComponentControllerPath);

                if (_universeComponentController == null)
                {
                    _universeComponentController = 
                        Object.Instantiate(prefab).GetComponent<UniverseComponentController>();
                    _universeComponentController.InitOwnedComponents();
                    await OnComponentControllerCreated();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"生成UniverseComponentController失败: {e.Message}");
            }
        }
        
        protected override async UniTask OnComponentControllerCreated()
        { 
            await base.OnComponentControllerCreated();
            SetInitialWorld();
        }

        #region 宇宙管理
        
        /// <summary>
        /// 设置初始世界
        /// </summary>
        public void SetInitialWorld()
        {
            _worldSystem = this.GetSystem<WorldSystem>();
            
            if (_currentUniverseData != null)
            {
                _worldSystem.SetFocusRegion();
            }
        }
        
        /// <summary>
        /// 切换世界
        /// </summary>
        public void ChangeWorld(WorldData willChangeWorld, WorldData lastWorld)
        {
            if (_currentUniverseData != null)
            {
                // TODO: 实现世界切换逻辑

            }
        }

        /// <summary>
        /// 获取当前宇宙数据
        /// </summary>
        public UniverseData GetCurrentUniverseData()
        {
            return _currentUniverseData;
        }

        #endregion

        #region 存档管理

        public override void SaveAllData()
        {
            
        }

        #endregion

        protected override void Cleanup()
        {
            base.Cleanup();
            
            if (_universeComponentController != null)
            {
                Object.Destroy(_universeComponentController.gameObject);
                _universeComponentController = null;
            }
        }
        
    }
}