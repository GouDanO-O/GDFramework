///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

using System;
using System.Collections.Generic;

namespace GAS.Runtime
{
    public class AS_Test : AttributeSet
    {
        #region Test

        /// <summary>
        /// 
        /// </summary>
        public AttributeBase Test { get; } = new ("AS_Test", "Test", 0f, CalculateMode.Stacking, (SupportedOperation)31, 0, float.MaxValue);

        public void InitTest(float value)
        {
            Test.SetBaseValue(value);
            Test.SetCurrentValue(value);
        }

        public void SetCurrentTest(float value)
        {
            Test.SetCurrentValue(value);
        }

        public void SetBaseTest(float value)
        {
            Test.SetBaseValue(value);
        }

        public void SetMinTest(float value)
        {
            Test.SetMinValue(value);
        }

        public void SetMaxTest(float value)
        {
            Test.SetMaxValue(value);
        }

        public void SetMinMaxTest(float min, float max)
        {
            Test.SetMinMaxValue(min, max);
        }

        #endregion Test

        public override AttributeBase this[string key]
        {
            get
            {
                switch (key)
                {
                    case "Test":
                        return Test;
                }

                return null;
            }
        }

        public override string[] AttributeNames { get; } =
        {
            "Test",
        };

        public override void SetOwner(AbilitySystemComponent owner)
        {
            _owner = owner;
            Test.SetOwner(owner);
        }

        public static class Lookup
        {
            public const string Test = "AS_Test.Test";
        }
    }

    public static class GAttrSetLib
    {
        public static readonly Dictionary<string, Type> AttrSetTypeDict = new Dictionary<string, Type>()
        {
            { "Test", typeof(AS_Test) },
        };

        public static readonly Dictionary<Type, string> TypeToName = new Dictionary<Type, string>
        {
            { typeof(AS_Test), nameof(AS_Test) },
        };

        public static List<string> AttributeFullNames = new List<string>()
        {
            "AS_Test.Test",
        };
    }
}