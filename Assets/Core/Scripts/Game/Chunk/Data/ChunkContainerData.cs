using System.Collections.Generic;
using Core.Game.Chunk.Data.Interface;

namespace Core.Game.Chunk.Data
{
    /// <summary>
    /// 容器运行时数据基类(可包含子级)
    /// </summary>
    public abstract class ChunkContainerData : ChunkData
    {
        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="childInstanceId"></param>
        public virtual void AddChild(string childInstanceId)
        {
            
        }

        /// <summary>
        /// 移除子节点
        /// </summary>
        /// <param name="childInstanceId"></param>
        public virtual void RemoveChild(string childInstanceId)
        {

        }
        
        /// <summary>
        /// 激活子节点
        /// </summary>
        /// <param name="childInstanceId"></param>
        public virtual void SetActiveChild(string childInstanceId)
        {

        }
    }
}