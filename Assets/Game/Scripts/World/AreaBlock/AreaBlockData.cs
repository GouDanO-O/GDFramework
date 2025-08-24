using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
    public class AreaBlockDto : IHierarchicalDto
    {
        [LabelText("配置名称")]
        public string configName;

        [LabelText("配置ID(当前配置的ID,同一层必须唯一)"),
         OnValueChanged("OnAreaBlockIdChange"),
         ValidateInput("ValidateConfigId", "配置ID不能包含下划线!", InfoMessageType.Error)]
        public string configId;

        [LabelText("用于存储和读取时使用的id(会进行层级拼接,防止重复)"), DisableInEditorMode]
        public string dtoId;

        [LabelText("配置描述")]
        public string configDes;

        [LabelText("地图区块固定数据")]
        public AreaBlockDataPersistent areaBlockDataPersistent;

        [LabelText("地图区块对局数据")]
        public AreaBlockDataTemporary areaBlockDataTemporary;

        #region EditorExtend

        [JsonIgnore, HideInInspector]
        private IHierarchicalDto parent;

        private List<RoomDto> _cachedRooms = new List<RoomDto>();

        // 实现 IHierarchicalDto 接口
        public string GetParentDtoId()
        {
            return parent?.GetType() == typeof(WorldDto) ? ((WorldDto)parent).dtoId : null;
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
            if (areaBlockDataPersistent?.roomDatas != null)
            {
                foreach (var room in areaBlockDataPersistent.roomDatas)
                {
                    room.SetParent(this);
                    room.RefreshDtoId();
                    room.RefreshChildrenDtoIds();
                }
            }
        }

        public void AutoRefreshHierarchy()
        {
            RefreshDtoId();
            RefreshChildrenDtoIds();
            CheckForRoomChanges();
        }

        private void CheckForRoomChanges()
        {
            if (areaBlockDataPersistent?.roomDatas == null) return;

            var currentRooms = areaBlockDataPersistent.roomDatas;
            var newRooms = currentRooms.Except(_cachedRooms).ToList();

            // 为新增的房间设置父级关系
            foreach (var newRoom in newRooms)
            {
                newRoom.SetParent(this);
                newRoom.AutoRefreshHierarchy();
            }

            _cachedRooms = new List<RoomDto>(currentRooms);
        }

#if UNITY_EDITOR
        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            // 检查房间列表变化
            if (areaBlockDataPersistent?.roomDatas != null)
            {
                bool needsRefresh = false;

                if (_cachedRooms.Count != areaBlockDataPersistent.roomDatas.Count)
                {
                    needsRefresh = true;
                }
                else
                {
                    for (int i = 0; i < areaBlockDataPersistent.roomDatas.Count; i++)
                    {
                        if (i >= _cachedRooms.Count ||
                            !ReferenceEquals(areaBlockDataPersistent.roomDatas[i], _cachedRooms[i]))
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

        private void OnAreaBlockIdChange()
        {
            AutoRefreshHierarchy();
        }

        private bool ValidateConfigId(string id)
        {
            return !string.IsNullOrEmpty(id) && !id.Contains("_");
        }

        // 保留原有的 UpdateDtoId 方法以确保向后兼容
        public void UpdateDtoId(string worldId)
        {
            this.dtoId = worldId + "_" + this.configId;
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
                configId = "area_default";

            // 自动刷新ID级联
            AutoRefreshHierarchy();

            areaBlockDataPersistent.roomIds ??= new List<string>();
            areaBlockDataPersistent.roomDatas ??= new List<RoomDto>();
            areaBlockDataPersistent.roomIds.Clear();

            string areaDir = Path.Combine(directory, configId);
            Directory.CreateDirectory(areaDir);

            foreach (var room in areaBlockDataPersistent.roomDatas)
            {
                string rid = string.IsNullOrEmpty(room.configId) ? "room_auto" : room.dtoId;
                if (areaBlockDataPersistent.roomIds.Contains(rid))
                    LogMonoUtility.AddErrorLog($"重复的房间ID: {rid}");
                else
                    areaBlockDataPersistent.roomIds.Add(rid);

                room.SaveData_Persistent(areaDir, settings ?? JsonSettings.Make());
            }

            var persistentData = new
            {
                configName = this.configName,
                configId = this.configId,
                dtoId = this.dtoId,
                configDes = this.configDes,
                areaBlockDataPersistent = this.areaBlockDataPersistent
            };

            string filePath = Path.Combine(directory, $"{dtoId}.json");
            string json = JsonConvert.SerializeObject(persistentData, settings ?? JsonSettings.Make());
            File.WriteAllText(filePath, json);
            LogMonoUtility.AddLog($"保存区块固定数据 {filePath} 成功");
#else
        LogMonoUtility.AddWarning("SaveData_Persistent 只能在编辑器下使用，运行时固定数据为只读状态！");
#endif
        }

        public void SaveData_Temporary(string areaBlockPath, JsonSerializerSettings settings)
        {
            if (string.IsNullOrEmpty(configId))
                configId = "area_default";

            try
            {
                var temporaryData = new
                {
                    configName = this.configName,
                    configId = this.dtoId,
                    configDes = this.configDes,
                    areaBlockDataTemporary = this.areaBlockDataTemporary
                };
                string areaRootDir = Path.Combine(areaBlockPath, this.configId);
                Directory.CreateDirectory(areaRootDir);

                foreach (var room in areaBlockDataPersistent.roomDatas)
                {
                    room.SaveData_Temporary(areaRootDir, JsonSettings.Make());
                }

                string areaJsonPath = Path.Combine(areaBlockPath, $"{dtoId}.json");
                File.WriteAllText(areaJsonPath, JsonConvert.SerializeObject(temporaryData, JsonSettings.Make()));
                LogMonoUtility.AddLog($"保存区块临时数据 {areaJsonPath} 成功");
            }
            catch (Exception ex)
            {
                LogMonoUtility.AddErrorLog($"保存区块临时数据失败: {ex.Message}");
            }
        }
    }

    [Serializable]
    public class AreaBlockData
    {
        public AreaBlockDto areaBlockDto;
    }
}