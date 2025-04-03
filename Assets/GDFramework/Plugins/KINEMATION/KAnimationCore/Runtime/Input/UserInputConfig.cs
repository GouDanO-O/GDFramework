// Designed by KINEMATION, 2024.

using System.Collections.Generic;
using UnityEngine;

namespace KINEMATION.KAnimationCore.Runtime.Input
{
    [CreateAssetMenu(fileName = "NewInputConfig", menuName = "KINEMATION/Input Config")]
    public class UserInputConfig : ScriptableObject
    {
        public List<IntProperty> intProperties = new();
        public List<FloatProperty> floatProperties = new();
        public List<BoolProperty> boolProperties = new();
        public List<VectorProperty> vectorProperties = new();
    }
}