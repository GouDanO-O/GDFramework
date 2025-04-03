using GDFramework_Core.GAS.General;
using GDFramework_Core.GAS.RunningTime.Cue.Base;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GDFramework_Core.GAS.RunningTime.Cue
{
    public class CuePlaySound : GameplayCueDurational
    {
        [BoxGroup]
        [LabelText(GasTextDefine.CUE_SOUND_EFFECT)]
        public AudioClip soundEffect; 
        
        [BoxGroup]
        [LabelText(GasTextDefine.CUE_ATTACH_TO_OWNER)]
        public bool isAttachToOwner = true;
        
        public override GameplayCueDurationalSpec CreateSpec(GameplayCueParameters parameters)
        {
            return new CuePlaySoundSpec(this, parameters);
        }
    }
    
    public class CuePlaySoundSpec : GameplayCueDurationalSpec<CuePlaySound>
    {
        private AudioSource _audioSource;
        
        public CuePlaySoundSpec(CuePlaySound cue, GameplayCueParameters parameters) : base(cue,
            parameters)
        {
            if (cue.isAttachToOwner)
            {
                _audioSource = Owner.gameObject.GetComponent<AudioSource>();
                if (_audioSource == null)
                {
                    _audioSource = Owner.gameObject.AddComponent<AudioSource>();
                }
            }
            else
            {
                var soundRoot = new GameObject("SoundRoot");
                soundRoot.transform.position = Owner.transform.position;
                _audioSource = soundRoot.AddComponent<AudioSource>();
            }
        }

        public override void OnAdd()
        {
            _audioSource.clip = cue.soundEffect;
            _audioSource.Play();
        }

        public override void OnRemove()
        {
            if (!cue.isAttachToOwner)
            {
                Object.Destroy(_audioSource.gameObject);
            }else
            {
                _audioSource.Stop();
                _audioSource.clip = null;
            }
        }

        public override void OnGameplayEffectActivate()
        {
        }

        public override void OnGameplayEffectDeactivate()
        {
        }

        public override void OnTick()
        {
        }
    }
}