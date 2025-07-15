// 3. 修正 NodeDataPersistent 中的泛型声明
using System.Collections.Generic;
using GDFrameworkExtend.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.World
{
    [CreateAssetMenu(fileName = "NodeDataPersistent", menuName = "NodeDataPersistent")]
    public class NodeDataPersistent : ConfigData
    {
        [LabelText("节点ID(对玩家不可见)")]
        public string nodeId;

        [LabelText("节点名称")]
        public string nodeName;

        [LabelText("节点所处的位置")]
        public Vector2 nodePosition;

        [LabelText("父节点属性")]
        public NodeDataPersistent parentNodeData;

        [LabelText("拥有的子节点")]
        public List<NodeDataPersistent> childNodeDataList = new List<NodeDataPersistent>();

        [FormerlySerializedAs("actionNodeTriggerData")]
        [LabelText("节点触发时会发生的效果")]
        public ActionTriggerData actionTriggerData;

        /// <summary>
        /// 获取所有子节点ID
        /// </summary>
        public List<string> GetChildNodeIds()
        {
            List<string> childIds = new List<string>();
            if (childNodeDataList != null)
            {
                foreach (var child in childNodeDataList)
                {
                    if (child != null)
                        childIds.Add(child.nodeId);
                }
            }
            return childIds;
        }

        /// <summary>
        /// 添加子节点
        /// </summary>
        public void AddChildNode(NodeDataPersistent childNode)
        {
            if (childNode != null && !childNodeDataList.Contains(childNode))
            {
                childNodeDataList.Add(childNode);
                childNode.parentNodeData = this;
            }
        }

        /// <summary>
        /// 移除子节点
        /// </summary>
        public void RemoveChildNode(NodeDataPersistent childNode)
        {
            if (childNode != null && childNodeDataList.Contains(childNode))
            {
                childNodeDataList.Remove(childNode);
                if (childNode.parentNodeData == this)
                    childNode.parentNodeData = null;
            }
        }

        /// <summary>
        /// 检查是否是根节点
        /// </summary>
        public bool IsRootNode()
        {
            return parentNodeData == null;
        }

        /// <summary>
        /// 检查是否是叶节点
        /// </summary>
        public bool IsLeafNode()
        {
            return childNodeDataList == null || childNodeDataList.Count == 0;
        }
    }
}