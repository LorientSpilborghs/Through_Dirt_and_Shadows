using System;
using PlayerRuntime;
using RootFeature.Runtime;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ZoneFeature.Runtime
{
    public class ZonePurification : Zone
    {
        public Action m_onValueChange;
        
        public int KnotsNeedForPurification
        {
            get => _knotsNeedForPurification;
            set => _knotsNeedForPurification = value;
        }

        public int CurrentKnotInTheZone
        {
            get => _currentKnotInTheZone;
            set => _currentKnotInTheZone = value;
        }
        
        private void Start()
        {
            _sphereCollider = GetComponent<SphereCollider>();
        }

        protected override void OnEnterZone()
        {
            PlayerV2.Instance.m_onNewKnotInstantiate += Purifying;
        }

        protected override void OnExitZone()
        {
            PlayerV2.Instance.m_onNewKnotInstantiate -= Purifying;
        }
        
        private void Purifying()
        {
            if (_completed) return;

            CurrentKnotInTheZone++;
            m_onValueChange?.Invoke();

            if (CurrentKnotInTheZone < KnotsNeedForPurification) return;
            
            _completed = true;
            PlayerV2.Instance.m_onNewKnotInstantiate -= Purifying;
            if (_ivyPreset.Length == 0) return;
            
            foreach (var ivy in _ivyPreset)
            {
                var radius = _sphereCollider.radius;
                Instantiate(ivy._ivyPrefab, 
                    new Vector3(transform.position.x + Random.insideUnitSphere.x * radius, 0, transform.position.z + Random.insideUnitSphere.z * radius), 
                    Quaternion.identity, transform);
            }
        }

        private void OnDrawGizmos()
        {
            if (_completed)
            {
                Gizmos.color = Color.green;
            }
            Gizmos.DrawWireSphere(transform.position, 5);
        }

        [SerializeField] private int _knotsNeedForPurification;
        [SerializeField] private Ivy[] _ivyPreset;

        private SphereCollider _sphereCollider;
        private int _currentKnotInTheZone;
        private bool _canPurify;
        private bool _completed;
    }
}