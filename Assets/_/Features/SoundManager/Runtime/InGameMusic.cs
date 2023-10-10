using FMOD.Studio;
using FMODUnity;
using GameManagerFeature.Runtime;
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
            GameManager.Instance.m_onGameOver += OnGameStopEventHandler;
            GameManager.Instance.m_onEndGame += OnGameStopEventHandler;
        }

        private void OnDestroy()
        {
            _inGameMusicInstance.stop(STOP_MODE.IMMEDIATE);
            GameManager.Instance.m_onGameOver -= OnGameStopEventHandler;
            GameManager.Instance.m_onEndGame -= OnGameStopEventHandler;
        }

        private void OnGameStopEventHandler()
        {
            _inGameMusicInstance.stop(STOP_MODE.ALLOWFADEOUT);
        }

        [SerializeField] private EventReference _inGameMusic;
        
        private EventInstance _inGameMusicInstance;
    }
}
