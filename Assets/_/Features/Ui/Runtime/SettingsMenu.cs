using UnityEngine;
using UnityEngine.Audio;

namespace UIFeature.Runtime
{
    public class SettingsMenu : MonoBehaviour
    {
        public void SetVolume(float volume)
        {
            _audioMixer.SetFloat("MasterVolume", volume);
        }

        [SerializeField] private AudioMixer _audioMixer;
    }
}
