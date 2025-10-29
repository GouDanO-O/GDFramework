using System.Collections.Generic;

namespace Core.Game.Chunk.Data.Interface
{

    /// <summary>
    /// 可包含子级的运行时数据接口
    /// </summary>
    public interface IChunkContainerData : IChunkData
    {
        /// <summary>
        /// 添加子级实例ID
        /// </summary>
        void AddChild(string childInstanceId);
        
        /// <summary>
        /// 移除子级实例ID
        /// </summary>
        void RemoveChild(string childInstanceId);
        
        /// <summary>
        /// 获取所有子级实例ID
        /// </summary>
        List<string> GetAllChildIds();
        
        /// <summary>
        /// 设置激活的子级
        /// </summary>
        void SetActiveChild(string childInstanceId);
        
        /// <summary>
        /// 获取激活的子级ID
        /// </summary>
        string GetActiveChildId();
    }
}