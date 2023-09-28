using FMODUnity;
using UnityEngine;
using ZoneFeature.Runtime;

namespace SoundManagerFeature.Runtime
{
    public class ZonePurificationSounds : MonoBehaviour
    {
        private void Awake()
        {
            _zonePurification = GetComponent<ZonePurification>();
        }

        private void Start()
        {
            _zonePurification.m_onZoneFinished += OnValueChangedEventHandler;
        }

        private void OnDestroy()
        {
            _zonePurification.m_onZoneFinished -= OnValueChangedEventHandler;
        }

        private void OnValueChangedEventHandler()
        {
            RuntimeManager.PlayOneShot(_purificationSound);
        }

        [SerializeField] private EventReference _purificationSound;
        
        private ZonePurification _zonePurification;
    }
}
