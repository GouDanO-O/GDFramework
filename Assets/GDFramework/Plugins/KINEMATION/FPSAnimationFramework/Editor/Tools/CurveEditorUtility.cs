// Designed by KINEMATION, 2023

using UnityEditor;
using UnityEngine;

namespace KINEMATION.FPSAnimationFramework.Editor.Tools
{
    public class CurveEditorUtility
    {
        public static Vector3 GetVectorValue(AnimationClip clip, EditorCurveBinding[] bindings, float time)
        {
            var tX = AnimationUtility.GetEditorCurve(clip, bindings[0]).Evaluate(time);
            var tY = AnimationUtility.GetEditorCurve(clip, bindings[1]).Evaluate(time);
            var tZ = AnimationUtility.GetEditorCurve(clip, bindings[2]).Evaluate(time);

            return new Vector3(tX, tY, tZ);
        }

        public static Quaternion GetQuatValue(AnimationClip clip, EditorCurveBinding[] bindings, float time)
        {
            var tX = AnimationUtility.GetEditorCurve(clip, bindings[0]).Evaluate(time);
            var tY = AnimationUtility.GetEditorCurve(clip, bindings[1]).Evaluate(time);
            var tZ = AnimationUtility.GetEditorCurve(clip, bindings[2]).Evaluate(time);
            var tW = AnimationUtility.GetEditorCurve(clip, bindings[3]).Evaluate(time);

            return new Quaternion(tX, tY, tZ, tW);
        }
    }
}