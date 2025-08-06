using System;
using GDFramework.Asset;
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
        public int shakeScreenStrength = 5;

        [LabelText("音频剪辑唯一ID"),AssetIDSelector(EAssetGroupType.Music)] 
        public string audioClipId;

        [LabelText("粒子对象唯一ID"),AssetIDSelector(EAssetGroupType.Particle)] 
        public string particleObjectId;
        
        [LabelText("对象唯一ID"),AssetIDSelector(EAssetGroupType.Prefabs)] 
        public string testobjId;

        [LabelText("粒子位置偏移")] [ShowIf("particleObjectId")]
        public Vector2 particleObjectOffset = Vector2.zero;

        [LabelText("触发延迟时间")]
        public float triggerDelayTime = 0f;

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