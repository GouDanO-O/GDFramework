using GDFramework_Core.GAS.RunningTime.Ability;
using GameplayEffectSpec = GDFramework_Core.GAS.RunningTime.Effects.GameplayEffectSpec;

namespace GDFramework_Core.GAS.RunningTime.Cue.Base
{
    public struct GameplayCueParameters
    {
        public GameplayEffectSpec sourceGameplayEffectSpec;
        
        public AbilitySpec sourceAbilitySpec;
        
        public object[] customArguments;
        // AggregatedSourceTags
        // AggregatedTargetTags
        // EffectContext
        // Magnitude
    }
}