using System.IO;
using GDFramework.Utility;
using GDFrameworkCore;
using UnityEngine;
using System;
using System.Collections.Generic;
using Game.World.Tools;

namespace Game.World
{
    public class WorldDataUtility  : IUtility
    {
        private WorldDataModel _worldDataModel;
        private ContentIndex _index;
        
        #region 解析世界数据
        
        public void LoadCompleteWorldData(WorldDataModel worldDataModel)
        {
            this._worldDataModel = worldDataModel;
            LoadWorldDataFromAb();
        }

        #region 从路径中进行加载

        private void LoadWorldDataFromFilePath()
        {
            
        }

        #endregion

        #region 从AB包中进行加载

        private void LoadWorldDataFromAb()
        {
            // 这里示例直接使用 StreamingAssets/BaseContent 和 持久化 Mods 目录
            string baseRoot = System.IO.Path.Combine(Application.streamingAssetsPath, "BaseContent");
            string modsRoot = System.IO.Path.Combine(Application.persistentDataPath, "Mods");
            var repo = new JsonPersistentRepo(baseRoot, modsRoot);
            _index = ContentIndex.Build(repo);
        }

        #endregion
        
        
        #endregion

        #region 保存世界数据

        /// <summary>
        /// 保存完整的世界数据
        /// </summary>
        public void SaveCompleteData()
        {
            if (this._worldDataModel != null)
            {
                SaveCompleteWorldData(this._worldDataModel);
            }
            else
            {
                LogMonoUtility.AddErrorLog("世界数据为空");
            }
        }
        
        public void SaveCompleteWorldData(WorldDataModel worldDataModel)
        {
            if (worldDataModel != null)
            {
                SaveWorldDataPersistent(worldDataModel);
            }
            else
            {
                LogMonoUtility.AddErrorLog("世界数据为空");
            }
        }

        /// <summary>
        /// 保存世界数据
        /// </summary>
        private void SaveWorldDataPersistent(WorldDataModel worldDataModel)
        {
            worldDataModel.SaveConfigData();
        }
        
        /// <summary>
        /// 保存所有区块数据
        /// </summary>
        private void SaveAllAreaBlockData()
        {
            
        }

        /// <summary>
        /// 保存所有房间数据
        /// </summary>
        private void SaveAllRoomData()
        {
            
        }

        /// <summary>
        /// 保存所有节点数据
        /// </summary>
        private void SaveAllNodeData()
        {
            
        }
        
        /// <summary>
        /// 保存当前区块数据
        /// </summary>
        /// <param name="areaBlockId"></param>
        public void SaveCurAreaBlockData(string areaBlockId)
        {
            
        }

        /// <summary>
        /// 保存当前房间数据
        /// </summary>
        /// <param name="roomId"></param>
        public void SaveCurRoomData(string roomId)
        {
            
        }

        /// <summary>
        /// 保存当前节点数据
        /// </summary>
        /// <param name="nodeId"></param>
        public void SaveCurNodeData(string nodeId)
        {
            
        }

        #endregion
    }

    public sealed class ContentIndex
    {
        public readonly Dictionary<string, WorldDef> worlds = new Dictionary<string, WorldDef>();
        public readonly Dictionary<string, AreaBlockDef> areaBlocks = new Dictionary<string, AreaBlockDef>();
        public readonly Dictionary<string, RoomDef> rooms = new Dictionary<string, RoomDef>();
        public readonly Dictionary<string, NodeDef> nodes = new Dictionary<string, NodeDef>();

        public readonly Dictionary<string, List<string>> children = new Dictionary<string, List<string>>(); // parentId -> children ids

        public WorldDef GetWorld(string id) => id != null && worlds.TryGetValue(id, out var v) ? v : null;
        public AreaBlockDef GetAreaBlock(string id) => id != null && areaBlocks.TryGetValue(id, out var v) ? v : null;
        public RoomDef GetRoom(string id) => id != null && rooms.TryGetValue(id, out var v) ? v : null;
        public NodeDef GetNode(string id) => id != null && nodes.TryGetValue(id, out var v) ? v : null;

        public IReadOnlyList<string> GetChildren(string parentId)
        {
            if (string.IsNullOrEmpty(parentId)) return Array.Empty<string>();
            return children.TryGetValue(parentId, out var list) ? (IReadOnlyList<string>)list : Array.Empty<string>();
        }

        public static ContentIndex Build(IPersistentRepo repo)
        {
            var idx = new ContentIndex();

            foreach (var w in repo.AllWorlds()) idx.worlds[w.id] = w;
            foreach (var ab in repo.AllAreaBlocks()) idx.areaBlocks[ab.id] = ab;
            foreach (var r in repo.AllRooms()) idx.rooms[r.id] = r;
            foreach (var n in repo.AllNodes()) idx.nodes[n.id] = n;

            void Link(string parent, IEnumerable<string> cs)
            {
                if (string.IsNullOrEmpty(parent) || cs == null) return;
                if (!idx.children.TryGetValue(parent, out var list))
                {
                    list = new List<string>();
                    idx.children[parent] = list;
                }
                foreach (var c in cs)
                {
                    if (!string.IsNullOrEmpty(c) && !list.Contains(c)) list.Add(c);
                }
            }

            foreach (var w in idx.worlds.Values) Link(w.id, w.areaBlockIds);
            foreach (var ab in idx.areaBlocks.Values) Link(ab.id, ab.roomIds);
            foreach (var r in idx.rooms.Values) Link(r.id, r.nodeIds);

            return idx;
        }
    }
}