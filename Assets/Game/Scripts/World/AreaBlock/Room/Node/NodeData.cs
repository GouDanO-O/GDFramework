using System;
using System.Collections.Generic;
using GDFrameworkCore;
using GDFrameworkExtend.Data;
using GDFrameworkExtend.EventKit;
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
        private NodeDataPersistent _nodeDataPersistent;
        
        [ShowInInspector, LabelText("节点临时属性")]
        private BindableProperty<NodeDataTemporary> _nodeDataTemporary;
        
        public NodeDataPersistent NodeDataPersistent
        {
            get { return _nodeDataPersistent; }
        }

        public NodeDataTemporary NodeDataTemporary
        {
            get { return _nodeDataTemporary.Value; }
        }

        public void InitNodeData(Node node)
        {
            if (this._nodeDataPersistent == null)
            {
                this._nodeDataPersistent = new NodeDataPersistent();
            }

            if (this._nodeDataTemporary == null)
            {
                this._nodeDataTemporary= new BindableProperty<NodeDataTemporary>();
                this._nodeDataTemporary.SetValueWithoutEvent(new NodeDataTemporary());
                node.GetSystem<StorageKit>().RegisterSaveableObject(_nodeDataTemporary);
            }

            this._nodeDataTemporary.Register(OnNodeDataTemporaryChanged);
        }
        
        private void OnNodeDataTemporaryChanged(NodeDataTemporary newData)
        {
            // 处理临时数据变化的逻辑
            Debug.Log($"节点临时数据发生变化: {newData}");
        }
        
        /// <summary>
        /// 能否进行互动
        /// </summary>
        /// <returns></returns>
        public bool CanTrigger()
        {
            return _nodeDataTemporary.Value.curNodeState == ENodeState.Triggerable;
        }

        /// <summary>
        /// 能否进行移动
        /// </summary>
        /// <returns></returns>
        public bool CanMoveable()
        {
            return _nodeDataTemporary.Value.curNodeState != ENodeState.Hidden || _nodeDataTemporary.Value.curNodeState != ENodeState.Locked;
        }

        /// <summary>
        /// 检查触发条件
        /// </summary>
        /// <returns></returns>
        public bool CheckCondition()
        {
            return _nodeDataTemporary.Value.curNodeState == ENodeState.Triggerable &&
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
            _nodeDataTemporary.Value.curNodePosition = position;
        }
    }
}