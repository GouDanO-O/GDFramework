using System;
using System.Diagnostics.CodeAnalysis;

namespace GDFramework.Mission
{
    /// <summary>
    /// 任务原型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MissionPrototype<T>
    {
        public readonly string MissionId;
        
        public readonly MissionAdditionalAttributeDes MissionAdditionalAttributeDes; 
        
        public readonly MissionRequirement<T>[] MissionRequirements;
        
        public readonly EMissionRequireMode MissionRequireMode;
        
        public readonly bool IsSingleRequire;
        
        public readonly MissionReward[] MissionRewards;
        
        /// <summary>
        /// 初始化任务原型
        /// </summary>
        /// <param name="id"></param>
        /// <param name="requires"></param>
        /// <param name="rewards"></param>
        /// <param name="requireMode"></param>
        /// <param name="property"></param>
        /// <exception cref="Exception"></exception>
        public MissionPrototype(string id, [DisallowNull] MissionRequirement<T>[] requires, MissionReward[] rewards = null, EMissionRequireMode requireMode = default, MissionAdditionalAttributeDes additionalAttributeDes = null)
        {
            if (string.IsNullOrEmpty(id)) 
                throw new Exception("任务id不能为空或空");
            this.MissionId = id;
            
            if (requires == null || requires.Length == 0)
                throw new Exception("任务要求不能为null或空");

            this.MissionRequirements = requires;
            this.MissionRewards = rewards;
            this.MissionRequireMode = requireMode;
            this.MissionAdditionalAttributeDes = additionalAttributeDes;
            
            this.IsSingleRequire = requires.Length == 1;
        }
        
        /// <summary>
        /// 兑现所有的奖励
        /// </summary>
        public void ApplyReward()
        {
            if (MissionRewards is null || MissionRewards.Length == 0) 
                return;
            foreach (var reward in MissionRewards)
                reward.ApplyReward();
        }
    }
}