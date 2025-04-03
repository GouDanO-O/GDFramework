// Designed by KINEMATION, 2024.

using KINEMATION.KAnimationCore.Editor.Misc;
using UnityEditor;
using UnityEngine;

namespace KINEMATION.FPSAnimationFramework.Editor.Tools
{
    public class IKAdditiveGenerator : IEditorTool
    {
        private Transform _root;
        private Transform _extractFrom;
        private Transform _extractTo;

        private AnimationClip _clip;
        private AnimationClip _refClip;

        private Vector3 _rotationOffset;
        private bool _isAdditive;

        private string GetBonePath(Transform targetBone, Transform root)
        {
            if (targetBone == null || root == null) return "";

            var path = targetBone.name;
            var current = targetBone.parent;

            while (current != null && current != root)
            {
                path = current.name + "/" + path;
                current = current.parent;
            }

            return current == root ? path : null;
        }

        private void ExtractAndSetAnimationData()
        {
            var bindings = AnimationUtility.GetCurveBindings(_clip);

            var tBindings = new EditorCurveBinding[3];
            var rBindings = new EditorCurveBinding[4];

            foreach (var binding in bindings)
            {
                if (!binding.path.EndsWith(_extractFrom.name)) continue;

                if (binding.propertyName.ToLower().Contains("m_localposition.x"))
                {
                    tBindings[0] = binding;
                    continue;
                }

                if (binding.propertyName.ToLower().Contains("m_localposition.y"))
                {
                    tBindings[1] = binding;
                    continue;
                }

                if (binding.propertyName.ToLower().Contains("m_localposition.z"))
                {
                    tBindings[2] = binding;
                    continue;
                }

                if (binding.propertyName.ToLower().Contains("m_localrotation.x"))
                {
                    rBindings[0] = binding;
                    continue;
                }

                if (binding.propertyName.ToLower().Contains("m_localrotation.y"))
                {
                    rBindings[1] = binding;
                    continue;
                }

                if (binding.propertyName.ToLower().Contains("m_localrotation.z"))
                {
                    rBindings[2] = binding;
                    continue;
                }

                if (binding.propertyName.ToLower().Contains("m_localrotation.w")) rBindings[3] = binding;
            }

            var refTranslation = Vector3.zero;
            var refRotation = Quaternion.identity;

            if (_refClip != null && _isAdditive)
            {
                refTranslation = CurveEditorUtility.GetVectorValue(_refClip, tBindings, 0f);
                refRotation =
                    CurveEditorUtility.GetQuatValue(_refClip, rBindings, 0f) * Quaternion.Euler(_rotationOffset);
            }

            var tX = new AnimationCurve();
            var tY = new AnimationCurve();
            var tZ = new AnimationCurve();

            var rX = new AnimationCurve();
            var rY = new AnimationCurve();
            var rZ = new AnimationCurve();
            var rW = new AnimationCurve();

            var playLength = _clip.length;
            var frameRate = 1f / _clip.frameRate;
            var playBack = 0f;

            while (playBack <= playLength)
            {
                var translation = CurveEditorUtility.GetVectorValue(_clip, tBindings, playBack);
                var rotation = CurveEditorUtility.GetQuatValue(_clip, rBindings, playBack) *
                               Quaternion.Euler(_rotationOffset);

                var deltaT = translation - refTranslation;
                var deltaR = Quaternion.Inverse(refRotation) * rotation;

                tX.AddKey(playBack, deltaT.x);
                tY.AddKey(playBack, deltaT.y);
                tZ.AddKey(playBack, deltaT.z);

                rX.AddKey(playBack, deltaR.x);
                rY.AddKey(playBack, deltaR.y);
                rZ.AddKey(playBack, deltaR.z);
                rW.AddKey(playBack, deltaR.w);

                playBack += frameRate;
            }

            var path = GetBonePath(_extractTo, _root);

            _clip.SetCurve(path, typeof(Transform), tBindings[0].propertyName, tX);
            _clip.SetCurve(path, typeof(Transform), tBindings[1].propertyName, tY);
            _clip.SetCurve(path, typeof(Transform), tBindings[2].propertyName, tZ);

            _clip.SetCurve(path, typeof(Transform), rBindings[0].propertyName, rX);
            _clip.SetCurve(path, typeof(Transform), rBindings[1].propertyName, rY);
            _clip.SetCurve(path, typeof(Transform), rBindings[2].propertyName, rZ);
            _clip.SetCurve(path, typeof(Transform), rBindings[3].propertyName, rW);
        }

        public void Render()
        {
            if (!EditorGUIUtility.wideMode) EditorGUIUtility.wideMode = true;

            EditorGUILayout.HelpBox("This tool will turn baked animation curves into a procedural additive animation."
                                    + "\nUseful for procedural animations, like idle, walk or sprint.",
                MessageType.Info);

            GUILayout.Label("Settings", EditorStyles.boldLabel);

            _clip =
                EditorGUILayout.ObjectField("Target Animation", _clip, typeof(AnimationClip), true)
                    as AnimationClip;

            _refClip =
                EditorGUILayout.ObjectField("Reference Animation", _refClip,
                    typeof(AnimationClip), true) as AnimationClip;

            _root = EditorGUILayout.ObjectField("Root", _root, typeof(Transform), true)
                as Transform;

            _extractFrom = EditorGUILayout.ObjectField("From", _extractFrom, typeof(Transform), true)
                as Transform;

            _extractTo = EditorGUILayout.ObjectField("To", _extractTo, typeof(Transform), true)
                as Transform;

            _rotationOffset = EditorGUILayout.Vector3Field("Rotation Offset", _rotationOffset);
            _isAdditive = EditorGUILayout.Toggle("Is Additive", _isAdditive);

            if (_clip == null)
            {
                EditorGUILayout.HelpBox("Please, specify the Target Animation!", MessageType.Warning);
                return;
            }

            if (_refClip == null)
            {
                EditorGUILayout.HelpBox("Please, specify the Reference Animation!", MessageType.Warning);
                return;
            }

            if (_root == null || _extractFrom == null || _extractTo == null)
            {
                EditorGUILayout.HelpBox("Please, specify the bones!", MessageType.Warning);
                return;
            }

            if (GUILayout.Button("Extract")) ExtractAndSetAnimationData();
        }
    }
}