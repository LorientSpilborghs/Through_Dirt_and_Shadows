using System.Collections;
using FMODUnity;
using UnityEngine;
using ZoneFeature.Runtime;

namespace SoundManagerFeature.Runtime
{
    public class ZoneHarvestSounds : MonoBehaviour
    {
        private void Awake()
        {
            _zoneHarvest = GetComponent<ZoneHarvestV2>();
        }

        private void Start()
        {
            _zoneHarvest.m_onValueChange += OnValueChangedEventHandler;
        }

        private void OnDestroy()
        {
            _zoneHarvest.m_onValueChange -= OnValueChangedEventHandler;
        }

        private void OnValueChangedEventHandler()
        {
            StartCoroutine(DelayBeforeSound());
            RuntimeManager.PlayOneShotAttached(_harvestSound, gameObject);
        }

        private IEnumerator DelayBeforeSound()
        {
            yield return new WaitForSeconds(_delayBeforeSound);
        }

        [SerializeField] private EventReference _harvestSound;
        [SerializeField] private float _delayBeforeSound; 
        
        private ZoneHarvestV2 _zoneHarvest;
    }
}
