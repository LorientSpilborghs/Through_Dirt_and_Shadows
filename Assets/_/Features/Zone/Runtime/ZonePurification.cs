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
        public Action m_onZonePurified;
        
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
            PlayerV2.Instance.m_onCameraBlendingStart += CheckIfRootIsInZone;
            PlayerV2.Instance.m_onCameraBlendingStop += StopPurifying;
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
            if (_isPurified) return;

            CurrentKnotInTheZone++;
            m_onValueChange?.Invoke();

            if (CurrentKnotInTheZone < KnotsNeedForPurification) return;
            
            _isPurified = true;
            m_onZonePurified?.Invoke();
            GlobalPurification.Instance.m_onZonePurified?.Invoke(_globalPercentageOnPurified);
            PlayerV2.Instance.m_onNewKnotInstantiate -= Purifying;
            if (_ivyPreset.Length == 0) return;
            
            foreach (var ivy in _ivyPreset)
            {
                var radius = _sphereCollider.radius;
                Instantiate(ivy._ivyPrefab, 
                    new Vector3(transform.position.x + Random.insideUnitSphere.x * radius, 0, transform.position.z + Random.insideUnitSphere.z * radius), 
                    Quaternion.identity, transform);
            }

            foreach (var particleSystem in _purificationParticle)
            {
                particleSystem.gameObject.SetActive(false);
            }
            
            if (!_isOpeningADoor) return;

            _doorAnimation.Play();
        }

        private void StopPurifying()
        {
            PlayerV2.Instance.m_onNewKnotInstantiate -= Purifying;
        }

        private void CheckIfRootIsInZone(Vector3 pos)
        {
            if (Vector3.Distance(transform.position, pos) > _sphereCollider.radius) return;
            PlayerV2.Instance.m_onNewKnotInstantiate += Purifying;
        }

        private void OnDrawGizmos()
        {
            if (_isPurified)
            {
                Gizmos.color = Color.green;
            }
            Gizmos.DrawWireSphere(transform.position, 5);
        }

        [SerializeField] private int _knotsNeedForPurification;
        [SerializeField] private float _globalPercentageOnPurified;
        [SerializeField] private bool _isOpeningADoor;
        [SerializeField] private Animation _doorAnimation;
        [Space] [SerializeField] private Ivy[] _ivyPreset;
        [Space] [SerializeField] private ParticleSystem[] _purificationParticle;

        private SphereCollider _sphereCollider;
        private int _currentKnotInTheZone;
        private bool _canPurify;
        private bool _isPurified;
    }
}