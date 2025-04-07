using System.Collections.Generic;
using GDFramework_Core.GAS.General;
using GDFramework_Core.GAS.RunningTime.Component;
using GDFramework;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine.Profiling;

namespace GDFramework_Core.GAS.RunningTime.Core
{
    public class GameplayAbilitySystem : Singleton<GameplayAbilitySystem>, IController
    {
        [ShowInInspector] [LabelText("是否开启Profiler调试")]
        private bool willShowProfiler = false;

        private const int AbilityCapacity = 1024;

        public List<AbilitySystemComponent> AbilitySystemComponentList { get; }

        private readonly List<AbilitySystemComponent> _cachedAbilitySystemComponentList;

        private GameplayAbilitySystem()
        {
            AbilitySystemComponentList = new List<AbilitySystemComponent>(AbilityCapacity);
            _cachedAbilitySystemComponentList = new List<AbilitySystemComponent>(AbilityCapacity);

            GasTimer.InitStartTimestamp();
        }

        public IArchitecture GetArchitecture()
        {
            return Main.Interface;
        }

        /// <summary>
        /// 注册技能组件
        /// </summary>
        /// <param name="abilitySystemComponent"></param>
        public void RegisterAbilitySystemComponent(AbilitySystemComponent abilitySystemComponent)
        {
            if (AbilitySystemComponentList.Contains(abilitySystemComponent))
                return;
            AbilitySystemComponentList.Add(abilitySystemComponent);
        }

        /// <summary>
        /// 注销技能组件
        /// </summary>
        /// <param name="abilitySystemComponent"></param>
        /// <returns></returns>
        public bool UnregisterAbilitySystemComponent(AbilitySystemComponent abilitySystemComponent)
        {
            return AbilitySystemComponentList.Remove(abilitySystemComponent);
        }

        public void Tick()
        {
            if (willShowProfiler) ProfilerAnalysis();
        }

        private void ProfilerAnalysis()
        {
            Profiler.BeginSample($"{nameof(GameplayAbilitySystem)}::Tick()");

            _cachedAbilitySystemComponentList.Clear();
            _cachedAbilitySystemComponentList.AddRange(AbilitySystemComponentList);

            foreach (var abilitySystemComponent in _cachedAbilitySystemComponentList) abilitySystemComponent.Tick();

            _cachedAbilitySystemComponentList.Clear();

            Profiler.EndSample();
        }
    }
}