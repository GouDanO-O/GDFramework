using System;
using System.Collections.Generic;
using System.Linq;
using GDFramework_Core.GAS.RunningTime.Component;
using GDFramework_Core.GAS.RunningTime.Effect.Modifier;
using UnityEngine;

namespace GDFramework_Core.GAS.RunningTime.Attribute
{
    public class AttributeBase
    {
        public readonly string Name;
        
        public readonly string SetName;
        
        public readonly string ShortName;
        
        protected event Action<AttributeBase, float, float> OnPostCurrentValueChange;
        
        protected event Action<AttributeBase, float, float> OnPostBaseValueChange;
        
        protected event Action<AttributeBase, float> OnPreCurrentValueChange;
        
        protected event Func<AttributeBase, float, float> OnPreBaseValueChange;
        
        protected IEnumerable<Func<AttributeBase, float, float>> PreBaseValueChangeListeners;
        
        private AttributeValue _value;
        
        private AbilitySystemComponent Owner;
        
        public AbilitySystemComponent Owner => Owner;
        
        public AttributeBase(string attrSetName, string attrName, float value = 0,
            ECalculateMode calculateMode = ECalculateMode.Stacking,
            ESupportedOperation supportedOperation = ESupportedOperation.All,
            float minValue = float.MinValue, float maxValue = float.MaxValue)
        {
            SetName = attrSetName;
            Name = $"{attrSetName}.{attrName}";
            ShortName = attrName;
            _value = new AttributeValue(value, calculateMode, supportedOperation, minValue, maxValue);
        }
        
           public AttributeValue Value => _value;
        public float BaseValue => _value.BaseValue;
        public float CurrentValue => _value.CurrentValue;

        public float MinValue => _value.MinValue;
        public float MaxValue => _value.MaxValue;

        public ECalculateMode CalculateMode => _value.CalculateMode;
        public ESupportedOperation SupportedOperation => _value.SupportedOperation;

        public void SetOwner(AbilitySystemComponent owner)
        {
            Owner = owner;
        }

        public void SetMinValue(float min)
        {
            _value.SetMinValue(min);
        }

        public void SetMaxValue(float max)
        {
            _value.SetMaxValue(max);
        }

        public void SetMinMaxValue(float min, float max)
        {
            _value.SetMinValue(min);
            _value.SetMaxValue(max);
        }
        
        public bool IsSupportOperation(EGeOperation operation)
        {
            return _value.IsSupportOperation(operation);
        }

        public void SetCurrentValue(float value)
        {
            value = Mathf.Clamp(value, _value.MinValue, _value.MaxValue);

            OnPreCurrentValueChange?.Invoke(this, value);

            var oldValue = CurrentValue;
            _value.SetCurrentValue(value);

            if (!Mathf.Approximately(oldValue, value)) OnPostCurrentValueChange?.Invoke(this, oldValue, value);
        }

        public void SetBaseValue(float value)
        {
            if (OnPreBaseValueChange != null)
            {
                value = InvokePreBaseValueChangeListeners(value);
            }

            var oldValue = _value.BaseValue;
            _value.SetBaseValue(value);

            if (!Mathf.Approximately(oldValue, value)) OnPostBaseValueChange?.Invoke(this, oldValue, value);
        }

        public void SetCurrentValueWithoutEvent(float value)
        {
            _value.SetCurrentValue(value);
        }

        public void SetBaseValueWithoutEvent(float value)
        {
            _value.SetBaseValue(value);
        }

        public void RegisterPreBaseValueChange(Func<AttributeBase, float, float> func)
        {
            OnPreBaseValueChange += func;
            _preBaseValueChangeListeners =
                OnPreBaseValueChange?.GetInvocationList().Cast<Func<AttributeBase, float, float>>();
        }

        public void RegisterPostBaseValueChange(Action<AttributeBase, float, float> action)
        {
            OnPostBaseValueChange += action;
        }

        public void RegisterPreCurrentValueChange(Action<AttributeBase, float> action)
        {
            OnPreCurrentValueChange += action;
        }

        public void RegisterPostCurrentValueChange(Action<AttributeBase, float, float> action)
        {
            OnPostCurrentValueChange += action;
        }

        public void UnregisterPreBaseValueChange(Func<AttributeBase, float, float> func)
        {
            OnPreBaseValueChange -= func;
            _preBaseValueChangeListeners =
                OnPreBaseValueChange?.GetInvocationList().Cast<Func<AttributeBase, float, float>>();
        }

        public void UnregisterPostBaseValueChange(Action<AttributeBase, float, float> action)
        {
            OnPostBaseValueChange -= action;
        }

        public void UnregisterPreCurrentValueChange(Action<AttributeBase, float> action)
        {
            OnPreCurrentValueChange -= action;
        }

        public void UnregisterPostCurrentValueChange(Action<AttributeBase, float, float> action)
        {
            OnPostCurrentValueChange -= action;
        }

        public virtual void Dispose()
        {
            OnPreBaseValueChange = null;
            OnPostBaseValueChange = null;
            OnPreCurrentValueChange = null;
            OnPostCurrentValueChange = null;
        }

        private float InvokePreBaseValueChangeListeners(float value)
        {
            if (_preBaseValueChangeListeners == null) return value;

            foreach (var t in _preBaseValueChangeListeners)
                value = t.Invoke(this, value);
            return value;
        }
    }
}