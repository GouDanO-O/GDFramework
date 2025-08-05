using UnityEngine;
using System.IO;
using GDFrameworkCore;
using System.Collections.Generic;
using Game.Models.Resource;
using GDFramework.Utility;
using GDFrameworkExtend.SingletonKit;
using Sirenix.OdinInspector;

namespace Game.World
{
    public class World : MonoSingleton<World>, IController
    {
        public WorldData currentWorldData;

        [Tooltip("JSON文件路径")]
        public string persistentDataPath = "Assets/Game/Res/Configs/WorldData/WorldPersistent.json";

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        private void Start()
        {
            InitWorldData();
            InitWorldComponent();
        }

        private void InitWorldData()
        {
            currentWorldData = LoadCompleteWorldData();
        }

        private void InitWorldComponent()
        {
        }

        [Button("解析世界数据json")]
        public void SetWorldData(TextAsset curAsset)
        {
            this.currentWorldData = LoadCompleteWorldData(curAsset);
        }
        
        public WorldData LoadCompleteWorldData(TextAsset curAsset)
        {
            if (curAsset != null && !string.IsNullOrEmpty(curAsset.text))
            {
                try
                {
                    WorldData loadedData = JsonUtility.FromJson<WorldData>(curAsset.text);

                    // 确保数据完整性
                    if (loadedData.worldDataPersistent == null)
                    {
                        loadedData.worldDataPersistent = CreateDefaultPersistentData();
                    }

                    if (loadedData.worldDataTemporary == null)
                    {
                        loadedData.worldDataTemporary = new WorldDataTemporary();
                    }

                    return loadedData;
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"加载世界数据失败: {e.Message}，创建默认数据");
                }
            }

            // 创建默认的完整世界数据
            return new WorldData
            {
                worldDataPersistent = CreateDefaultPersistentData(),
                worldDataTemporary = new WorldDataTemporary()
            };
        }

        /// <summary>
        /// 从文件加载完整的世界数据，如果文件不存在或数据不完整则创建默认数据
        /// </summary>
        private WorldData LoadCompleteWorldData()
        {
            TextAsset curAsset = this.GetModel<GameSceneResourcesDataModel>().WorldDataAsset;

            if (curAsset != null && !string.IsNullOrEmpty(curAsset.text))
            {
                try
                {
                    WorldData loadedData = JsonUtility.FromJson<WorldData>(curAsset.text);

                    // 确保数据完整性
                    if (loadedData.worldDataPersistent == null)
                    {
                        loadedData.worldDataPersistent = CreateDefaultPersistentData();
                    }

                    if (loadedData.worldDataTemporary == null)
                    {
                        loadedData.worldDataTemporary = new WorldDataTemporary();
                    }

                    return loadedData;
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"加载世界数据失败: {e.Message}，创建默认数据");
                }
            }

            // 创建默认的完整世界数据
            return new WorldData
            {
                worldDataPersistent = CreateDefaultPersistentData(),
                worldDataTemporary = new WorldDataTemporary()
            };
        }
        
        /// <summary>
        /// 保存完整的世界数据（包含固定数据和对局数据）
        /// </summary>
        [Button("保存世界数据")]
        public void SaveCompleteWorldData()
        {
            if (currentWorldData == null)
            {
                Debug.LogError("当前世界数据为空，无法保存");
                return;
            }

            // 验证数据
            if (!ValidateWorldData(currentWorldData.worldDataPersistent, out string errorMessage))
            {
                Debug.LogError($"数据验证失败: {errorMessage}");
                return;
            }

            string dirPath = Path.GetDirectoryName(persistentDataPath);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            // 保存完整的WorldData对象
            string json = JsonUtility.ToJson(currentWorldData, true);
            File.WriteAllText(persistentDataPath, json);
            Debug.Log("保存完整世界数据成功");
        }

        /// <summary>
        /// 创建默认的固定数据
        /// </summary>
        private WorldDataPersistent CreateDefaultPersistentData()
        {
            return new WorldDataPersistent
            {
                worldName = "New World",
                worldId = System.Guid.NewGuid().ToString()
            };
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
                if (string.IsNullOrEmpty(node.nodeDataPersistent.nodeId))
                {
                    errorMessage = $"房间 {roomData.roomDataPersistent.roomId} 中存在节点ID为空";
                    return false;
                }

                if (nodeIds.Contains(node.nodeDataPersistent.nodeId))
                {
                    errorMessage =
                        $"房间 {roomData.roomDataPersistent.roomId} 中存在重复的节点ID: {node.nodeDataPersistent.nodeId}";
                    return false;
                }

                nodeIds.Add(node.nodeDataPersistent.nodeId);
            }

            errorMessage = null;
            return true;
        }
    }
}