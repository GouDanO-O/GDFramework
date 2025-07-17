// 4. 添加 ActionTriggerData 和相关类

using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.World
{
    [Serializable]
    [LabelText("行为触发数据")]
    public class ActionTriggerData
    {
        [LabelText("触发条件")] 
        public ActionTriggerCondition actionTriggerCondition;

        [LabelText("是否震动屏幕")] 
        public bool willShakeScreen = false;

        [LabelText("震动强度")] [ShowIf("willShakeScreen")] [Range(1, 10)]
        public int shakeSceenStrength = 5;

        [LabelText("音频剪辑")] 
        public AudioClip audioClip;

        [LabelText("粒子对象")] 
        public GameObject particleObject;

        [LabelText("粒子位置偏移")] [ShowIf("particleObject")]
        public Vector2 particlePos = Vector2.zero;

        [LabelText("延迟时间")]
        public float delayTime = 0f;

        /// <summary>
        /// 检查是否可以触发
        /// </summary>
        public bool CanTrigger()
        {
            return actionTriggerCondition.CheckCondition();
        }

        /// <summary>
        /// 执行触发
        /// </summary>
        public void ExecuteTrigger()
        {
            if (CanTrigger())
            {
               
            }
        }
    }
}