using System;
using GDFrameworkExtend.Data;
using Sirenix.OdinInspector;

namespace Game.World
{
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