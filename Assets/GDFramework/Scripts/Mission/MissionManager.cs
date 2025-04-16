using System;
using System.Collections.Generic;
using System.Linq;
using GDFrameworkCore;

namespace GDFramework.Mission
{
    public class MissionManager<T> 
    {
        private readonly Dictionary<string, Mission<T>> allMissions = new Dictionary<string, Mission<T>>();
        
        private readonly List<IMissionSystemComponent<T>> components = new List<IMissionSystemComponent<T>>();

        /// <summary>
        /// 启动目标任务
        /// </summary>
        /// <param name="proto"></param>
        /// <returns></returns>
        public bool StartMission(MissionPrototype<T> proto)
        {
            if (proto is null || allMissions.ContainsKey(proto.MissionId)) 
                return false;
            var mission = new Mission<T>(proto);
            allMissions.Add(proto.MissionId, mission);
            
            /* 通知所有的组件任务启动了 */
            foreach (var component in components)
                component.OnMissionStarted(mission);
            return true;
        }

        /// <summary>
        /// 查询所有符合条件的任务（条件为空时返回所有任务）
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public Mission<T>[] GetMissions(Func<MissionAdditionalAttributeDes, bool> condition = null)
        {
            return condition is null
                ? allMissions.Values.ToArray()
                : allMissions.Values.Where(m => condition(m.MissionAdditionalAttributeDes)).ToArray();
        }

        /// <summary>
        /// 查找对应id的任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Mission<T> GetMission(string id)
        {
            return string.IsNullOrEmpty(id) ? null : allMissions.GetValueOrDefault(id, null);
        }

        /// <summary>
        /// 移除目标任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool RemoveMission(string id)
        {
            if (!allMissions.Remove(id, out var mission)) return false;
            foreach (var component in components)
                component.OnMissionRemoved(mission, false);
            return true;
        }
        
        /// <summary>
        /// 向任务系统发送消息以驱动任务系统
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(T message)
        {
            if (allMissions.Count == 0) 
                return;
            var queueToRemove = new Queue<Mission<T>>(); 
            foreach (var mission in allMissions.Values)
            {
                if (!mission.SendMessage(message, out var hasStatusChanged))
                {
                    if (hasStatusChanged) _OnMissionStatusChanged(mission, false);
                    continue;
                }

                _OnMissionStatusChanged(mission, true);
                mission.ApplyReward();
                queueToRemove.Enqueue(mission);
            }
            
            /* remove completed missions */
            while (queueToRemove.Count > 0)
            {
                var mission = queueToRemove.Dequeue();
                allMissions.Remove(mission.MissionId);
                
                /* inform all componetns that target mission has been removed */
                foreach (var component in components)
                    component.OnMissionRemoved(mission, true);
            }
        }
        
        /// <summary>
        /// 添加任务系统组件
        /// </summary>
        /// <param name="component"></param>
        public bool AddComponent(IMissionSystemComponent<T> component)
        {
            if (component is null || components.Contains(component))
                return false;
            components.Add(component);
            return true;
        }

        /// <summary>
        /// 移除任务组件
        /// </summary>
        /// <param name="component"></param>
        public bool RemoveComponent(IMissionSystemComponent<T> component)
        {
            return component is not null && components.Remove(component);
        }

        private void _OnMissionStatusChanged(Mission<T> mission, bool isFinished)
        {
            foreach (var component in components)
                component.OnMissionStatusChanged(mission, isFinished);
        }
    }
}