#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos
{
    using UnityEditor;
    using UnityEngine;
    using System.Collections.Generic;
    using Editor;
    using Sirenix.Utilities.Editor;
    using OdinInspector;
    using Utilities;

    public class OverrideGetTargetsExampleWindow : OdinEditorWindow
    {
        [MenuItem("Tools/Odin/Demos/Odin Editor Window Demos/Draw Any Target")]
        private static void OpenWindow()
        {
            GetWindow<OverrideGetTargetsExampleWindow>()
                .position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
        }

        [HideLabel] [Multiline(6)] [SuffixLabel("This is drawn", true)]
        public string Test;

        // In the default implemenentation, it simply yield returns it self.
        // But you can also override this behaviour and have your window render any
        // object you like - Unity and non-Unity objects a like.
        protected override IEnumerable<object> GetTargets()
        {
            // Draws this instance using Odin
            yield return this;

            // Draw non-unity objects.
            yield return GUI.skin.settings; // GUISettings is a regular class.

            // Or Unity objects.
            yield return GUI.skin; // GUI.Skin is a ScriptableObject
        }

        // You can also override the method that draws each editor.
        // This come in handy if you want to add titles, boxes, or draw them in a GUI.Window etc...
        protected override void DrawEditor(int index)
        {
            var currentDrawingEditor = CurrentDrawingTargets[index];

            SirenixEditorGUI.Title(
                currentDrawingEditor.ToString(),
                currentDrawingEditor.GetType().GetNiceFullName(),
                TextAlignment.Left,
                true
            );

            base.DrawEditor(index);

            if (index != CurrentDrawingTargets.Count - 1) SirenixEditorGUI.DrawThickHorizontalSeparator(15, 15);
        }
    }
}
#endif