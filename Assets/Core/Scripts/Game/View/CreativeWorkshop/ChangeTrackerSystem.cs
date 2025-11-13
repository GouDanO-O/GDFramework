using System.Collections.Generic;
using Core.Game.Chunk.Data.Interface;
using GDFrameworkCore;
using GDFrameworkExtend.LogKit;

namespace Core.Game.View
{
    public class ChangeTrackerSystem : AbstractSystem
    { 
        // 存储每个对象的原始快照
        private Dictionary<string, string> _snapshots = new Dictionary<string, string>();
        
        // 存储已修改的对象ID
        private HashSet<string> _changedIds = new HashSet<string>();

        protected override void OnInit()
        {
            
        }
        
        /// <summary>
        /// 开始追踪一个对象
        /// </summary>
        public void StartTracking(ITrackableData data, string id)
        {
            if (data == null || string.IsNullOrEmpty(id))
                return;
                
            _snapshots[id] = data.CreateSnapshot();
            _changedIds.Remove(id); // 重置修改状态
        }

        /// <summary>
        /// 批量开始追踪
        /// </summary>
        public void StartTrackingBatch(Dictionary<string, ITrackableData> dataDict)
        {
            foreach (var kvp in dataDict)
            {
                StartTracking(kvp.Value, kvp.Key);
            }
        }

        /// <summary>
        /// 检查对象是否有变化
        /// </summary>
        public bool HasChanges(ITrackableData data, string id)
        {
            if (data == null || string.IsNullOrEmpty(id))
                return false;
                
            if (!_snapshots.TryGetValue(id, out string snapshot))
                return true; // 没有快照,认为是新对象
                
            bool changed = data.HasChanges(snapshot);
            
            if (changed)
                _changedIds.Add(id);
            else
                _changedIds.Remove(id);
                
            return changed;
        }

        /// <summary>
        /// 检查是否有任何变化
        /// </summary>
        public bool HasAnyChanges()
        {
            return _changedIds.Count > 0;
        }

        /// <summary>
        /// 获取所有已修改的ID
        /// </summary>
        public List<string> GetChangedIds()
        {
            return new List<string>(_changedIds);
        }

        /// <summary>
        /// 更新快照(保存后调用)
        /// </summary>
        public void UpdateSnapshot(ITrackableData data, string id)
        {
            if (data == null || string.IsNullOrEmpty(id))
                return;
                
            _snapshots[id] = data.CreateSnapshot();
            _changedIds.Remove(id);
        }

        /// <summary>
        /// 批量更新快照
        /// </summary>
        public void UpdateSnapshotBatch(Dictionary<string, ITrackableData> dataDict)
        {
            foreach (var kvp in dataDict)
            {
                UpdateSnapshot(kvp.Value, kvp.Key);
            }
        }

        /// <summary>
        /// 重置追踪(丢弃所有变更)
        /// </summary>
        public void Reset()
        {
            _snapshots.Clear();
            _changedIds.Clear();
        }

        /// <summary>
        /// 移除追踪
        /// </summary>
        public void StopTracking(string id)
        {
            _snapshots.Remove(id);
            _changedIds.Remove(id);
        }

        /// <summary>
        /// 获取变更摘要
        /// </summary>
        public string GetChangeSummary()
        {
            if (_changedIds.Count == 0)
                return "无未保存的修改";
                
            return $"有 {_changedIds.Count} 个对象未保存";
        }

        /// <summary>
        /// 打印变更详情
        /// </summary>
        public void LogChanges()
        {
            if (_changedIds.Count == 0)
            {
                LogKit.Log("无未保存的修改");
                return;
            }

            LogKit.Log($"<color=yellow>未保存的修改 ({_changedIds.Count}):</color>");
            foreach (var id in _changedIds)
            {
                LogKit.Log($"  - {id}");
            }
        }


    }
}