using System;
using System.Collections.Generic;
using UnityEngine.Serialization;
using GDFramework.GAS.RunningTime.Effects;

namespace GDFramework.GAS.RunningTime.Ability.TimelineAbility.TrackItem
{
    [Serializable]
    public class BuffGameplayEffectTrackData : TrackDataBase
    {
        public List<BuffGameplayEffectClipEvent> clipEvents = new();

        public override void AddToAbilityAsset(TimelineAbilityAssetBase abilityAsset)
        {
            base.AddToAbilityAsset(abilityAsset);
            abilityAsset.BuffGameplayEffects.Add(this);
        }

        public override void DefaultInit()
        {
            base.DefaultInit();
            trackName = "Buff";
        }
    }

    [Serializable]
    public class BuffGameplayEffectClipEvent : ClipEventBase
    {
        public BuffTarget buffTarget;

        [FormerlySerializedAs("gameplayEffects")]
        public GameplayEffectAsset gameplayEffect;
    }

    public enum BuffTarget
    {
        Self
    }
}