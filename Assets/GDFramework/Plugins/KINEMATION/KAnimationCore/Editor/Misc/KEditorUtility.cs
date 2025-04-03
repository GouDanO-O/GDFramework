// Designed by KINEMATION, 2024.

using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using Object = UnityEngine.Object;

namespace KINEMATION.KAnimationCore.Editor.Misc
{
    public class KEditorUtility
    {
        public static string GetProjectWindowFolder()
        {
            var projectWindowUtilType = typeof(ProjectWindowUtil);

            var getActiveFolderPathMethod = projectWindowUtilType.GetMethod("GetActiveFolderPath",
                BindingFlags.Static | BindingFlags.NonPublic);

            if (getActiveFolderPathMethod != null)
            {
                var result = getActiveFolderPathMethod.Invoke(null, null);
                if (result != null) return result.ToString();
            }

            return "No folder is currently opened.";
        }

        public static void SaveAsset(Object asset, string directory, string nameWithExtension)
        {
            var filePath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(directory, nameWithExtension));

            AssetDatabase.CreateAsset(asset, filePath);
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
        }
    }
}