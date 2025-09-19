using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.World.Actions
{
    [Serializable]
    public abstract class NodeAction
    {
        // 在编辑器里显示的描述
        [TextArea(1, 2), LabelText("动作描述(仅编辑器)")] 
        public string description;
        
        // 运行时执行的逻辑
        public abstract void Execute();
    }
}