using System;
using System.Collections.Generic;
using Game.World.Actions;
using GDFramework.Asset;
using GDFrameworkExtend.JsonKit;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.World
{
    [Serializable,JsonObject]
    [LabelText("行为触发数据")]
    public class ActionTriggerData
    {
        [LabelText("触发条件")] 
        public List<ActionTriggerCondition> conditions = new List<ActionTriggerCondition>();
        
        [LabelText("触发延迟时间")]
        public float triggerDelayTime = 0f;
        
        [SerializeReference, LabelText("触发后执行的动作列表")]
        public List<NodeAction> actions = new List<NodeAction>();
        
        public bool CanTrigger()
        {
            foreach (var condition in conditions)
            {
                if (!condition.CheckCondition())
                {
                    return false;
                }
            }
            return true;
        }
    }
}