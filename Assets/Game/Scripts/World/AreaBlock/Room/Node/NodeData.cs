using System;
using System.Collections.Generic;
using System.IO;
using GDFramework.Utility;
using GDFrameworkCore;
using GDFrameworkExtend.Data;
using GDFrameworkExtend.EventKit;
using GDFrameworkExtend.JsonKit;
using GDFrameworkExtend.StorageKit;
using Newtonsoft.Json;
using NUnit.Framework;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.World
{
    [Serializable, JsonObject]
    public class NodeDto : IHierarchicalDto
    {
        [LabelText("配置名称")]
        public string configName;

        [LabelText("配置ID(当前配置的ID,同一层必须唯一)"),
         OnValueChanged("OnNodeIdChange"),
         ValidateInput("ValidateConfigId", "配置ID不能包含下划线!", InfoMessageType.Error)]
        public string configId;

        [LabelText("用于存储和读取时使用的id(会进行层级拼接,防止重复)"), DisableInEditorMode]
        public string dtoId;

        [LabelText("配置描述")]
        public string configDes;

        [LabelText("节点固定数据")]
        public NodeDataPersistent nodeDataPersistent;

        [LabelText("节点对局数据")]
        public NodeDataTemporary nodeDataTemporary;

        #region EditorExtend

        [JsonIgnore, HideInInspector]
        private IHierarchicalDto parent;

        // 实现 IHierarchicalDto 接口
        public string GetParentDtoId()
        {
            return parent?.GetType() == typeof(RoomDto) ? ((RoomDto)parent).dtoId : null;
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
            // Node 是叶子节点，没有子级
        }

        public void AutoRefreshHierarchy()
        {
            RefreshDtoId();
            // Node 没有子级，所以不需要刷新子级
        }

        private void OnNodeIdChange()
        {
            AutoRefreshHierarchy();
        }

        private bool ValidateConfigId(string id)
        {
            return !string.IsNullOrEmpty(id) && !id.Contains("_");
        }

        // 保留原有的 UpdateDtoId 方法以确保向后兼容
        public void UpdateDtoId(string roomId)
        {
            this.dtoId = roomId + "_" + this.configId;
        }

        #endregion


        #region Load & Save

        public void SaveData(string directory, JsonSerializerSettings settings)
        {
            SaveData_Persistent(directory, settings);
        }

        public void SaveData_Persistent(string directory, JsonSerializerSettings settings)
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(configId))
                configId = "node_default";

            // 自动刷新ID
            AutoRefreshHierarchy();

            Directory.CreateDirectory(directory);

            var persistentData = new
            {
                configName = this.configName,
                configId = this.configId,
                configDes = this.configDes,
                nodeDataPersistent = this.nodeDataPersistent
            };

            string filePath = Path.Combine(directory, $"{configId}.json");
            string json = JsonConvert.SerializeObject(persistentData, settings ?? JsonSettings.Make());
            File.WriteAllText(filePath, json);
            LogMonoUtility.AddLog($"保存节点固定数据 {filePath} 成功");
#else
        LogMonoUtility.AddWarning("SaveData_Persistent 只能在编辑器下使用，运行时固定数据为只读状态！");
#endif
        }

        public void SaveData_Temporary(string nodePath, JsonSerializerSettings settings = null)
        {
            if (string.IsNullOrEmpty(configId))
                configId = "node_default";

            try
            {
                var temporaryData = new
                {
                    configName = this.configName,
                    configId = this.dtoId,
                    configDes = this.configDes,
                    nodeDataTemporary = this.nodeDataTemporary
                };

                string nodeJsonPath = Path.Combine(nodePath, $"{dtoId}.json");
                File.WriteAllText(nodeJsonPath, JsonConvert.SerializeObject(temporaryData, JsonSettings.Make()));
                LogMonoUtility.AddLog($"保存固定数据 {nodeJsonPath} 成功");
            }
            catch (Exception ex)
            {
                LogMonoUtility.AddErrorLog($"保存节点临时数据失败: {ex.Message}");
            }
        }

        #endregion
    }

    /// <summary>
    /// World->单个世界里面的所有区域
    /// Areas->单个区域里面的所有房间
    /// Rooms->单个房间里面的所有Nodes,这个房间里面会存储着如下所持有的所有节点数据
    /// 通过运行时或非运行时进行序列化存储
    /// 每次进入区域,首先,序列化所有房间,房间里面又存储
    /// 只存储当前节点的触发状态和位置
    /// </summary>
    [Serializable, LabelText("节点数据")]
    public class NodeData
    {
        public NodeDto nodeDto;

        public void InitNodeData(Node node)
        {
        }

        /// <summary>
        /// 能否进行互动
        /// </summary>
        /// <returns></returns>
        public bool CanTrigger()
        {
            return true;
        }

        /// <summary>
        /// 能否进行移动
        /// </summary>
        /// <returns></returns>
        public bool CanMoveable()
        {
            return true;
        }

        /// <summary>
        /// 检查触发条件
        /// </summary>
        /// <returns></returns>
        public bool CheckCondition()
        {
            return true;
        }

        /// <summary>
        /// 重置节点状态
        /// </summary>
        public void ResetNodeState()
        {
        }

        /// <summary>
        /// 存储节点数据
        /// </summary>
        public void SaveNodeData()
        {
        }

        /// <summary>
        /// 销毁节点临时数据
        /// </summary>
        public void DestroyNodeData()
        {
        }

        public void ChangeTempPosition(Vector2 position)
        {
        }
    }
}