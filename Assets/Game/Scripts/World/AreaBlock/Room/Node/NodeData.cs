// 2. 完善 NodeData 构造函数

using System;
using System.Collections.Generic;
using GDFrameworkExtend.Data;
using GDFrameworkExtend.StorageKit;
using NUnit.Framework;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
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
    [ES3Serializable, LabelText("节点数据")]
    public class NodeData
    {
        [ShowInInspector,LabelText("节点固定属性")] 
        private NodeDataPersistent _nodeDataPersistent = new NodeDataPersistent();

        [ShowInInspector,LabelText("节点临时属性")] 
        private NodeDataTemporary _nodeDataTemporary = new NodeDataTemporary();

        public NodeDataPersistent NodeDataPersistent
        {
            get { return _nodeDataPersistent; }
        }

        public NodeDataTemporary NodeDataTemporary
        {
            get { return _nodeDataTemporary; }
        }

        public void InitNodeData()
        {
            
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
            
        }

        /// <summary>
        /// 销毁节点临时数据
        /// </summary>
        public void DestroyNodeData()
        {
            
        }

        public void ChangeTempPosition(Vector2 position)
        {
            _nodeDataTemporary.curNodePosition = position;
        }
    }
}