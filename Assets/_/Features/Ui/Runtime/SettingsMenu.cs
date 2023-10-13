using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace UIFeature.Runtime
{
    public class SettingsMenu : MonoBehaviour
    {
        public static SettingsMenu Instance { get; private set; }

        [Header("Volume")] [Range(0, 1)] public float m_masterVolume = 1;
        [Header("Volume")] [Range(0, 1)] public float m_musicVolume = 1;
        [Header("Volume")] [Range(0, 1)] public float m_ambienceVolume = 1;
        [Header("Volume")] [Range(0, 1)] public float m_sfxVolume = 1;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            m_masterVolume = PlayerPrefs.GetFloat("masterVolume");
            m_musicVolume = PlayerPrefs.GetFloat("musicVolume");
            m_ambienceVolume = PlayerPrefs.GetFloat("ambienceVolume");
            m_sfxVolume = PlayerPrefs.GetFloat("sfxVolume");
            
            DontDestroyOnLoad(this);
            
            _masterBus = RuntimeManager.GetBus("bus:/");
            _musicBus = RuntimeManager.GetBus("bus:/Music");
            _sfxBus = RuntimeManager.GetBus("bus:/SFX");
            _ambienceBus = RuntimeManager.GetBus("bus:/Ambience");
        }

        private void Update()
        {
            _masterBus.setVolume(m_masterVolume);
            _musicBus.setVolume(m_musicVolume);
            _sfxBus.setVolume(m_sfxVolume);
            _ambienceBus.setVolume(m_ambienceVolume);
        }
        

        private Bus _masterBus;
        private Bus _musicBus;
        private Bus _sfxBus;
        private Bus _ambienceBus;
    }
}
