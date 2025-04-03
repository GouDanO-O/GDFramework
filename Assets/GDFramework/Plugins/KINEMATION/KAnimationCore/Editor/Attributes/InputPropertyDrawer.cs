// Designed by KINEMATION, 2024.

using KINEMATION.KAnimationCore.Runtime.Attributes;
using KINEMATION.KAnimationCore.Runtime.Input;
using System.Collections.Generic;
using KINEMATION.KAnimationCore.Runtime.Rig;
using UnityEditor;
using UnityEngine;

namespace KINEMATION.KAnimationCore.Editor.Attributes
{
    [CustomPropertyDrawer(typeof(InputProperty))]
    public class InputPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var component = property.serializedObject.targetObject as MonoBehaviour;
            UserInputConfig config = null;

            if (component != null)
            {
                var root = component.transform.root;
                var controller = root.gameObject.GetComponentInChildren<UserInputController>();
                config = controller == null ? null : controller.inputConfig;
            }

            if (config == null)
                config = (property.serializedObject.targetObject as IRigUser)?.GetRigAsset().inputConfig;

            if (config == null)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            var selectedIndex = -1;
            var indexOffset = 0;

            var properties = new List<string>();
            var options = new List<string>();

            var count = config.boolProperties.Count;
            for (var i = 0; i < count; i++)
            {
                var inputProperty = config.boolProperties[i];
                properties.Add(inputProperty.name);
                options.Add($"{inputProperty.name} (bool)");
                if (property.stringValue.Equals(inputProperty.name)) selectedIndex = i + indexOffset;
            }

            indexOffset += count;

            count = config.intProperties.Count;
            for (var i = 0; i < count; i++)
            {
                var inputProperty = config.intProperties[i];
                properties.Add(inputProperty.name);
                options.Add($"{inputProperty.name} (int)");
                if (property.stringValue.Equals(inputProperty.name)) selectedIndex = i + indexOffset;
            }

            indexOffset += count;

            count = config.floatProperties.Count;
            for (var i = 0; i < count; i++)
            {
                var inputProperty = config.floatProperties[i];
                properties.Add(inputProperty.name);
                options.Add($"{inputProperty.name} (float)");
                if (property.stringValue.Equals(inputProperty.name)) selectedIndex = i + indexOffset;
            }

            indexOffset += count;

            count = config.vectorProperties.Count;
            for (var i = 0; i < count; i++)
            {
                var inputProperty = config.vectorProperties[i];
                properties.Add(inputProperty.name);
                options.Add($"{inputProperty.name} (Vector4)");
                if (property.stringValue.Equals(inputProperty.name)) selectedIndex = i + indexOffset;
            }

            selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex,
                options.ToArray());

            property.stringValue = selectedIndex == -1 ? "None" : properties[selectedIndex];
        }
    }
}