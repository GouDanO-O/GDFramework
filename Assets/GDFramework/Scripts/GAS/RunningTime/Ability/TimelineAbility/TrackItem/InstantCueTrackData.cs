using System;
using System.Collections.Generic;
using GDFramework_Core.GAS.RunningTime.Cue.Base;

namespace GDFramework_Core.GAS.RunningTime.Ability.TimelineAbility.TrackItem
{
    [Serializable]
    public class InstantCueTrackData : TrackDataBase
    {
        public List<InstantCueMarkEvent> markEvents = new();

        public override void AddToAbilityAsset(TimelineAbilityAssetBase abilityAsset)
        {
            base.AddToAbilityAsset(abilityAsset);
            abilityAsset.InstantCues.Add(this);
        }
    }

    [Serializable]
    public class InstantCueMarkEvent : MarkEventBase
    {
        public List<GameplayCueInstant> cues = new();
    }
}