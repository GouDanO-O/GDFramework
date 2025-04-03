// Designed by KINEMATION, 2024.

using KINEMATION.KAnimationCore.Runtime.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KINEMATION.KAnimationCore.Runtime.Rig
{
    [Serializable]
    public class KRigElementChain
    {
        public string chainName;
        [HideInInspector] public List<KRigElement> elementChain;
        [HideInInspector] public bool isStandalone;
    }

    // A simplified version of the KRigElementChain, which contains transforms only.
    public class KTransformChain
    {
        public List<Transform> transformChain = new();
        public List<KTransform> cachedTransforms = new();
        public ESpaceType spaceType;

        public void CacheTransforms(ESpaceType targetSpace, Transform root = null)
        {
            cachedTransforms.Clear();
            spaceType = targetSpace;

            foreach (var element in transformChain)
            {
                var cache = new KTransform();

                if (targetSpace == ESpaceType.WorldSpace)
                {
                    cache.position = element.position;
                    cache.rotation = element.rotation;
                }
                else if (targetSpace == ESpaceType.ComponentSpace)
                {
                    if (root == null) root = element.root;

                    cache.position = root.InverseTransformPoint(element.position);
                    cache.rotation = Quaternion.Inverse(root.rotation) * element.rotation;
                }
                else
                {
                    cache.position = element.localPosition;
                    cache.rotation = element.localRotation;
                }

                cachedTransforms.Add(cache);
            }
        }

        public void BlendTransforms(float weight)
        {
            var count = transformChain.Count;

            for (var i = 0; i < count; i++)
            {
                var element = transformChain[i];
                var cache = cachedTransforms[i];

                var pose = new KPose()
                {
                    modifyMode = EModifyMode.Replace,
                    pose = cache,
                    space = spaceType
                };

                KAnimationMath.ModifyTransform(element.root, element, pose, weight);
            }
        }

        public float GetLength(Transform root = null)
        {
            var chainLength = 0f;
            var count = transformChain.Count;

            if (count > 0 && root == null) root = transformChain[0];

            for (var i = 0; i < count; i++)
            {
                var targetBone = transformChain[i];

                if (count == 1)
                {
                    var targetMS = root.InverseTransformPoint(targetBone.position);
                    chainLength = targetMS.magnitude;
                }

                if (i > 0) chainLength += (targetBone.position - transformChain[i - 1].position).magnitude;
            }

            return chainLength;
        }

        public bool IsValid()
        {
            if (transformChain == null || cachedTransforms == null) return false;
            if (transformChain.Count == 0) return false;

            return true;
        }
    }
}