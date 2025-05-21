using System.Collections.Generic;
using GDFrameworkExtend.Data;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.World
{
    /// <summary>
    /// 基础行为节点数据
    /// 可以进行持久化存储
    /// </summary>
    public class ActionNodeData : PersistentData
    {
        [LabelText("父节点(仅会存在一个父节点)")]
        public ActionNodeData ParentNodeData;

        [LabelText("子节点(同时会存在多个子节点)")]
        public List<ActionNodeData> ChildNodeDataList;

        [LabelText("节点位置(当玩家移动后,进行更新)")]
        public Vector2 NodePosition;

        [LabelText("是否被展示(节点是否被父节点生成)")]
        public bool HasBeenDisplayed;

        [LabelText("是否被触发")]
        public bool HasBeanTrigger;

        [LabelText("行为触发器")]
        public ActionNodeTrigger ActionNodeTrigger;
        
        
    }
    
    /// <summary>
    /// 基础行为Node
    /// 每个房间里面由n个节点Node组成
    /// </summary>
    public abstract class ActionNode
    {
        public ActionNodeData ActionNodeData;

        public void InitActionNode()
        {
            
        }
    }
}

