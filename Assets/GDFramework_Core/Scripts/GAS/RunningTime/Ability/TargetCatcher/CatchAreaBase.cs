using System;
using GDFramework_Core.GAS.RunningTime.Component;
using UnityEngine;

namespace GDFramework_Core.GAS.RunningTime.Ability.TargetCatcher
{
    [Serializable]
    public abstract class CatchAreaBase : TargetCatcherBase
    {
        public LayerMask checkLayer;

        public void Init(AbilitySystemComponent owner, LayerMask checkLayer) 
        {
            base.Init(owner);
            this.checkLayer = checkLayer;
        }
    }
}