using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SoundManagerFeature.Runtime
{
    public class MenuMusic : MonoBehaviour
    {
        private void Start()
        {
            _menuMusicInstance = RuntimeManager.CreateInstance(_musicTest);
            _menuMusicInstance.start();
        }
        
        private void OnDestroy()
        {
            _menuMusicInstance.stop(STOP_MODE.IMMEDIATE);
        }
        
        
        [SerializeField] private EventReference _musicTest;
        private EventInstance _menuMusicInstance;
    }
}
