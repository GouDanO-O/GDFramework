// 3. 修正 NodeDataPersistent 中的泛型声明
using System.Collections.Generic;
using GDFrameworkExtend.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.World
{
    /// <summary>
    /// 游戏节点固定数据
    /// 不会受到玩家行为的变更而产生变化
    /// </summary>
    [CreateAssetMenu(fileName = "NodeDataPersistent", menuName = "节点固定数据")]
    public class NodeDataPersistent : ConfigData
    {
        [LabelText("节点ID(对玩家不可见)")]
        public string NodeId;

        [LabelText("节点名称")]
        public string NodeName;
        
        [LabelText("拥有的子节点ID")]
        public List<string> ChildNodeDataList = new List<string>();
        
        [LabelText("节点触发时会发生的效果")]
        public ActionTriggerData ActionTriggerData;
    }
}