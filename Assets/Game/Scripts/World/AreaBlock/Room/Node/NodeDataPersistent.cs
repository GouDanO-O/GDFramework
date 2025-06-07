using System.Collections.Generic;
using GDFrameworkExtend.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.World
{
    [CreateAssetMenu(fileName = "NodeDataPersistent",menuName = "NodeDataPersistent")]
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
        public List<NodeDataPersistent> childNodeDataList;
        
        [FormerlySerializedAs("actionNodeTriggerData")] [LabelText("节点触发时会发生的效果")]
        public ActionTriggerData actionTriggerData;
    }
}