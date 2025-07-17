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
    /// <summary>
    /// World->单个世界里面的所有区域
    /// Areas->单个区域里面的所有房间
    /// Rooms->单个房间里面的所有Nodes,这个房间里面会存储着如下所持有的所有节点数据
    /// 通过运行时或非运行时进行序列化存储
    /// 每次进入区域,首先,序列化所有房间,房间里面又存储
    /// 只存储当前节点的触发状态和位置
    /// </summary>
    [Serializable, LabelText("节点数据")]
    public class NodeData
    {
        [ShowInInspector] private NodeDataPersistent _nodeDataPersistent;

        [ShowInInspector] private NodeDataTemporary _nodeDataTemporary;

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
        /// 复制节点
        /// </summary>
        public NodeData(NodeData nodeData)
        {
            if (nodeData?._nodeDataPersistent != null)
            {
                _nodeDataPersistent = nodeData._nodeDataPersistent;
                _nodeDataTemporary = new NodeDataTemporary();

                _nodeDataTemporary.InitData();
            }
        }

        /// <summary>
        /// 能否进行互动
        /// </summary>
        /// <returns></returns>
        public bool CanTrigger()
        {
            return _nodeDataTemporary.curNodeState == ENodeState.Triggerable;
        }

        /// <summary>
        /// 能否进行移动
        /// </summary>
        /// <returns></returns>
        public bool CanMoveable()
        {
            return _nodeDataTemporary.curNodeState != ENodeState.Hidden || _nodeDataTemporary.curNodeState != ENodeState.Locked;
        }

        /// <summary>
        /// 检查触发条件
        /// </summary>
        /// <returns></returns>
        public bool CheckCondition()
        {
            return _nodeDataTemporary.curNodeState == ENodeState.Triggerable &&
                   _nodeDataPersistent.ActionTriggerData.CanTrigger();
        }
        
        /// <summary>
        /// 重置节点状态
        /// </summary>
        public void ResetNodeState()
        {
            if (_nodeDataTemporary != null)
            {

            }
        }

        /// <summary>
        /// 存储节点数据
        /// </summary>
        public void SaveNodeData()
        {
            _nodeDataPersistent.SaveData();
        }

        /// <summary>
        /// 销毁节点临时数据
        /// </summary>
        public void DestroyNodeData()
        {
            _nodeDataTemporary.DeInitData();
        }
    }
}