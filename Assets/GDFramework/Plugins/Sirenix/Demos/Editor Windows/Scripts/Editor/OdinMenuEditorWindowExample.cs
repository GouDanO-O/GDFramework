using System;

#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos
{
    using Editor;
    using System.Linq;
    using UnityEngine;
    using Sirenix.Utilities.Editor;
    using Serialization;
    using UnityEditor;
    using Utilities;

    // 
    // Be sure to check out OdinMenuStyleExample.cs as well. It shows you various ways to customize the look and behaviour of OdinMenuTrees.
    // 

    public class OdinMenuEditorWindowExample : OdinMenuEditorWindow
    {
        [MenuItem("Tools/Odin/Demos/Odin Editor Window Demos/Odin Menu Editor Window Example")]
        private static void OpenWindow()
        {
            var window = GetWindow<OdinMenuEditorWindowExample>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
        }

        [SerializeField]
        private SomeData
            someData = new(); // Take a look at SomeData.cs to see how serialization works in Editor Windows.

        private void Update()
        {
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(true)
            {
                { "Home", this, EditorIcons.House }, // Draws the this.someData field in this case.
                { "Odin Settings", null, SdfIconType.GearFill },
                { "Odin Settings/Color Palettes", ColorPaletteManager.Instance, SdfIconType.PaletteFill },
                { "Odin Settings/AOT Generation", AOTGenerationConfig.Instance, EditorIcons.SmartPhone },
                { "Player Settings", Resources.FindObjectsOfTypeAll<PlayerSettings>().FirstOrDefault() },
                { "Some Class", someData }
            };

            tree.AddAllAssetsAtPath("Odin Settings/More Odin Settings", "Plugins/Sirenix", typeof(ScriptableObject),
                    true)
                .AddThumbnailIcons();

            tree.AddAssetAtPath("Odin Getting Started", "Plugins/Sirenix/Getting Started With Odin.asset");

            tree.MenuItems.Insert(2, new OdinMenuItem(tree, "Menu Style", tree.DefaultMenuStyle));

            tree.Add("Menu/Items/Are/Created/As/Needed", new GUIContent());
            tree.Add("Menu/Items/Are/Created", new GUIContent("And can be overridden"));

            tree.SortMenuItemsByName();

            // As you can see, Odin provides a few ways to quickly add editors / objects to your menu tree.
            // The API also gives you full control over the selection, etc..
            // Make sure to check out the API Documentation for OdinMenuEditorWindow, OdinMenuTree and OdinMenuItem for more information on what you can do!

            return tree;
        }
    }
}
#endif