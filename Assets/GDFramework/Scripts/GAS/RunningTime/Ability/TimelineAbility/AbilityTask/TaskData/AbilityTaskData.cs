using System;
using GDFramework.GAS.General.DataClass;
using UnityEngine;

namespace GDFramework.GAS.RunningTime.Ability.TimelineAbility.AbilityTask.TaskData
{
    [Serializable]
    public abstract class AbilityTaskData
    {
        public JsonData TaskData;

        public virtual AbilityTaskBase Create(AbilitySpec abilitySpec)
        {
            var task = Load();
            task.Init(abilitySpec);
            return task;
        }

        public void Save(AbilityTaskBase task)
        {
            var jsonData = JsonUtility.ToJson(task);
            var dataType = task.GetType().FullName;
            TaskData = new JsonData
            {
                Type = dataType,
                Data = jsonData
            };
        }

        public abstract AbilityTaskBase Load();
    }
}