#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos
{
    using OdinInspector;
    using Editor;
    using Utilities;
    using Sirenix.Utilities.Editor;
    using UnityEngine;

    public class SomeClass2
    {
        [HideLabel] [Title("Title", horizontalLine: false, bold: false)]
        public string Title = "Some Title";

        [TextArea(10, 20)] public string Description = "Some description.";
    }

    public class QuicklyInspectObjects
    {
        private SomeClass2 someObject = new();

        [Button(ButtonSizes.Large)]
        [Title("OdinEditorWindow.InspectObject examples", "Make sure to checkout QuicklyInspectObjects.cs")]
        private void InspectObject()
        {
            OdinEditorWindow.InspectObject(someObject);
        }

        [Button(ButtonSizes.Large)]
        [HorizontalGroup("row1")]
        private void InDropDownAutoHeight()
        {
            var btnRect = GUIHelper.GetCurrentLayoutRect();
            OdinEditorWindow.InspectObjectInDropDown(someObject, btnRect, btnRect.width);
        }

        [Button(ButtonSizes.Large)]
        [HorizontalGroup("row1")]
        private void InDropDown()
        {
            var btnRect = GUIHelper.GetCurrentLayoutRect();
            OdinEditorWindow.InspectObjectInDropDown(someObject, btnRect, new Vector2(btnRect.width, 100));
        }

        [Button(ButtonSizes.Large)]
        [HorizontalGroup("row2")]
        private void InCenter()
        {
            var window = OdinEditorWindow.InspectObject(someObject);
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(270, 200);
        }

        [Button(ButtonSizes.Large)]
        [HorizontalGroup("row2")]
        private void OtherStuffYouCanDo()
        {
            var window = OdinEditorWindow.InspectObject(someObject);

            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(270, 200);
            window.titleContent = new GUIContent("Custom title", EditorIcons.RulerRect.Active);
            window.OnClose += () => Debug.Log("Window Closed");
            window.OnBeginGUI += () => GUILayout.Label("-----------");
            window.OnEndGUI += () => GUILayout.Label("-----------");
        }
    }
}
#endif