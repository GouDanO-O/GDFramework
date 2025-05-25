using System;
using System.Collections.Generic;
using GDFrameworkExtend.Data;
using NUnit.Framework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.World
{
    [Serializable,LabelText("节点数据")]
    public class NodeData
    {
        [ShowInInspector]
        private NodeDataPersistent _nodeDataPersistent;
        
        [ShowInInspector]
        private NodeDataTemporary _nodeDataTemporary;
        
        public NodeDataPersistent NodeDataPersistent
        {
            get
            {
                return _nodeDataPersistent;
            }
        }

        public NodeDataTemporary NodeDataTemporary
        {
            get
            {
                return _nodeDataTemporary;
            }
        }

        public NodeData(NodeData nodeData)
        {
            
        }
    }
    
    [CreateAssetMenu(fileName = "NodeData",menuName = "NodeData")]
    [LabelText("节点固有属性,不会受到对局的影响而产生变化")]
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

    [Serializable,LabelText("节点临时属性,会受到对局的影响而产生变化")]
    public class NodeDataTemporary : TemporalityData
    {
        [LabelText("是否已经展示")]
        public bool hasBeanShowing;

        [LabelText("是否已经触发")]
        public bool hasBeanTrigger;
        
        public override void InitData()
        {
            
        }

        public override void DeInitData()
        {
            
        }
    }
}