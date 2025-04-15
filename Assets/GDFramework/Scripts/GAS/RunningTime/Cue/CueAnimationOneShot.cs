using GDFramework.GAS.General;
using GDFramework.GAS.General.Util;
using GDFramework.GAS.RunningTime.Cue.Base;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GDFramework.GAS.RunningTime.Cue
{
    [CreateAssetMenu(fileName = "CuePlayAnimation", menuName = "GAS/Cue/CuePlayAnimation")]
    public class CueAnimationOneShot : GameplayCueInstant
    {
        [BoxGroup]
        [InfoBox(GasTextDefine.CUE_ANIMATION_PATH_TIP)]
        [LabelText(GasTextDefine.CUE_ANIMATION_PATH)]
        [SerializeField]
        private string _animatorRelativePath;

        [BoxGroup]
        [InfoBox(GasTextDefine.CUE_ANIMATION_INCLUDE_CHILDREN_ANIMATOR_TIP)]
        [LabelText(GasTextDefine.CUE_ANIMATION_INCLUDE_CHILDREN)]
        [SerializeField]
        private bool _includeChildrenAnimator;

        [BoxGroup] [LabelText(GasTextDefine.CUE_ANIMATION_STATE)] [SerializeField]
        private string _stateName;

        public string AnimatorRelativePath => _animatorRelativePath;
        public bool IncludeChildrenAnimator => _includeChildrenAnimator;
        public string StateName => _stateName;


        public override GameplayCueInstantSpec CreateSpec(GameplayCueParameters parameters)
        {
            return new CueAnimationOneShotSpec(this, parameters);
        }

#if UNITY_EDITOR
        public override void OnEditorPreview(GameObject previewObject, int frame, int startFrame)
        {
            if (startFrame <= frame)
            {
                var transform = previewObject.transform.Find(AnimatorRelativePath);
                Animator animator = null;
                if (transform != null)
                    animator = IncludeChildrenAnimator
                        ? transform.GetComponentInChildren<Animator>()
                        : transform.GetComponent<Animator>();

                if (animator == null)
                {
                    Debug.LogError(
                        $"Animator is null. Please check the cue asset: {name}, AnimatorRelativePath: {AnimatorRelativePath}, IncludeChildrenAnimator: {IncludeChildrenAnimator}");
                    return;
                }

                var stateMap = animator.GetAllAnimationState();
                if (stateMap.TryGetValue(StateName, out var clip))
                {
                    if (clip != null)
                    {
                        float clipFrameCount = (int)(clip.frameRate * clip.length);
                        if (frame <= clipFrameCount + startFrame)
                        {
                            var progress = (frame - startFrame) / clipFrameCount;
                            if (progress > 1 && clip.isLooping) progress -= (int)progress;
                            clip.SampleAnimation(animator.gameObject, progress * clip.length);
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Clip is null. Please check the cue asset: {name}, StateName: {StateName}");
                    }
                }
            }
        }
#endif
    }

    public class CueAnimationOneShotSpec : GameplayCueInstantSpec<CueAnimationOneShot>
    {
        private readonly Animator _animator;

        public CueAnimationOneShotSpec(CueAnimationOneShot cue, GameplayCueParameters parameters)
            : base(cue, parameters)
        {
            var transform = Owner.transform.Find(cue.AnimatorRelativePath);
            if (transform != null)
                _animator = cue.IncludeChildrenAnimator
                    ? transform.GetComponentInChildren<Animator>()
                    : transform.GetComponent<Animator>();

            if (_animator == null)
                Debug.LogError(
                    $"Animator is null. Please check the cue asset: {cue.name}, AnimatorRelativePath: {cue.AnimatorRelativePath}, IncludeChildrenAnimator: {cue.IncludeChildrenAnimator}");
        }

        public override void Trigger()
        {
            if (_animator != null) _animator.Play(cue.StateName);
        }
    }
}