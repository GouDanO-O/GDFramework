using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Core.Game.Chunk.Region;
using GDFrameworkCore;
using Core.Game.Chunk.World.Data;
using Cysharp.Threading.Tasks;
using GDFramework.Utility;

namespace Core.Game.Chunk.World
{
    public class WorldSystem : ChunkSystem
    {
        private WorldDataModel _worldDataModel;
        private WorldComponentController _worldComponentController;

        // 当前激活的世界
        private WorldData _currentWorldData;

        // 所有已加载的世界
        private List<WorldData> _loadedWorlds = new List<WorldData>();

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
            _worldDataModel = this.GetModel<WorldDataModel>();
        }

        protected override async void SpawnComponentController()
        {
            try
            {
                GameObject prefab = await this.GetUtility<ResourcesUtility>()
                    .LoadPrefabAsync(ComponentControllerPath);

                if (_worldComponentController == null)
                {
                    _worldComponentController =
                        Object.Instantiate(prefab).GetComponent<WorldComponentController>();
                    _worldComponentController.InitOwnedComponents();
                    await OnComponentControllerCreated();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"生成WorldComponentController失败: {e.Message}");
            }
        }

        #region 世界管理

        /// <summary>
        /// 设置焦点区域
        /// </summary>
        public void SetFocusRegion()
        {
            if (_currentWorldData != null)
            {
                // TODO: 实现焦点区域设置逻辑
                string currentRegionId = _currentWorldData.WorldTempData.CurrentRegionInstanceId;
                Debug.Log($"设置焦点区域: {currentRegionId}");
            }
        }

        /// <summary>
        /// 创建并加载世界
        /// </summary>
        public WorldData CreateWorld(string defId)
        {
            var world = _worldDataModel.CreateWorld(defId);
            _loadedWorlds.Add(world);
            return world;
        }

        /// <summary>
        /// 加载已有世界
        /// </summary>
        public WorldData LoadWorld(string instanceId)
        {
            var world = _worldDataModel.LoadWorld(instanceId);
            if (world != null && !_loadedWorlds.Contains(world))
            {
                _loadedWorlds.Add(world);
            }

            return world;
        }

        /// <summary>
        /// 切换到指定世界
        /// </summary>
        public async UniTask SwitchToWorld(string worldInstanceId)
        {
            // 停用当前世界
            if (_currentWorldData != null)
            {
                _currentWorldData.Deactivate();
            }

            // 加载新世界
            var newWorld = LoadWorld(worldInstanceId);
            if (newWorld != null)
            {
                _currentWorldData = newWorld;
                _currentWorldData.Activate();

                // 触发世界切换事件
                await OnWorldSwitched(_currentWorldData);
            }
        }

        /// <summary>
        /// 世界切换后的回调
        /// </summary>
        protected virtual async UniTask OnWorldSwitched(WorldData newWorld)
        {
            Debug.Log($"已切换到世界: {newWorld.InstanceId}");

            // 重新设置焦点区域
            SetFocusRegion();

            await UniTask.NextFrame();
        }

        /// <summary>
        /// 卸载世界
        /// </summary>
        public void UnloadWorld(string worldInstanceId, bool saveData = true)
        {
            var world = _loadedWorlds.Find(w => w.InstanceId == worldInstanceId);
            if (world != null)
            {
                if (saveData)
                {
                    world.SaveTemporaryData();
                }

                _loadedWorlds.Remove(world);

                if (_currentWorldData == world)
                {
                    _currentWorldData = null;
                }
            }
        }

        /// <summary>
        /// 获取当前世界
        /// </summary>
        public WorldData GetCurrentWorld()
        {
            return _currentWorldData;
        }

        #endregion

        #region 存档管理

        public override void SaveAllData()
        {
            // 保存所有已加载的世界
            foreach (var world in _loadedWorlds)
            {
                world.SaveTemporaryData();
            }

            // 保存所有实例数据
            _worldDataModel?.SaveAll();

            Debug.Log($"已保存 {_loadedWorlds.Count} 个世界的数据");
        }

        #endregion

        protected override void Cleanup()
        {
            base.Cleanup();

            // 保存并清理所有世界
            SaveAllData();
            _loadedWorlds.Clear();

            if (_worldComponentController != null)
            {
                Object.Destroy(_worldComponentController.gameObject);
                _worldComponentController = null;
            }
        }
    }
}