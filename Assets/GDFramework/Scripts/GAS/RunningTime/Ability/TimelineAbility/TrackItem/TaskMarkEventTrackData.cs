using System;
using System.Collections.Generic;
using GDFramework.GAS.RunningTime.Ability.TimelineAbility.AbilityTask.TaskData;

namespace GDFramework.GAS.RunningTime.Ability.TimelineAbility.TrackItem
{
    [Serializable]
    public class TaskMarkEventTrackData : TrackDataBase
    {
        public List<TaskMarkEvent> markEvents = new();

        public override void AddToAbilityAsset(TimelineAbilityAssetBase abilityAsset)
        {
            base.AddToAbilityAsset(abilityAsset);
            abilityAsset.InstantTasks.Add(this);
        }
    }

    [Serializable]
    public class TaskMarkEvent : MarkEventBase
    {
        public List<InstantTaskData> InstantTasks = new();
    }
}