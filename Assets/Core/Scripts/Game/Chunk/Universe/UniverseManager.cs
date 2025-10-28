using Core.Game.Chunk.Universe.Data;
using Core.Game.Chunk.World;
using Core.Game.Chunk.World.Data;
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
    public class UniverseManager : ChunkManager
    {
        private UniverseDataModel _universeDataModel;
        private WorldManager _worldManager;
        private UniverseComponentController _universeComponentController;

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
            
            // 加载或创建初始宇宙
            LoadOrCreateInitialUniverse();
        }

        /// <summary>
        /// 加载或创建初始宇宙
        /// </summary>
        private void LoadOrCreateInitialUniverse()
        {
            bool hasSaveData = CheckHasSaveData();
            
            if (hasSaveData)
            {
                // 从存档加载
                string lastUniverseInstanceId = GetLastUniverseInstanceId();
                _currentUniverseData = _universeDataModel.LoadUniverse(lastUniverseInstanceId);
            }
            else
            {
                // 创建新宇宙
                string defaultUniverseDefId = GetDefaultUniverseDefId();
                _currentUniverseData = _universeDataModel.CreateUniverse(defaultUniverseDefId);
            }
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
            
            // 初始化世界
            SetInitialWorld();
        }

        #region 宇宙管理
        
        /// <summary>
        /// 设置初始世界
        /// </summary>
        public void SetInitialWorld()
        {
            _worldManager = this.GetSystem<WorldManager>();
            
            if (_currentUniverseData != null)
            {
                // TODO: 根据宇宙数据设置初始世界
                _worldManager.SetFocusRegion();
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
                // _currentUniverseData.ChangeWorld(willChangeWorld, lastWorld);
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
            // 保存当前宇宙数据
            _currentUniverseData?.SaveTemporaryData();
            
            // 保存所有实例数据
            _universeDataModel?.SaveAll();
            
            Debug.Log("宇宙数据已保存");
        }

        private bool CheckHasSaveData()
        {
            // TODO: 实现检查存档逻辑
            return ES3.KeyExists("LastUniverseInstanceId");
        }

        private string GetLastUniverseInstanceId()
        {
            return ES3.Load<string>("LastUniverseInstanceId", "");
        }

        private string GetDefaultUniverseDefId()
        {
            // TODO: 返回默认宇宙配置ID
            return "UNIVERSE_DEF_DEFAULT";
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