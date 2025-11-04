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

        #endregion

        #region 存档管理

        public override void SaveAllData()
        {
            // 保存所有已加载的世界
            foreach (var world in _loadedWorlds)
            {
                world.SaveTemporaryData();
            }
            
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