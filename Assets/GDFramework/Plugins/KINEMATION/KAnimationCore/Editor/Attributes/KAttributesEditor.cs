// Designed by KINEMATION, 2024

using UnityEditor;
using UnityEngine;

namespace KINEMATION.KAnimationCore.Editor.Attributes
{
    public class KAttributesEditor
    {
        public static T GetComponent<T>(SerializedProperty property) where T : class
        {
            var targetObject = property.serializedObject.targetObject;

            var targetComponent = targetObject as Component;
            if (targetComponent != null) return targetComponent.GetComponentInChildren<T>();

            return null;
        }
    }
}