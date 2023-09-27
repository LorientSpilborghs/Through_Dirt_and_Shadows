using FMODUnity;
using UnityEngine;
using ZoneFeature.Runtime;

namespace SoundManagerFeature.Runtime
{
    public class EndGameCutSceneSounds : MonoBehaviour
    {
        private void Awake()
        {
            _zoneEndGame = GetComponent<ZoneEndGame>();
        }

        private void Start()
        {
            _zoneEndGame.m_onEnterZoneEndGame += OnValueChangedEventHandler;

        }

        private void OnDestroy()
        {
            _zoneEndGame.m_onEnterZoneEndGame -= OnValueChangedEventHandler;
        }

        private void OnValueChangedEventHandler()
        {
            RuntimeManager.PlayOneShot(_endGameCutSceneSound);
        }

        [SerializeField] private EventReference _endGameCutSceneSound;
        
        private ZoneEndGame _zoneEndGame;
    }
}
