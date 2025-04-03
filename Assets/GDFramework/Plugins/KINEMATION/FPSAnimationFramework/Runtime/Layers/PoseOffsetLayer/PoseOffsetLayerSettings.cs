// Designed by KINEMATION, 2024.

using System;
using System.Collections.Generic;
using KINEMATION.FPSAnimationFramework.Runtime.Core;
using KINEMATION.KAnimationCore.Runtime.Attributes;
using KINEMATION.KAnimationCore.Runtime.Rig;

namespace KINEMATION.FPSAnimationFramework.Runtime.Layers.PoseOffsetLayer
{
    [Serializable]
    public struct PoseOffset
    {
        [Unfold] public KPose pose;
        [CurveSelector] public CurveBlend blend;
        public bool keepChildrenPose;
    }

    public class PoseOffsetLayerSettings : FPSAnimatorLayerSettings
    {
        public List<PoseOffset> poseOffsets = new();

        public override FPSAnimatorLayerState CreateState()
        {
            return new PoseOffsetLayerState();
        }

#if UNITY_EDITOR
        public override void OnRigUpdated()
        {
            var count = poseOffsets.Count;

            for (var i = 0; i < count; i++)
            {
                var item = poseOffsets[i];
                UpdateRigElement(ref item.pose.element);
                poseOffsets[i] = item;
            }
        }
#endif
    }
}