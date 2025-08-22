using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GDFramework.Utility;
using GDFrameworkExtend.Data;
using GDFrameworkExtend.JsonKit;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Game.World
{
    [Serializable, JsonObject]
    public class RoomDto : IHierarchicalDto
    {
        [LabelText("配置名称")] public string configName;

        [LabelText("配置ID(当前配置的ID,同一层必须唯一)"),
         OnValueChanged("OnRoomIdChange"),
         ValidateInput("ValidateConfigId", "配置ID不能包含下划线!", InfoMessageType.Error)]
        public string configId;

        [LabelText("用于存储和读取时使用的id(会进行层级拼接,防止重复)"),
         DisableInEditorMode]
        public string dtoId;

        [LabelText("配置描述")] public string configDes;

        [LabelText("房间固定数据")] 
        public RoomDataPersistent roomDataPersistent;
        [LabelText("房间对局数据")] 
        public RoomDataTemporary roomDataTemporary;

        #region EditorExtend

        [JsonIgnore, HideInInspector] private IHierarchicalDto parent;

        private List<NodeDto> _cachedNodes = new List<NodeDto>();

        // 实现 IHierarchicalDto 接口
        public string GetParentDtoId()
        {
            return parent?.GetType() == typeof(AreaBlockDto) ? ((AreaBlockDto)parent).dtoId : null;
        }

        public void SetParent(IHierarchicalDto parent)
        {
            this.parent = parent;
        }

        public void RefreshDtoId()
        {
            string parentId = GetParentDtoId();
            if (!string.IsNullOrEmpty(parentId) && !string.IsNullOrEmpty(configId))
            {
                this.dtoId = $"{parentId}_{configId}";
            }
            else if (!string.IsNullOrEmpty(configId))
            {
                this.dtoId = configId;
            }
        }

        public void RefreshChildrenDtoIds()
        {
            if (roomDataPersistent?.nodeDatas != null)
            {
                foreach (var node in roomDataPersistent.nodeDatas)
                {
                    node.SetParent(this);
                    node.RefreshDtoId();
                }
            }
        }

        public void AutoRefreshHierarchy()
        {
            RefreshDtoId();
            RefreshChildrenDtoIds();
            CheckForNodeChanges();
        }

        private void CheckForNodeChanges()
        {
            if (roomDataPersistent?.nodeDatas == null) return;

            var currentNodes = roomDataPersistent.nodeDatas;
            var newNodes = currentNodes.Except(_cachedNodes).ToList();

            // 为新增的节点设置父级关系
            foreach (var newNode in newNodes)
            {
                newNode.SetParent(this);
                newNode.AutoRefreshHierarchy();
            }

            _cachedNodes = new List<NodeDto>(currentNodes);
        }

#if UNITY_EDITOR
        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            // 检查节点列表变化
            if (roomDataPersistent?.nodeDatas != null)
            {
                bool needsRefresh = false;

                if (_cachedNodes.Count != roomDataPersistent.nodeDatas.Count)
                {
                    needsRefresh = true;
                }
                else
                {
                    for (int i = 0; i < roomDataPersistent.nodeDatas.Count; i++)
                    {
                        if (i >= _cachedNodes.Count ||
                            !ReferenceEquals(roomDataPersistent.nodeDatas[i], _cachedNodes[i]))
                        {
                            needsRefresh = true;
                            break;
                        }
                    }
                }

                if (needsRefresh)
                {
                    EditorApplication.delayCall += () => { AutoRefreshHierarchy(); };
                }
            }
        }
#endif
        
        private void OnRoomIdChange()
        {
            AutoRefreshHierarchy();
        }

        private bool ValidateConfigId(string id)
        {
            return !string.IsNullOrEmpty(id) && !id.Contains("_");
        }

        // 保留原有的 UpdateDtoId 方法以确保向后兼容
        public void UpdateDtoId(string areaBlockId)
        {
            this.dtoId = areaBlockId + "_" + this.configId;
            RefreshChildrenDtoIds();
        }
        #endregion
        
        public void SaveData(string directory, JsonSerializerSettings settings)
        {
            SaveData_Persistent(directory, settings);
        }

        public void SaveData_Persistent(string directory, JsonSerializerSettings settings)
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(configId))
                configId = "room_default";

            // 自动刷新ID级联
            AutoRefreshHierarchy();

            roomDataPersistent.nodeIds ??= new List<string>();
            roomDataPersistent.nodeDatas ??= new List<NodeDto>();
            roomDataPersistent.nodeIds.Clear();

            string roomDir = Path.Combine(directory, configId);
            Directory.CreateDirectory(roomDir);

            foreach (var node in roomDataPersistent.nodeDatas)
            {
                string nid = string.IsNullOrEmpty(node.configId) ? "node_auto" : node.configId;
                if (roomDataPersistent.nodeIds.Contains(nid))
                    LogMonoUtility.AddErrorLog($"重复的节点ID: {nid}");
                else
                    roomDataPersistent.nodeIds.Add(nid);

                node.SaveData_Persistent(roomDir, settings ?? JsonSettings.Make());
            }

            var persistentData = new
            {
                configName = this.configName,
                configId = this.configId,
                configDes = this.configDes,
                roomDataPersistent = this.roomDataPersistent
            };

            string filePath = Path.Combine(directory, $"{configId}.json");
            string json = JsonConvert.SerializeObject(persistentData, settings ?? JsonSettings.Make());
            File.WriteAllText(filePath, json);
            LogMonoUtility.AddLog($"保存房间固定数据 {filePath} 成功");
#else
        LogMonoUtility.AddWarning("SaveData_Persistent 只能在编辑器下使用，运行时固定数据为只读状态！");
#endif
        }

        public void SaveData_Temporary(string roomPath, JsonSerializerSettings settings)
        {
            if (string.IsNullOrEmpty(configId))
                configId = "room_default";

            try
            {
                var temporaryData = new
                {
                    configName = this.configName,
                    configId = this.dtoId,
                    configDes = this.configDes,
                    roomDataTemporary = this.roomDataTemporary
                };
                
                string roomRootDir = Path.Combine(roomPath, this.configId);
                Directory.CreateDirectory(roomRootDir);
                
                foreach (var room in roomDataPersistent.nodeDatas)
                {
                    room.SaveData_Temporary(roomRootDir,JsonSettings.Make());
                }
                
                string roomJsonPath = Path.Combine(roomPath, $"{dtoId}.json");
                File.WriteAllText(roomJsonPath, JsonConvert.SerializeObject(temporaryData, JsonSettings.Make()));
                LogMonoUtility.AddLog($"保存固定数据 {roomJsonPath} 成功");
            }
            catch (Exception ex)
            {
                LogMonoUtility.AddErrorLog($"保存房间临时数据失败: {ex.Message}");
            }
        }
    }

    [Serializable]
    public class RoomData
    {
        public RoomDto roomDto;
    }
}