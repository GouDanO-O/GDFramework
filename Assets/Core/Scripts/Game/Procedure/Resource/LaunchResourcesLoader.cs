using System;
using System.Collections.Generic;
using Core.Game.Chunk.Dungeon.Data;
using Core.Game.Chunk.Region.Data;
using Core.Game.Chunk.Room.Data;
using Core.Game.Chunk.Substance.Data;
using Core.Game.Chunk.Universe.Data;
using Core.Game.Chunk.World.Data;
using Core.Game.Procedure.Models.Resource;
using Cysharp.Threading.Tasks;
using GDFramework.Input;
using GDFramework.Resource;
using GDFramework.YooAssetKit;
using GDFrameworkCore;
using GDFrameworkExtend.LogKit;
using UnityEngine;
using YooAsset;

namespace Core.Game.Procedure.Resource
{
    public class LaunchResourcesLoader : BaseResourcesLoader, ICanGetSystem
    {
        private LaunchResourcesDataModel _launchResourcesDataModel;
        
        private UniverseDataModel _universeDataModel;
        
        private WorldDataModel _worldDataModel;
        
        private RegionDataModel _regionDataModel;
        
        private DungeonDataModel _dungeonDataModel;
        
        private RoomDataModel _roomDataModel;
        
        private EntityDataModel _entityDataModel;
        
        protected async override void AddLoadingResource()
        {
            _launchResourcesDataModel = this.GetModel<LaunchResourcesDataModel>();
            _universeDataModel = this.GetModel<UniverseDataModel>();
            _worldDataModel = this.GetModel<WorldDataModel>();
            _regionDataModel = this.GetModel<RegionDataModel>();
            _dungeonDataModel = this.GetModel<DungeonDataModel>();
            _roomDataModel = this.GetModel<RoomDataModel>();

            _entityDataModel = this.GetModel<EntityDataModel>();
            this.GetSystem<NewInputManager>().InitActionAsset();
            await LoadAllChunkDefJson();
        }
        
        
        private async UniTask LoadAllChunkDefJson()
        {
            await LoadPackageChunkDefJson();
            await LoadModChunkDefJson();
            LoadingComplete();
        }

        #region 本地

        private async UniTask LoadPackageChunkDefJson()
        {
            var package = this.GetSystem<YooAssetManager>().GetPackage();
            AssetInfo[] assetInfos = package.GetAssetInfos("ChunkData");
    
            LogKit.Log($"开始加载ChunkData,共 {assetInfos.Length} 个文件");

            foreach (var assetInfo in assetInfos)
            {
                var asset = package.LoadAssetAsync<TextAsset>(assetInfo.AssetPath);
                await asset;
        
                if (asset.AssetObject != null)
                {
                    TextAsset textAsset = asset.AssetObject as TextAsset;
                    string jsonContent = textAsset.text;
            
                    // 根据文件路径判断类型并加载
                    ProcessChunkDataByPath(assetInfo.AssetPath, jsonContent);
            
                    LogKit.Log($"已加载: {assetInfo.AssetPath}");
                }
            }
            
            LogKit.Log("ChunkData加载完成");
        }

        /// <summary>
        /// 根据文件路径判断类型并处理数据
        /// </summary>
        private void ProcessChunkDataByPath(string assetPath, string jsonContent)
        {
            string lowerPath = assetPath.ToLower();
            
            if (lowerPath.Contains("universe"))
            {
                ProcessUniverseData(jsonContent);
            }
            else if (lowerPath.Contains("world"))
            {
                ProcessWorldData(jsonContent);
            }
            else if (lowerPath.Contains("region"))
            {
                ProcessRegionData(jsonContent);
            }
            else if (lowerPath.Contains("dungeon"))
            {
                ProcessDungeonData(jsonContent);
            }
            else if (lowerPath.Contains("room"))
            {
                ProcessRoomData(jsonContent);
            }
            else
            {
                LogKit.Warning($"无法识别的文件类型: {assetPath}");
            }
        }

        private async UniTask LoadPackageEntityDefJson()
        {
            var package = this.GetSystem<YooAssetManager>().GetPackage();
            AssetInfo[] assetInfos = package.GetAssetInfos("EntityData");
    
            LogKit.Log($"EntityData,共 {assetInfos.Length} 个文件");

            foreach (var assetInfo in assetInfos)
            {
                var asset = package.LoadAssetAsync<TextAsset>(assetInfo.AssetPath);
                await asset;
        
                if (asset.AssetObject != null)
                {
                    TextAsset textAsset = asset.AssetObject as TextAsset;
                    string jsonContent = textAsset.text;
            
                    // 根据文件路径判断类型并加载
                    ProcessEntityDataByPath(assetInfo.AssetPath, jsonContent);
            
                    LogKit.Log($"已加载: {assetInfo.AssetPath}");
                }
            }
            
            LogKit.Log("ChunkData加载完成");
        }

        private void ProcessEntityDataByPath(string assetPath, string jsonContent)
        {
            
        }
        #endregion
        
        #region Mods

        /// <summary>
        /// 加载创意工坊的资源包
        /// </summary>
        private async UniTask LoadModChunkDefJson()
        {
            // TODO: 实现 Mod 加载逻辑
            await UniTask.Yield();
        }

        #endregion
        
        #region 各层级数据处理方法

        private void ProcessUniverseData(string jsonContent)
        {
            UniverseDtoDef dtoDef = Newtonsoft.Json.JsonConvert.DeserializeObject<UniverseDtoDef>(jsonContent);
            _universeDataModel.AddDtoDef(dtoDef);
            LogKit.Log($"已添加Universe: {dtoDef.DefId}");
        }
        
        private void ProcessWorldData(string jsonContent)
        {
            WorldDtoDef dtoDef = Newtonsoft.Json.JsonConvert.DeserializeObject<WorldDtoDef>(jsonContent);
            _worldDataModel.AddDtoDef(dtoDef);
            LogKit.Log($"已添加World: {dtoDef.DefId}");
        }
        
        private void ProcessRegionData(string jsonContent)
        {
            RegionDtoDef dtoDef = Newtonsoft.Json.JsonConvert.DeserializeObject<RegionDtoDef>(jsonContent);
            _regionDataModel.AddDtoDef(dtoDef);
            LogKit.Log($"已添加Region: {dtoDef.DefId}");
        }
        
        private void ProcessDungeonData(string jsonContent)
        {
            DungeonDtoDef dtoDef = Newtonsoft.Json.JsonConvert.DeserializeObject<DungeonDtoDef>(jsonContent);
            _dungeonDataModel.AddDtoDef(dtoDef);
            LogKit.Log($"已添加Dungeon: {dtoDef.DefId}");
        }

        private void ProcessRoomData(string jsonContent)
        {
            RoomDtoDef dtoDef = Newtonsoft.Json.JsonConvert.DeserializeObject<RoomDtoDef>(jsonContent);
            _roomDataModel.AddDtoDef(dtoDef);
            LogKit.Log($"已添加Room: {dtoDef.DefId}");
        }

        private void ProcessEntityData(string jsonContent)
        {
            EntityDtoDef dtoDef = Newtonsoft.Json.JsonConvert.DeserializeObject<EntityDtoDef>(jsonContent);
            _entityDataModel.AddDtoDef(dtoDef);
        }

        #endregion
    }
}