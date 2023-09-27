using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SoundManagerFeature.Runtime
{
    public class InGameMusic : MonoBehaviour
    {
        private void Start()
        {
            _inGameMusicInstance = RuntimeManager.CreateInstance(_inGameMusic);
            _inGameMusicInstance.start();
        }

        private void OnDestroy()
        {
            _inGameMusicInstance.stop(STOP_MODE.IMMEDIATE);
        }

        [SerializeField] private EventReference _inGameMusic;
        
        private EventInstance _inGameMusicInstance;
    }
}
