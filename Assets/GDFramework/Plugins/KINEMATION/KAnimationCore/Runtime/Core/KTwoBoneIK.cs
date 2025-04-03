// Designed by KINEMATION, 2023

using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace KINEMATION.KAnimationCore.Runtime.Core
{
    public struct KTwoBoneIkData
    {
        public KTransform root;
        public KTransform mid;
        public KTransform tip;
        public KTransform target;
        public KTransform hint;

        public float posWeight;
        public float rotWeight;
        public float hintWeight;

        public bool hasValidHint;
    }

    public class KTwoBoneIK
    {
        public static void Solve(ref KTwoBoneIkData ikData)
        {
            var aPosition = ikData.root.position;
            var bPosition = ikData.mid.position;
            var cPosition = ikData.tip.position;

            var tPosition = Vector3.Lerp(cPosition, ikData.target.position, ikData.posWeight);
            var tRotation = Quaternion.Lerp(ikData.tip.rotation, ikData.target.rotation, ikData.rotWeight);
            var hasHint = ikData.hasValidHint && ikData.hintWeight > 0f;

            var ab = bPosition - aPosition;
            var bc = cPosition - bPosition;
            var ac = cPosition - aPosition;
            var at = tPosition - aPosition;

            var abLen = ab.magnitude;
            var bcLen = bc.magnitude;
            var acLen = ac.magnitude;
            var atLen = at.magnitude;

            var oldAbcAngle = KMath.TriangleAngle(acLen, abLen, bcLen);
            var newAbcAngle = KMath.TriangleAngle(atLen, abLen, bcLen);

            // Bend normal strategy is to take whatever has been provided in the animation
            // stream to minimize configuration changes, however if this is collinear
            // try computing a bend normal given the desired target position.
            // If this also fails, try resolving axis using hint if provided.
            var axis = Vector3.Cross(ab, bc);
            if (axis.sqrMagnitude < KMath.SqrEpsilon)
            {
                axis = hasHint ? Vector3.Cross(ikData.hint.position - aPosition, bc) : Vector3.zero;

                if (axis.sqrMagnitude < KMath.SqrEpsilon)
                    axis = Vector3.Cross(at, bc);

                if (axis.sqrMagnitude < KMath.SqrEpsilon)
                    axis = Vector3.up;
            }

            axis = Vector3.Normalize(axis);

            var a = 0.5f * (oldAbcAngle - newAbcAngle);
            var sin = Mathf.Sin(a);
            var cos = Mathf.Cos(a);
            var deltaR = new Quaternion(axis.x * sin, axis.y * sin, axis.z * sin, cos);

            var localTip = ikData.mid.GetRelativeTransform(ikData.tip, false);
            ikData.mid.rotation = deltaR * ikData.mid.rotation;

            // Update child transform.
            ikData.tip = ikData.mid.GetWorldTransform(localTip, false);

            cPosition = ikData.tip.position;
            ac = cPosition - aPosition;

            var localMid = ikData.root.GetRelativeTransform(ikData.mid, false);
            localTip = ikData.mid.GetRelativeTransform(ikData.tip, false);
            ikData.root.rotation = KMath.FromToRotation(ac, at) * ikData.root.rotation;

            // Update child transforms.
            ikData.mid = ikData.root.GetWorldTransform(localMid, false);
            ikData.tip = ikData.mid.GetWorldTransform(localTip, false);

            if (hasHint)
            {
                var acSqrMag = ac.sqrMagnitude;
                if (acSqrMag > 0f)
                {
                    bPosition = ikData.mid.position;
                    cPosition = ikData.tip.position;
                    ab = bPosition - aPosition;
                    ac = cPosition - aPosition;

                    var acNorm = ac / Mathf.Sqrt(acSqrMag);
                    var ah = ikData.hint.position - aPosition;
                    var abProj = ab - acNorm * Vector3.Dot(ab, acNorm);
                    var ahProj = ah - acNorm * Vector3.Dot(ah, acNorm);

                    var maxReach = abLen + bcLen;
                    if (abProj.sqrMagnitude > maxReach * maxReach * 0.001f && ahProj.sqrMagnitude > 0f)
                    {
                        var hintR = KMath.FromToRotation(abProj, ahProj);
                        hintR.x *= ikData.hintWeight;
                        hintR.y *= ikData.hintWeight;
                        hintR.z *= ikData.hintWeight;
                        hintR = KMath.NormalizeSafe(hintR);
                        ikData.root.rotation = hintR * ikData.root.rotation;

                        ikData.mid = ikData.root.GetWorldTransform(localMid, false);
                        ikData.tip = ikData.mid.GetWorldTransform(localTip, false);
                    }
                }
            }

            ikData.tip.rotation = tRotation;
        }
    }

    public struct KTwoBoneIKJob : IJobParallelFor
    {
        public NativeArray<KTwoBoneIkData> twoBoneIkJobData;

        public void Execute(int index)
        {
            var twoBoneIkData = twoBoneIkJobData[index];
            KTwoBoneIK.Solve(ref twoBoneIkData);
            twoBoneIkJobData[index] = twoBoneIkData;
        }
    }
}