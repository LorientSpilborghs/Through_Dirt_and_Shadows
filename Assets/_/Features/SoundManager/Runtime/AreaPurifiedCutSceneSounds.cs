using FMODUnity;
using UnityEngine;
using ZoneFeature.Runtime;

namespace SoundManagerFeature.Runtime
{
    public class AreaPurifiedCutSceneSounds : MonoBehaviour
    {
        private void Start()
        {
            GlobalPurification.Instance.m_onAreaPurified += OnAreaPurifiedEventHandler;

        }

        private void OnDestroy()
        {
            GlobalPurification.Instance.m_onAreaPurified -= OnAreaPurifiedEventHandler;
        }

        private void OnAreaPurifiedEventHandler()
        {
            RuntimeManager.PlayOneShot(_areaPurifiedSound);
        }

        [SerializeField] private EventReference _areaPurifiedSound;
    }
}
