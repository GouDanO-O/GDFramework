using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.World
{
    public enum TriggerConditionType
    {
        [LabelText("没有限制条件,直接满足")]
        Always,        
        [LabelText("拥有特定物品")]
        HasItem,      
        [LabelText("玩家等级")]
        PlayerLevel,   
    }
    
    [Serializable]
    [LabelText("行为触发条件")]
    public class ActionTriggerCondition
    {
        [LabelText("条件类型"),JsonConverter(typeof(StringEnumConverter))]
        public TriggerConditionType conditionType = TriggerConditionType.Always;

        [LabelText("需要的物品ID")]
        [ShowIf("conditionType", TriggerConditionType.HasItem)]
        public string requiredItemId;

        [LabelText("需要的数量")]
        [ShowIf("conditionType", TriggerConditionType.HasItem)]
        public int requiredAmount = 1;

        [LabelText("需要的等级")]
        [ShowIf("conditionType", TriggerConditionType.PlayerLevel)]
        public int requiredLevel = 1;
        
        /// <summary>
        /// 检查条件是否满足
        /// </summary>
        public bool CheckCondition()
        {
            switch (conditionType)
            {
                case TriggerConditionType.Always:
                    return true;
                case TriggerConditionType.HasItem:
                    // TODO: 实现物品检查逻辑
                    return true;
                case TriggerConditionType.PlayerLevel:
                    // TODO: 实现等级检查逻辑
                    return true;
                default:
                    return false;
            }
        }
    }
}



