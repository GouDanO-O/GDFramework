using GDFramework_Core.GAS.RunningTime.Effect.Modifier;
using Sirenix.OdinInspector;

namespace GDFramework_Core.GAS.RunningTime.Attribute
{
    public enum ECalculateMode
    {
        [LabelText(SdfIconType.Stack, Text = "叠加计算")]
        Stacking,

        [LabelText(SdfIconType.GraphDownArrow, Text = "取最小值")]
        MinValueOnly,

        [LabelText(SdfIconType.GraphUpArrow, Text = "取最大值")]
        MaxValueOnly,
    }
    
    public struct AttributeValue
    {
        public float BaseValue { get; private set; }
        public float CurrentValue { get; private set; }

        public float MinValue { get; private set; }
        public float MaxValue { get; private set; }

        public ECalculateMode CalculateMode { get; }
        public ESupportedOperation SupportedOperation { get; }
        
        public AttributeValue(float baseValue,
            ECalculateMode calculateMode = ECalculateMode.Stacking,
            ESupportedOperation supportedOperation = ESupportedOperation.All,
            float minValue = float.MinValue, float maxValue = float.MaxValue)
        {
            BaseValue = baseValue;
            SupportedOperation = supportedOperation;
            CurrentValue = baseValue;
            CalculateMode = calculateMode;
            MinValue = minValue;
            MaxValue = maxValue;
        }
        
        /// <summary>
        /// ignore min and max value, set current value directly
        /// </summary>
        public void SetCurrentValue(float value)
        {
            CurrentValue = value;
        }

        public void SetBaseValue(float value)
        {
            BaseValue = value;
        }

        public void SetMinValue(float min)
        {
            MinValue = min;
        }

        public void SetMaxValue(float max)
        {
            MaxValue = max;
        }

        public void SetMinMaxValue(float min, float max)
        {
            MinValue = min;
            MaxValue = max;
        }

        public bool IsSupportOperation(EGeOperation operation)
        {
            return SupportedOperation.HasFlag((ESupportedOperation)(1 << (int)operation));
        }
    }
}