using UnityEngine;
using System.IO;
using GDFrameworkCore;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Game.World
{
    public class World : MonoBehaviour, IController
    {
        public WorldData currentWorldData;
        
        // 在编辑器中可配置的路径
        [Tooltip("JSON文件路径 (相对StreamingAssets)")]
        public string persistentDataPath = "WorldData/world_persistent.json";
        [Tooltip("运行时数据路径 (相对Application.persistentDataPath)")]
        public string temporaryDataPath = "WorldData/world_temporary.json";
        
        public IArchitecture GetArchitecture() => new GameMain();

        private void Start()
        {
            InitWorldData();
            InitWorldComponent();
        }

        private void InitWorldData()
        {
            currentWorldData = new WorldData
            {
                worldDataPersistent = LoadPersistentData(),
                worldDataTemporary = LoadTemporaryData()
            };
        }

        private void InitWorldComponent()
        {
            // UI初始化代码将放在这里
        }
        
        // JSON持久化操作 ---------------------------------
        [Button("加载json")]
        public WorldDataPersistent LoadPersistentData()
        {
            string fullPath = Path.Combine(Application.streamingAssetsPath, persistentDataPath);
            
            if (File.Exists(fullPath))
            {
                string json = File.ReadAllText(fullPath);
                return JsonUtility.FromJson<WorldDataPersistent>(json);
            }
            
            // 创建默认数据
            var defaultData = new WorldDataPersistent
            {
                worldName = "New World",
                worldId = System.Guid.NewGuid().ToString()
            };
            
            SavePersistentData(defaultData);
            return defaultData;
        }

        [Button("保存世界固有数据")]
        public void SavePersistentData()
        {
            if (!ValidateWorldData(currentWorldData.worldDataPersistent, out string errorMessage))
            {
                Debug.LogError($"数据验证失败: {errorMessage}");
                return;
            }
            
            string dirPath = Path.GetDirectoryName(Path.Combine(Application.streamingAssetsPath, persistentDataPath));
            if (!Directory.Exists(dirPath)) 
                Directory.CreateDirectory(dirPath);
            
            string fullPath = Path.Combine(Application.streamingAssetsPath, persistentDataPath);
            File.WriteAllText(fullPath, JsonUtility.ToJson(currentWorldData, true));
            Debug.Log("保存数据");
        }
        

        public void SavePersistentData(WorldDataPersistent data)
        {
            if (!ValidateWorldData(data, out string errorMessage))
            {
                Debug.LogError($"数据验证失败: {errorMessage}");
                return;
            }
            
            string dirPath = Path.GetDirectoryName(Path.Combine(Application.streamingAssetsPath, persistentDataPath));
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            
            string fullPath = Path.Combine(Application.streamingAssetsPath, persistentDataPath);
            File.WriteAllText(fullPath, JsonUtility.ToJson(data, true));
        }

        public WorldDataTemporary LoadTemporaryData()
        {
            string fullPath = Path.Combine(Application.persistentDataPath, temporaryDataPath);
            
            if (File.Exists(fullPath))
            {
                string json = File.ReadAllText(fullPath);
                return JsonUtility.FromJson<WorldDataTemporary>(json);
            }
            return new WorldDataTemporary();
        }

        public void SaveTemporaryData(WorldDataTemporary data)
        {
            string fullPath = Path.Combine(Application.persistentDataPath, temporaryDataPath);
            string dirPath = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            
            File.WriteAllText(fullPath, JsonUtility.ToJson(data, true));
        }
        
        private bool ValidateWorldData(WorldDataPersistent data, out string errorMessage)
        {
            // 1. 验证世界ID
            if (string.IsNullOrEmpty(data.worldId))
            {
                errorMessage = "世界ID不能为空";
                return false;
            }
            
            // 2. 验证区块ID唯一性
            var blockIds = new HashSet<string>();
            foreach (var block in data.areaBlockDatas)
            {
                if (block.areaBlockDataPersistent == null)
                {
                    errorMessage = "区块数据不能为空";
                    return false;
                }
                
                string blockId = block.areaBlockDataPersistent.areaBlockId;
                
                if (string.IsNullOrEmpty(blockId))
                {
                    errorMessage = "区块ID不能为空";
                    return false;
                }
                
                if (blockIds.Contains(blockId))
                {
                    errorMessage = $"区块ID重复: {blockId}";
                    return false;
                }
                blockIds.Add(blockId);
                
                // 3. 验证房间ID唯一性
                if (!ValidateRoomData(block.areaBlockDataPersistent, out errorMessage))
                {
                    return false;
                }
            }
            
            errorMessage = null;
            return true;
        }
        
        private bool ValidateRoomData(AreaBlockDataPersistent blockData, out string errorMessage)
        {
            var roomIds = new HashSet<string>();
            
            foreach (var room in blockData.roomDatas)
            {
                if (string.IsNullOrEmpty(room.roomDataPersistent.roomId))
                {
                    errorMessage = $"区块 {blockData.areaBlockId} 中存在房间ID为空";
                    return false;
                }
                
                if (roomIds.Contains(room.roomDataPersistent.roomId))
                {
                    errorMessage = $"区块 {blockData.areaBlockId} 中存在重复的房间ID: {room.roomDataPersistent.roomId}";
                    return false;
                }
                roomIds.Add(room.roomDataPersistent.roomId);
                
                // 4. 验证节点ID唯一性
                if (!ValidateNodeData(room, out errorMessage))
                {
                    return false;
                }
            }
            
            errorMessage = null;
            return true;
        }
        
        /// <summary>
        /// 验证节点数据ID的唯一性
        /// </summary>
        private bool ValidateNodeData(RoomData roomData, out string errorMessage)
        {
            var nodeIds = new HashSet<string>();
            
            foreach (var node in roomData.roomDataPersistent.NodeDatas)
            {
                if (string.IsNullOrEmpty(node.NodeDataPersistent.nodeId))
                {
                    errorMessage = $"房间 {roomData.roomDataPersistent.roomId} 中存在节点ID为空";
                    return false;
                }
                
                if (nodeIds.Contains(node.NodeDataPersistent.nodeId))
                {
                    errorMessage = $"房间 {roomData.roomDataPersistent.roomId} 中存在重复的节点ID: {node.NodeDataPersistent.nodeId}";
                    return false;
                }
                nodeIds.Add(node.NodeDataPersistent.nodeId);
            }
            
            errorMessage = null;
            return true;
        }
        
        // 编辑器工具方法 --------------------------------
        #if UNITY_EDITOR
        [ContextMenu("Save Current World Data")]
        public void SaveCurrentWorldData()
        {
            if (currentWorldData == null) return;
            
            SavePersistentData(currentWorldData.worldDataPersistent);
            SaveTemporaryData(currentWorldData.worldDataTemporary);
            Debug.Log("World data saved!");
        }
        
        [ContextMenu("Reload World Data")]
        public void ReloadWorldData()
        {
            currentWorldData.worldDataPersistent = LoadPersistentData();
            currentWorldData.worldDataTemporary = LoadTemporaryData();
            Debug.Log("World data reloaded!");
        }
        #endif
    }
}