using System;
using GDFrameworkExtend.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.World
{
    /// <summary>
    /// 从玩家第一次进入游戏后,就会存在的数据,会受到对局的影响而产生变化
    /// 除非重新开始对局或者清除数据,否则会一直存在
    /// </summary>
    [Serializable,LabelText("节点临时属性,会受到对局的影响而产生变化")]
    public class NodeDataTemporary : TemporalityData
    {
        [LabelText("当前节点的状态")]
        public ENodeState curNodeState;
        
        [LabelText("节点所处的位置")]
        public Vector2 curNodePosition;
        
        /// <summary>
        /// 改变节点状态
        /// </summary>
        /// <param name="newState"></param>
        public void ChangeNodeState(ENodeState newState)
        {
            curNodeState = newState;
        }
    }
}