using System.Collections.Generic;
using System.Linq;

namespace GDFramework.Mission
{
    public class Mission<T>
    {
        private readonly MissionPrototype<T> _missionPrototype;
        
        private readonly MissionRequirementHandle<T>[] _missionRequirementHandles;

        private readonly List<MissionRequirementHandle<T>> _unFinishedMissionRequirementHandles =
            new List<MissionRequirementHandle<T>>();

        public string MissionId => _missionPrototype.MissionId;
        
        public MissionAdditionalAttributeDes MissionAdditionalAttributeDes => _missionPrototype.MissionAdditionalAttributeDes;
        
        public Mission(MissionPrototype<T> prototype)
        {
            this._missionPrototype = prototype;
            _missionRequirementHandles = prototype.MissionRequirements.Select(r => r.CreateHandle()).ToArray();
            if (!prototype.IsSingleRequire)
                _unFinishedMissionRequirementHandles.AddRange(_missionRequirementHandles);
        }
        
        /// <summary>
        /// 获取任务的进度状态
        /// </summary>
        public string[] HandleStatus
        {
            get
            {
                var status = new string[_missionRequirementHandles.Length];
                for (var i = 0; i < _missionRequirementHandles.Length; i++)
                    status[i] = _missionRequirementHandles[i].ToString();
                return status;
            }
        }
        
        /// <summary>
        /// 兑现任务奖励
        /// </summary>
        public void ApplyReward() => _missionPrototype.ApplyReward();
        
        /// <summary>
        /// 向任务发送玩家行为消息并检查任务是否完成以及是否发生状态变化
        /// </summary>
        /// <param name="message">玩家行为消息</param>
        /// <param name="hasStatusChanged">任务是否产生状态变化</param>
        /// <returns>任务是否完成</returns>
        public bool SendMessage(T message, out bool hasStatusChanged) =>
            _missionPrototype.IsSingleRequire
                ? _SendMessage_SingleRequire(message, out hasStatusChanged)
                : _SendMessage_MultiRequire(message, out hasStatusChanged);

        /// <summary>
        /// 单需求的任务处理
        /// </summary>
        /// <param name="message"></param>
        /// <param name="hasStatusChanged"></param>
        /// <returns></returns>
        private bool _SendMessage_SingleRequire(T message, out bool hasStatusChanged) =>
            _missionRequirementHandles[0].SendMessage(message, out hasStatusChanged);

        /// <summary>
        /// 多需求任务处理
        /// </summary>
        /// <param name="message"></param>
        /// <param name="hasStatusChanged"></param>
        /// <returns></returns>
        private bool _SendMessage_MultiRequire(T message, out bool hasStatusChanged)
        {
            hasStatusChanged = false;
            var queueToRemove = new Queue<MissionRequirementHandle<T>>();
            
            //更新所有handle
            foreach (var requireHandle in _unFinishedMissionRequirementHandles)
            {
                if (!requireHandle.SendMessage(message, out var _hasStatusChanged))
                {
                    hasStatusChanged |= _hasStatusChanged;
                    continue;
                }
                hasStatusChanged = true;
                if (_missionPrototype.MissionRequireMode == EMissionRequireMode.Any) 
                    return true;
                queueToRemove.Enqueue(requireHandle);
            }
            
            //移除已经完成的handle
            while (queueToRemove.Count > 0)
            {
                var handle = queueToRemove.Dequeue();
                _unFinishedMissionRequirementHandles.Remove(handle);
            }
            
            return _unFinishedMissionRequirementHandles.Count == 0;
        }
    }
}