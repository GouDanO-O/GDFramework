using System.Collections.Generic;
using UnityEngine;

namespace GDFramework.Mission
{
    [CreateAssetMenu]
    public class SingleMission : ScriptableObject 
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string taskID;         
        
        /// <summary>
        /// 任务名称
        /// </summary>
        public string title;
        
        /// <summary>
        /// 描述
        /// </summary>
        public string description;   
        
        /// <summary>
        /// 任务类型
        /// </summary>
        public EMissionType type;       
        
        /// <summary>
        ///  当前状态（未接受/进行中/已完成/已提交）
        /// </summary>
        public EMissionStatus state;       
    }
}