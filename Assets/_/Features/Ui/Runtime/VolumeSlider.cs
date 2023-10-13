using UnityEngine;
using UnityEngine.UI;

namespace UIFeature.Runtime
{
    public class VolumeSlider : MonoBehaviour
    {
        private enum VolumeType
        {
            MASTER,
            MUSIC,
            AMBIENCE,
            SFX
        }

        private void Awake()
        {
            _volumeSlider = GetComponent<Slider>();
        }

        private void Update()
        {
            switch (_volumeType)
            {
                case VolumeType.MASTER:
                    _volumeSlider.value = SettingsMenu.Instance.m_masterVolume;
                    break;
                case VolumeType.MUSIC:
                    _volumeSlider.value = SettingsMenu.Instance.m_musicVolume;
                    break;
                case VolumeType.AMBIENCE:
                    _volumeSlider.value = SettingsMenu.Instance.m_ambienceVolume;
                    break;
                case VolumeType.SFX:
                    _volumeSlider.value = SettingsMenu.Instance.m_sfxVolume;
                    break;
            }
        }

        public void OnSliderValueChange()
        {
            switch (_volumeType)
            {
                case VolumeType.MASTER:
                    SettingsMenu.Instance.m_masterVolume = _volumeSlider.value;
                    PlayerPrefs.SetFloat("masterVolume", _volumeSlider.value);
                    break;
                case VolumeType.MUSIC:
                    SettingsMenu.Instance.m_musicVolume = _volumeSlider.value;
                    PlayerPrefs.SetFloat("musicVolume", _volumeSlider.value);
                    break;
                case VolumeType.AMBIENCE:
                    SettingsMenu.Instance.m_ambienceVolume = _volumeSlider.value;
                    PlayerPrefs.SetFloat("ambienceVolume", _volumeSlider.value);
                    break;
                case VolumeType.SFX:
                    SettingsMenu.Instance.m_sfxVolume = _volumeSlider.value;
                    PlayerPrefs.SetFloat("sfxVolume", _volumeSlider.value);
                    break;
            }
        }

        [Header("Type")] [SerializeField] private VolumeType _volumeType;
        
        private Slider _volumeSlider;
    }
}