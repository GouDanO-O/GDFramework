using System;
using System.Collections.Generic;
using System.IO;
using Game.Models.Resource;
using GDFramework.Utility;
using GDFrameworkCore;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.World
{
    [Serializable]
    public class WorldDataModel : AbstractModel,ICanGetModel
    {
        [LabelText("JSON文件路径")]
        public string persistentDataPath = "Assets/Game/Res/Configs/WorldData/WorldData.json";
        
        [LabelText("世界固定数据")]
        public WorldDataPersistent worldDataPersistent;
        
        [LabelText("世界对局数据"),ReadOnly]
        public WorldDataTemporary worldDataTemporary;

        [LabelText("世界画布数据")]
        public WorldCanvasDataPersistent worldCanvasDataPersistent;
        
        private Dictionary<string,AreaBlockData> _areaBlockDataDict = new Dictionary<string, AreaBlockData>();

        protected override void OnInit()
        {
            
        }

        public AreaBlockData GetCurrentAreaBlockData(string areaBlockId)
        {
            if (worldDataPersistent == null)
            {
                LogMonoUtility.AddErrorLog("世界固定数据为空");
                return null;
            }

            if (worldDataPersistent.areaBlockDatas.Count == 0)
            {
                LogMonoUtility.AddErrorLog("世界中的区块数据为空");
                return null;
            }

            if (_areaBlockDataDict.Count == 0)
            {
                LogMonoUtility.AddErrorLog("区块字典为空");
                return null;
            }
            
            if (_areaBlockDataDict.ContainsKey(areaBlockId))
            {
                return _areaBlockDataDict[areaBlockId];
            }

            LogMonoUtility.AddErrorLog("区块字典未包含该ID");
            return null;
        }
        
        #region 解析世界数据
        
        /// <summary>
        /// 从文件加载完整的世界数据，如果文件不存在或数据不完整则创建默认数据
        /// </summary>
        public void LoadCompleteWorldData()
        {
            TextAsset curAsset = this.GetModel<GameSceneResourcesDataModel>().WorldDataAsset;
            LoadCompleteWorldData(curAsset);
        }
        
        public void LoadCompleteWorldData(TextAsset curAsset)
        {
            if (curAsset != null && !string.IsNullOrEmpty(curAsset.text))
            {
                try
                {
                    WorldDataModel loadedDataModel = JsonUtility.FromJson<WorldDataModel>(curAsset.text);
                    
                    // 确保数据完整性
                    if (loadedDataModel.worldDataPersistent == null)
                    {
                        worldDataPersistent = CreateDefaultPersistentData();
                    }
                    else
                    {
                        this.worldDataPersistent = loadedDataModel.worldDataPersistent;
                    }

                    if (loadedDataModel.worldDataTemporary == null)
                    {
                        worldDataTemporary = new WorldDataTemporary();
                    }
                    else
                    {
                        this.worldDataTemporary = loadedDataModel.worldDataTemporary;
                    }
                    
                    if (loadedDataModel.worldCanvasDataPersistent == null)
                    {
                        worldCanvasDataPersistent = new WorldCanvasDataPersistent();
                    }
                    else
                    {
                        this.worldCanvasDataPersistent = loadedDataModel.worldCanvasDataPersistent;
                    }
                    
                    _areaBlockDataDict.Clear();
                    for (int i = 0; i < this.worldDataPersistent.areaBlockDatas.Count; i++)
                    {
                        AreaBlockData areaBlockData = this.worldDataPersistent.areaBlockDatas[i];
                        string key = areaBlockData.areaBlockDataPersistent.areaBlockId;
                        _areaBlockDataDict.Add(key, areaBlockData);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"加载世界数据失败: {e.Message}，创建默认数据");
                }
            }
        }
        
        /// <summary>
        /// 保存完整的世界数据（包含固定数据和对局数据）
        /// </summary>
        [Button("保存世界数据")]
        public void SaveCompleteWorldData()
        {
            // 验证数据
            if (!ValidateWorldData(worldDataPersistent, out string errorMessage))
            {
                Debug.LogError($"数据验证失败: {errorMessage}");
                return;
            }

            string dirPath = Path.GetDirectoryName(persistentDataPath);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            // 保存完整的WorldData对象
            string json = JsonUtility.ToJson(this, true);
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

        #endregion
        
        
    }
}