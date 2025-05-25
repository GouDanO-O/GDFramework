using System;
using GDFrameworkExtend.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.World
{
    [Serializable,LabelText("行为触发时发生的效果")]
    public class ActionTriggerData : ConfigData
    {
        [FormerlySerializedAs("actionNodeTriggerCondition")] [LabelText("行为触发条件")]
        public ActionTriggerCondition actionTriggerCondition;
        
        [LabelText("震动屏幕")]
        public bool willShakeScreen = false;

        [ShowIf("willShakeScreen"),LabelText("震动强度")]
        public int shakeSceenStrength;

        [LabelText("触发时播放的音频")]
        public AudioClip audioClip;

        [LabelText("触发时生成的粒子特效")]
        public GameObject particleObject;
        
        [ShowIf("$particleObject"),LabelText("生成粒子特效的位置)(默认为触发节点周围)")]
        public Vector3 particlePos = Vector3.zero;
    }
}