// 2. 完善 NodeData 构造函数
using System;
using System.Collections.Generic;
using GDFrameworkExtend.Data;
using NUnit.Framework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.World
{
    [Serializable, LabelText("节点数据")]
    public class NodeData
    {
        [ShowInInspector]
        private NodeDataPersistent _nodeDataPersistent;

        [ShowInInspector]
        private NodeDataTemporary _nodeDataTemporary;

        public NodeDataPersistent NodeDataPersistent
        {
            get { return _nodeDataPersistent; }
        }

        public NodeDataTemporary NodeDataTemporary
        {
            get { return _nodeDataTemporary; }
        }

        /// <summary>
        /// 从持久化数据创建节点数据
        /// </summary>
        public NodeData(NodeDataPersistent nodeDataPersistent)
        {
            _nodeDataPersistent = nodeDataPersistent;
            _nodeDataTemporary = new NodeDataTemporary();
            _nodeDataTemporary.InitData();
        }

        /// <summary>
        /// 复制构造函数
        /// </summary>
        public NodeData(NodeData nodeData)
        {
            if (nodeData?._nodeDataPersistent != null)
            {
                _nodeDataPersistent = nodeData._nodeDataPersistent;
                _nodeDataTemporary = new NodeDataTemporary();
                
                // 复制临时数据状态
                if (nodeData._nodeDataTemporary != null)
                {
                    _nodeDataTemporary.hasBeanShowing = nodeData._nodeDataTemporary.hasBeanShowing;
                    _nodeDataTemporary.hasBeanTrigger = nodeData._nodeDataTemporary.hasBeanTrigger;
                }
                
                _nodeDataTemporary.InitData();
            }
        }

        /// <summary>
        /// 重置节点状态
        /// </summary>
        public void ResetNodeState()
        {
            if (_nodeDataTemporary != null)
            {
                _nodeDataTemporary.hasBeanShowing = false;
                _nodeDataTemporary.hasBeanTrigger = false;
            }
        }

        /// <summary>
        /// 销毁节点数据
        /// </summary>
        public void DestroyNodeData()
        {
            _nodeDataTemporary?.DeInitData();
        }
    }
}