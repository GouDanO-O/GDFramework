// Designed by KINEMATION, 2024.

using KINEMATION.FPSAnimationFramework.Runtime.Core;
using System.Collections.Generic;
using UnityEngine;

namespace KINEMATION.FPSAnimationFramework.Runtime.Layers.MirrorLayer
{
    public class MirrorLayerState : FPSAnimatorLayerState
    {
        private MirrorLayerSettings _settings;
        private List<Transform> _bonesToMirror;

        public override void InitializeState(FPSAnimatorLayerSettings newSettings)
        {
            _settings = (MirrorLayerSettings)newSettings;
            _bonesToMirror = new List<Transform>();

            foreach (var element in _settings.bonesToMirror) _bonesToMirror.Add(_rigComponent.GetRigTransform(element));
        }

        public override void OnEvaluatePose()
        {
            var worldAxis = _owner.transform.TransformDirection(_settings.mirrorAxis.normalized);
            var originRotation = _owner.transform.rotation; //Quaternion.LookRotation(worldAxis, _owner.transform.up);
            var originPosition = _owner.transform.position;

            foreach (var bone in _bonesToMirror)
            {
                var targetDirection = bone.position - originPosition;
                targetDirection = Vector3.Reflect(targetDirection, -worldAxis);
                bone.position = originPosition + targetDirection;

                var targetRotation = Quaternion.Inverse(Quaternion.Inverse(originRotation) * bone.rotation);
                targetRotation = originRotation * targetRotation;
                bone.rotation = targetRotation;
            }
        }
    }
}