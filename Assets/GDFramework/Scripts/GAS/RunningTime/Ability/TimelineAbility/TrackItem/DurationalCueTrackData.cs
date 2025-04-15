using System;
using System.Collections.Generic;
using GDFramework.GAS.RunningTime.Cue.Base;

namespace GDFramework.GAS.RunningTime.Ability.TimelineAbility.TrackItem
{
    [Serializable]
    public class DurationalCueTrackData : TrackDataBase
    {
        public List<DurationalCueClipEvent> clipEvents = new();

        public override void AddToAbilityAsset(TimelineAbilityAssetBase abilityAsset)
        {
            base.AddToAbilityAsset(abilityAsset);
            abilityAsset.DurationalCues.Add(this);
        }
    }

    [Serializable]
    public class DurationalCueClipEvent : ClipEventBase
    {
        public GameplayCueDurational cue;
    }
}