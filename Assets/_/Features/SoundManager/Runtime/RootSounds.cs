using System.Collections;
using FMOD.Studio;
using FMODUnity;
using PostProcessManagerFeature.Runtime;
using RootFeature.Runtime;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SoundManagerFeature.Runtime
{
    public class RootSounds : MonoBehaviour
    {
        private void Start()
        {
            _growingRootSoundInstance = RuntimeManager.CreateInstance(_growingRootSound);
            PostProcessManager.Instance.m_onLowHealth += OnLowHealthEventHandler;
            PostProcessManager.Instance.m_onOverHealth += OnOverHealthEventHandler;
            _root = GetComponent<RootV2>();
            _root.m_onStartGrow = OnStartGrowEventHandler;
            _root.m_onEndGrow = OnEndGrowEventHandler;
        }
        
        private void OnStartGrowEventHandler()
        {
            _growingRootSoundInstance.start();
            if (!_canPlaySound) return;
            _canPlaySound = false;
            RuntimeManager.PlayOneShot(_startGrowSound);
            StartCoroutine(SoundDelay());
        }

        private void OnEndGrowEventHandler()
        {
            _growingRootSoundInstance.stop(STOP_MODE.IMMEDIATE);
        }

        private void OnOverHealthEventHandler()
        {
            RuntimeManager.PlayOneShot(_overHealthSound);
        }
        
        private void OnLowHealthEventHandler()
        {
            RuntimeManager.PlayOneShot(_lowHealthSound);
        }
        
        private IEnumerator SoundDelay()
        {
            yield return new WaitForSeconds(_delayBeforeSoundOnStartGrow);
            _canPlaySound = true;
        }
        
        
        [SerializeField] private EventReference _growingRootSound;
        [SerializeField] private EventReference _startGrowSound;
        [SerializeField] private EventReference _overHealthSound;
        [SerializeField] private EventReference _lowHealthSound;
        [Space] [SerializeField] private float _delayBeforeSoundOnStartGrow = 2;
    
        private EventInstance _growingRootSoundInstance;
        private RootV2 _root;
        private bool _canPlaySound = true;
    }
}
