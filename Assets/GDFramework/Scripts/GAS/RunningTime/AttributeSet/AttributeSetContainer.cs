using System;
using System.Collections.Generic;
using GDFramework.GAS.RunningTime.Attribute;
using GDFramework.GAS.RunningTime.Attribute.Value;
using GDFramework.GAS.RunningTime.Component;

namespace GDFramework.GAS.RunningTime.AttributeSet
{
    public class AttributeSetContainer
    {
        private readonly AbilitySystemComponent _owner;

        private readonly Dictionary<string, AttributeSet> _attributeSetDict = new();

        private readonly Dictionary<AttributeBase, AttributeAggregator> _attributeAggregatorDict = new();

        public Dictionary<string, AttributeSet> AttributeSetDict => _attributeSetDict;

        public AttributeSetContainer(AbilitySystemComponent owner)
        {
            _owner = owner;
        }

        public void AddAttributeSet<T>() where T : AttributeSet
        {
            AddAttributeSet(typeof(T));
        }

        public void AddAttributeSet(Type attrSetType)
        {
            if (TryGetAttributeSet(attrSetType, out _)) return;
            var setName = AttributeSetUtility.AttributeSetName(attrSetType);
            _attributeSetDict.Add(setName, Activator.CreateInstance(attrSetType) as AttributeSet);

            var attrSet = _attributeSetDict[setName];
            foreach (var attr in attrSet.AttributeNames)
                if (!_attributeAggregatorDict.ContainsKey(attrSet[attr]))
                {
                    var attrAggt = new AttributeAggregator(attrSet[attr], _owner);
                    if (_owner.enabled) attrAggt.OnEnable();
                    _attributeAggregatorDict.Add(attrSet[attr], attrAggt);
                }

            attrSet.SetOwner(_owner);
        }

        /// <summary>
        /// 使用此方法时要小心
        /// 它可能会导致意想不到的错误（当使用网络同步时）。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RemoveAttributeSet<T>() where T : AttributeSet
        {
            var setName = AttributeSetUtility.AttributeSetName(typeof(T));
            var attrSet = _attributeSetDict[setName];
            foreach (var attr in attrSet.AttributeNames) _attributeAggregatorDict.Remove(attrSet[attr]);

            _attributeSetDict.Remove(setName);
        }

        public bool TryGetAttributeSet<T>(out T attributeSet) where T : AttributeSet
        {
            if (_attributeSetDict.TryGetValue(AttributeSetUtility.AttributeSetName(typeof(T)), out var set))
            {
                attributeSet = (T)set;
                return true;
            }

            attributeSet = null;
            return false;
        }

        private bool TryGetAttributeSet(Type attrSetType, out AttributeSet attributeSet)
        {
            if (_attributeSetDict.TryGetValue(AttributeSetUtility.AttributeSetName(attrSetType), out var set))
            {
                attributeSet = set;
                return true;
            }

            attributeSet = null;
            return false;
        }

        public AttributeValue? GetAttributeAttributeValue(string attrSetName, string attrShortName)
        {
            return _attributeSetDict.TryGetValue(attrSetName, out var set)
                ? set[attrShortName].Value
                : (AttributeValue?)null;
        }

        public CalculateMode? GetAttributeCalculateMode(string attrSetName, string attrShortName)
        {
            return _attributeSetDict.TryGetValue(attrSetName, out var set)
                ? set[attrShortName].CalculateMode
                : (CalculateMode?)null;
        }

        public float? GetAttributeBaseValue(string attrSetName, string attrShortName)
        {
            return _attributeSetDict.TryGetValue(attrSetName, out var set)
                ? set[attrShortName].BaseValue
                : (float?)null;
        }

        public float? GetAttributeCurrentValue(string attrSetName, string attrShortName)
        {
            return _attributeSetDict.TryGetValue(attrSetName, out var set)
                ? set[attrShortName].CurrentValue
                : (float?)null;
        }

        public Dictionary<string, float> Snapshot()
        {
            var snapshot = new Dictionary<string, float>();
            foreach (var attributeSet in _attributeSetDict)
            foreach (var name in attributeSet.Value.AttributeNames)
            {
                var attr = attributeSet.Value[name];
                snapshot.Add(attr.Name, attr.CurrentValue);
            }

            return snapshot;
        }

        public void OnDisable()
        {
            foreach (var aggregator in _attributeAggregatorDict)
                aggregator.Value.OnDisable();
        }

        public void OnEnable()
        {
            foreach (var aggregator in _attributeAggregatorDict)
                aggregator.Value.OnEnable();
        }
    }
}