using System;
using System.Collections.Generic;
using GDFramework_Core.GAS.RunningTime.Component;
using GDFramework_Core.GAS.RunningTime.Effect;
using GDFramework_Core.GAS.RunningTime.Effect.Modifier;

namespace GDFramework_Core.GAS.RunningTime.Attribute
{
    public class AttributeAggregator
    {
        AttributeBase _processedAttribute;
        
        AbilitySystemComponent _owner;
        
        /// <summary>
        ///  modifiers的顺序很重要，因为modifiers的执行是按照顺序来的。
        /// </summary>
        private List<Tuple<GameplayEffectSpec, GameplayEffectModifier>> _modifierCache =
            new List<Tuple<GameplayEffectSpec, GameplayEffectModifier>>();

    }
}