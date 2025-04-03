// Designed by KINEMATION, 2024.

using KINEMATION.FPSAnimationFramework.Runtime.Core;
using KINEMATION.KAnimationCore.Runtime.Core;
using KINEMATION.KAnimationCore.Runtime.Rig;
using UnityEngine;

namespace KINEMATION.FPSAnimationFramework.Runtime.Layers.CollisionLayer
{
    public class CollisionLayerState : FPSAnimatorLayerState
    {
        private CollisionLayerSettings _settings;
        private Transform _weaponIkBone;
        private KTransform _blockingPose = KTransform.Identity;

        private int _mouseInputPropertyIndex;

        public override void InitializeState(FPSAnimatorLayerSettings newSettings)
        {
            _settings = (CollisionLayerSettings)newSettings;
            _weaponIkBone = _rigComponent.GetRigTransform(_settings.weaponIkBone);
            _mouseInputPropertyIndex = _inputController.GetPropertyIndex(FPSANames.MouseInput);
        }

        public override void OnEvaluatePose()
        {
            var direction = _weaponIkBone.forward;
            var start = _weaponIkBone.position - direction * _settings.rayStartOffset;

            var bHit = Physics.Raycast(start, direction, out var hit, _settings.barrelLength,
                _settings.layerMask);

            var target = KTransform.Identity;
            if (bHit)
            {
                var blockRatio = 1f - hit.distance / (_settings.barrelLength + _settings.rayStartOffset);

                var mouseInput = _inputController.GetValue<Vector4>(_mouseInputPropertyIndex).y;
                target = KTransform.Lerp(target,
                    mouseInput > 0f ? _settings.secondaryPose : _settings.primaryPose, blockRatio);
            }

            var alpha = KMath.ExpDecayAlpha(_settings.smoothingSpeed, Time.deltaTime);
            _blockingPose = KTransform.Lerp(_blockingPose, target, alpha);

            var pose = new KPose()
            {
                modifyMode = EModifyMode.Add,
                pose = _blockingPose,
                space = _settings.targetSpace
            };

            KAnimationMath.ModifyTransform(_owner.transform, _weaponIkBone, pose, Weight);
        }

#if UNITY_EDITOR
        public override void OnDrawGizmos()
        {
            if (_weaponIkBone == null || _settings == null) return;

            var direction = _weaponIkBone.forward;
            var start = _weaponIkBone.position - direction * _settings.rayStartOffset;

            var color = Gizmos.color;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(start, start + direction * _settings.barrelLength);
            Gizmos.color = color;
        }
#endif
    }
}