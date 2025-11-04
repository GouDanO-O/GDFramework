using System;
using System.Collections.Generic;
using System.Linq;
using Core.Game.Chunk.Dungeon.Data;
using Core.Game.Chunk.Region.Data;
using Core.Game.Chunk.Room.Data;
using Core.Game.Chunk.Universe.Data;
using Core.Game.Chunk.World.Data;
using Core.Game.Procedure.Models.Resource;
using Cysharp.Threading.Tasks;
using GDFramework.Resource;
using GDFramework.YooAssetKit;
using GDFrameworkCore;
using UnityEngine;
using UnityEngine.Events;
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
        
        protected async override void AddLoadingResource()
        {
            _launchResourcesDataModel = this.GetModel<LaunchResourcesDataModel>();
            _universeDataModel = this.GetModel<UniverseDataModel>();
            _worldDataModel = this.GetModel<WorldDataModel>();
            _regionDataModel = this.GetModel<RegionDataModel>();
            _dungeonDataModel = this.GetModel<DungeonDataModel>();
            _roomDataModel = this.GetModel<RoomDataModel>();
            await LoadAllUniverseDefJson();
        }
        
        private async UniTask LoadAllUniverseDefJson()
        {
            await LoadPackageUniverseDefJson();
            await LoadModUniverseDefJson();
            LoadingComplete();
        }

        /// <summary>
        /// 层级定义枚举
        /// </summary>
        public enum ChunkHierarchyLevel
        {
            Universe = 0,
            World = 1,
            Region = 2,
            Dungeon = 3,
            Room = 4
        }

        /// <summary>
        /// 层级节点信息
        /// </summary>
        public class HierarchyNode
        {
            public ChunkHierarchyLevel Level;
            
            public string Name;
            
            public Dictionary<string, HierarchyNode> Children = new Dictionary<string, HierarchyNode>();
            
            public List<AssetInfo> Assets = new List<AssetInfo>();
        }
        
        /// <summary>
        /// 层级上下文信息
        /// </summary>
        public class HierarchyContext
        {
            public string UniverseName;
            
            public string WorldName;
            
            public string RegionName;
            
            public string DungeonName;
            
            public string RoomName;
    
            public override string ToString()
            {
                var parts = new List<string>();
                if (!string.IsNullOrEmpty(UniverseName)) parts.Add($"Universe:{UniverseName}");
                if (!string.IsNullOrEmpty(WorldName)) parts.Add($"World:{WorldName}");
                if (!string.IsNullOrEmpty(RegionName)) parts.Add($"Region:{RegionName}");
                if (!string.IsNullOrEmpty(DungeonName)) parts.Add($"Dungeon:{DungeonName}");
                if (!string.IsNullOrEmpty(RoomName)) parts.Add($"Room:{RoomName}");
                return string.Join(" > ", parts);
            }
        }

        #region 本地

        private async UniTask LoadPackageUniverseDefJson()
        {
            var package = this.GetSystem<YooAssetManager>().GetPackage();
            AssetInfo[] assetInfos = package.GetAssetInfos("ChunkData");
    
            var hierarchyTree = BuildHierarchyTree(assetInfos);
    
            var context = new HierarchyContext();
            await LoadHierarchyTreeWithContext(package, hierarchyTree, 
                ChunkHierarchyLevel.Universe, context);
        }
        
        /// <summary>
        /// 构建层级树结构
        /// </summary>
        private Dictionary<string, HierarchyNode> BuildHierarchyTree(AssetInfo[] assetInfos)
        {
            var rootNodes = new Dictionary<string, HierarchyNode>();

            foreach (var assetInfo in assetInfos)
            {
                string[] pathParts = assetInfo.AssetPath.Split('/');
                int chunkDataIndex = Array.IndexOf(pathParts, "ChunkData");

                if (chunkDataIndex == -1) continue;

                // 获取ChunkData后的所有层级目录(不包括文件名)
                var levelNames = new List<string>();
                for (int i = chunkDataIndex + 1; i < pathParts.Length - 1; i++)
                {
                    levelNames.Add(pathParts[i]);
                }

                if (levelNames.Count == 0) continue;

                // 构建树结构
                Dictionary<string, HierarchyNode> currentLevel = rootNodes;

                for (int i = 0; i < levelNames.Count; i++)
                {
                    string levelName = levelNames[i];
                    ChunkHierarchyLevel hierarchyLevel = (ChunkHierarchyLevel)i;

                    if (!currentLevel.ContainsKey(levelName))
                    {
                        currentLevel[levelName] = new HierarchyNode
                        {
                            Level = hierarchyLevel,
                            Name = levelName
                        };
                    }

                    var node = currentLevel[levelName];

                    // 如果是最后一层,添加资源
                    if (i == levelNames.Count - 1)
                    {
                        node.Assets.Add(assetInfo);
                    }

                    currentLevel = node.Children;
                }
            }

            return rootNodes;
        }

        /// <summary>
        /// 递归加载层级树(带上下文版本)
        /// </summary>
        private async UniTask LoadHierarchyTreeWithContext(ResourcePackage package, 
            Dictionary<string, HierarchyNode> nodes, 
            ChunkHierarchyLevel currentLevel,
            HierarchyContext context)
        {
            foreach (var kvp in nodes.OrderBy(x => x.Key))
            {
                var node = kvp.Value;
                string indent = new string(' ', (int)currentLevel * 2);
        
                // 更新上下文
                var newContext = CloneContext(context);
                UpdateContext(newContext, currentLevel, node.Name);
        
                Debug.Log($"{indent}[{currentLevel}] 开始加载: {node.Name} (路径: {newContext})");
        
                // 加载当前节点的所有资源
                foreach (var assetInfo in node.Assets)
                {
                    var asset = package.LoadAssetAsync<TextAsset>(assetInfo.AssetPath);
                    await asset;
            
                    if (asset.AssetObject != null)
                    {
                        TextAsset textAsset = asset.AssetObject as TextAsset;
                        string jsonContent = textAsset.text;
                
                        // 根据层级类型处理不同的数据,并传入完整上下文
                        await ProcessChunkDataWithContext(currentLevel, newContext, 
                            assetInfo.AssetPath, jsonContent);
                
                        Debug.Log($"{indent}  已加载: {assetInfo.AssetPath}");
                    }
                }
        
                // 递归加载子节点
                if (node.Children.Count > 0)
                {
                    ChunkHierarchyLevel nextLevel = currentLevel + 1;
                    await LoadHierarchyTreeWithContext(package, node.Children, nextLevel, newContext);
                }
            }
        }
        
        private HierarchyContext CloneContext(HierarchyContext context)
        {
            return new HierarchyContext
            {
                UniverseName = context.UniverseName,
                WorldName = context.WorldName,
                RegionName = context.RegionName,
                DungeonName = context.DungeonName,
                RoomName = context.RoomName
            };
        }
        
        private void UpdateContext(HierarchyContext context, ChunkHierarchyLevel level, string name)
        {
            switch (level)
            {
                case ChunkHierarchyLevel.Universe:
                    context.UniverseName = name;
                    break;
                case ChunkHierarchyLevel.World:
                    context.WorldName = name;
                    break;
                case ChunkHierarchyLevel.Region:
                    context.RegionName = name;
                    break;
                case ChunkHierarchyLevel.Dungeon:
                    context.DungeonName = name;
                    break;
                case ChunkHierarchyLevel.Room:
                    context.RoomName = name;
                    break;
            }
        }

        /// <summary>
        /// 根据层级类型处理数据(带上下文版本)
        /// </summary>
        private async UniTask ProcessChunkDataWithContext(ChunkHierarchyLevel level, 
            HierarchyContext context, string assetPath, string jsonContent)
        {
            switch (level)
            {
                case ChunkHierarchyLevel.Universe:
                    ProcessUniverseDataWithContext(context, jsonContent);
                    break;
                case ChunkHierarchyLevel.World:
                    ProcessWorldDataWithContext(context, jsonContent);
                    break;
                case ChunkHierarchyLevel.Region:
                    ProcessRegionDataWithContext(context, jsonContent);
                    break;
                case ChunkHierarchyLevel.Dungeon:
                    ProcessDungeonDataWithContext(context, jsonContent);
                    break;
                case ChunkHierarchyLevel.Room:
                    ProcessRoomDataWithContext(context, jsonContent);
                    break;
            }
    
            await UniTask.Yield();
        }

        #endregion
        
        #region Mods

        /// <summary>
        /// 加载创意工坊的资源包
        /// </summary>
        private async UniTask LoadModUniverseDefJson()
        {
            
        }

        #endregion
        
        #region 各层级数据处理方法

        private void ProcessUniverseDataWithContext(HierarchyContext context, string jsonContent)
        {
            Debug.Log($"处理Universe数据: {context.UniverseName}");
            UniverseDtoDef dtoDef = Newtonsoft.Json.JsonConvert.DeserializeObject<UniverseDtoDef>(jsonContent);
            _universeDataModel.AddDtoDef(dtoDef);
        }
        
        private void ProcessWorldDataWithContext(HierarchyContext context, string jsonContent)
        {
            Debug.Log($"处理World数据: {context.WorldName}, 所属Universe: {context.UniverseName}");
            WorldDtoDef dtoDef = Newtonsoft.Json.JsonConvert.DeserializeObject<WorldDtoDef>(jsonContent);
            _worldDataModel.AddDtoDef(context,dtoDef);
        }
        
        private void ProcessRegionDataWithContext(HierarchyContext context, string jsonContent)
        {
            Debug.Log($"处理Region数据: {context.RegionName}, 所属World: {context.WorldName}");
            RegionDtoDef dtoDef = Newtonsoft.Json.JsonConvert.DeserializeObject<RegionDtoDef>(jsonContent);
            _regionDataModel.AddDtoDef(context,dtoDef);
        }
        
        private void ProcessDungeonDataWithContext(HierarchyContext context, string jsonContent)
        {
            Debug.Log($"处理Dungeon数据: {context.DungeonName}, Region: {context.RegionName}");
            DungeonDtoDef dtoDef = Newtonsoft.Json.JsonConvert.DeserializeObject<DungeonDtoDef>(jsonContent);
            _dungeonDataModel.AddDtoDef(context,dtoDef);
        }

        private void ProcessRoomDataWithContext(HierarchyContext context, string jsonContent)
        {
            Debug.Log($"处理Room数据: {context.RoomName}, 完整路径: {context}");
            RoomDtoDef dtoDef = Newtonsoft.Json.JsonConvert.DeserializeObject<RoomDtoDef>(jsonContent);
            _roomDataModel.AddDtoDef(context,dtoDef);
        }

        #endregion


        

    }
}