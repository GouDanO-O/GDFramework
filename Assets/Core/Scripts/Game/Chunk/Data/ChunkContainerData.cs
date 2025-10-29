using System.Collections.Generic;
using Core.Game.Chunk.Data.Interface;

namespace Core.Game.Chunk.Data
{
    /// <summary>
    /// 容器运行时数据基类(可包含子级)
    /// </summary>
    public abstract class ChunkContainerData : ChunkData, IChunkContainerData
    {
        protected IChunkContainerTemporaryData ContainerTempData => TemporaryData as IChunkContainerTemporaryData;

        public virtual void AddChild(string childInstanceId)
        {
            if (ContainerTempData != null && !ContainerTempData.ChildInstanceIds.Contains(childInstanceId))
            {
                ContainerTempData.ChildInstanceIds.Add(childInstanceId);
                SaveTemporaryData();
                OnChildAdded(childInstanceId);
            }
        }

        public virtual void RemoveChild(string childInstanceId)
        {
            if (ContainerTempData != null && ContainerTempData.ChildInstanceIds.Contains(childInstanceId))
            {
                ContainerTempData.ChildInstanceIds.Remove(childInstanceId);

                // 如果移除的是激活的子级,清空激活状态
                if (ContainerTempData.ActiveChildInstanceId == childInstanceId)
                {
                    ContainerTempData.ActiveChildInstanceId = null;
                }

                SaveTemporaryData();
                OnChildRemoved(childInstanceId);
            }
        }

        public virtual List<string> GetAllChildIds()
        {
            return ContainerTempData?.ChildInstanceIds ?? new List<string>();
        }

        public virtual void SetActiveChild(string childInstanceId)
        {
            if (ContainerTempData != null && ContainerTempData.ChildInstanceIds.Contains(childInstanceId))
            {
                ContainerTempData.ActiveChildInstanceId = childInstanceId;
                SaveTemporaryData();
                OnActiveChildChanged(childInstanceId);
            }
        }

        public virtual string GetActiveChildId()
        {
            return ContainerTempData?.ActiveChildInstanceId;
        }

        protected virtual void OnChildAdded(string childInstanceId)
        {
        }

        protected virtual void OnChildRemoved(string childInstanceId)
        {
        }

        protected virtual void OnActiveChildChanged(string childInstanceId)
        {
        }
    }
}