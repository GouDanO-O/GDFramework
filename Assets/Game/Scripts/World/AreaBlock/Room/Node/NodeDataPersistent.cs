// 3. 修正 NodeDataPersistent 中的泛型声明

using System;
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
    [Serializable]
    public class NodeDataPersistent
    {
        
        [LabelText("节点触发时会发生的效果")]
        public ActionTriggerData actionTriggerData;
        
    }
}