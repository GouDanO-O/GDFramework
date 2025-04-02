using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using GDFramework_Core.GAS.RunningTime.AttributeSet;

namespace GDFramework_Core.GAS.RunningTime.Component
{
    /// <summary>
    /// 能力系统组件
    /// </summary>
    public class AbilitySystemComponent  : MonoBehaviour, IAbilitySystemComponent
    {
        [SerializeField]
        private AbilitySystemComponentPreset preset;
        
        public AbilitySystemComponentPreset Preset => preset;
        
        public int Level { get; protected set; }
        
        public GameplayEffectContainer GameplayEffectContainer { get; private set; }
        
        public GameplayTagAggregator GameplayTagAggregator { get; private set; }
        
        public AbilityContainer AbilityContainer { get; private set; }

        public AttributeSetContainer AttributeSetContainer { get; private set; }
        
        private bool _isReady;

        private void Init()
        {
            if(_isReady)
                return;

            AttributeSetContainer = new AttributeSetContainer(this);
        }

        private void Enable()
        {
            
        }
        
        public void SetPreset(AbilitySystemComponentPreset ascPreset)
        {
            throw new NotImplementedException();
        }

        public void Init(GameplayTag[] baseTag, Type[] attributeTypes, AbilityAsset[] baseAbilities, int level)
        {
            throw new NotImplementedException();
        }

        public void SetLevel(int level)
        {
            throw new NotImplementedException();
        }

        public bool HasTag(GameplayTag tag)
        {
            throw new NotImplementedException();
        }

        public bool HasAllTags(GameplayTagSet tags)
        {
            throw new NotImplementedException();
        }

        public bool HasAnyTags(GameplayTagSet tags)
        {
            throw new NotImplementedException();
        }

        public void AddFixedTag(GameplayTag tags)
        {
            throw new NotImplementedException();
        }

        public void AddFixedTags(GameplayTagSet tags)
        {
            throw new NotImplementedException();
        }

        public void RemoveFixedTag(GameplayTag gameplayTag)
        {
            throw new NotImplementedException();
        }

        public void RemoveFixedTags(GameplayTagSet tags)
        {
            throw new NotImplementedException();
        }

        public GameplayEffectSpec ApplyGameplayEffectTo(GameplayEffect gameplayEffect, AbilitySystemComponent target)
        {
            throw new NotImplementedException();
        }

        public GameplayEffectSpec ApplyGameplayEffectToSelf(GameplayEffect gameplayEffect)
        {
            throw new NotImplementedException();
        }

        public void ApplyModFromInstantGameplayEffect(GameplayEffectSpec spec)
        {
            throw new NotImplementedException();
        }

        public void RemoveGameplayEffect(GameplayEffectSpec spec)
        {
            throw new NotImplementedException();
        }

        public void Tick()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, float> DataSnapshotDict()
        {
            throw new NotImplementedException();
        }

        public AbilitySpec GrantAbility(AbstractAbility ability)
        {
            throw new NotImplementedException();
        }

        public void RemoveAbility(string abilityName)
        {
            throw new NotImplementedException();
        }

        public float? GetAttributeCurrentValue(string setName, string attributeShortName)
        {
            throw new NotImplementedException();
        }

        public float? GetAttributeBaseValue(string setName, string attributeShortName)
        {
            throw new NotImplementedException();
        }

        public bool TryActivateAbility(string abilityName, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void TryEndAbility(string abilityName)
        {
            throw new NotImplementedException();
        }

        public CooldownTimer CheckCooldownFromTags(GameplayTagSet tags)
        {
            throw new NotImplementedException();
        }

        public T AttrSet<T>() where T : AttributeSet
        {
            throw new NotImplementedException();
        }

        public void ClearGameplayEffect()
        {
            throw new NotImplementedException();
        }
    }
}