using System;

namespace GDFramework.Mission
{
    [System.Serializable]
    public abstract class MissionRequirement<T>
    {
        /// <summary>
        /// 检查给定的消息是否对当前需求有效
        /// </summary>
        /// <param name="message">目标消息</param>
        /// <returns>是否有效</returns>
        public abstract bool CheckMessage(T message);

        /// <summary>
        /// 创建当前需求的状态记录柄
        /// </summary>
        /// <returns></returns>
        public MissionRequirementHandle<T> CreateHandle()
        {
            var _handleType = GetType().GetNestedType("Handle");
            if (_handleType == null)
                throw new Exception($"{GetType()} 未找到该定义");

            return (MissionRequirementHandle<T>)Activator.CreateInstance(_handleType, this);
        }
    }
}