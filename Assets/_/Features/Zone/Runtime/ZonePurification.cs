using System;
using PlayerRuntime;
using RootFeature.Runtime;
using UnityEngine;
using UnityEngine.Playables;
using Random = UnityEngine.Random;

namespace ZoneFeature.Runtime
{
    public class ZonePurification : Zone
    {
        public Action m_onValueChange;
        public Action m_onZoneFinished;
        
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
            _player = PlayerV2.Instance;
            _player.m_onCameraBlendingStart += CheckIfRootIsInZone;
            _player.m_onCameraBlendingStop += StopPurifying;
        }

        protected override void OnEnterZone()
        {
            _player.m_onNewKnotInstantiate += Purifying;
        }

        protected override void OnExitZone()
        {
            _player.m_onNewKnotInstantiate -= Purifying;
        }
        
        private void Purifying()
        {
            if (_isPurified) return;

            CurrentKnotInTheZone++;
            m_onValueChange?.Invoke();

            if (CurrentKnotInTheZone < KnotsNeedForPurification) return;
            
            _isPurified = true;
            _player.m_onNewKnotInstantiate -= Purifying;
            m_onZoneFinished?.Invoke();
            GlobalPurification.Instance.m_onZonePurified?.Invoke(_globalPercentageOnPurified);
            
            if (_ivyPreset.Length != 0)
            {
                foreach (var ivy in _ivyPreset)
                {
                    var radius = _sphereCollider.radius;
                    Instantiate(ivy._ivyPrefab, 
                        new Vector3(transform.position.x + Random.insideUnitSphere.x * radius, 0, transform.position.z + Random.insideUnitSphere.z * radius), 
                        Quaternion.identity, transform);
                }
            }
            if (_purificationParticle.Length != 0)
            {
                foreach (var particleSystem in _purificationParticle)
                {
                    particleSystem.gameObject.SetActive(false);
                }
            }
            if (_doorAnimation == null || _playableDirector == null) return;

            _doorAnimation.Play("OpenDoor");
            _playableDirector.Play();
            _fogRevealer.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
        }

        private void StopPurifying()
        {
            _player.m_onNewKnotInstantiate -= Purifying;
        }

        private void CheckIfRootIsInZone(Vector3 pos)
        {
            if (Vector3.Distance(transform.position, pos) > _sphereCollider.radius) return;
            _player.m_onNewKnotInstantiate += Purifying;
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
        [Space] [Header("Manage Door Animation")]
        [SerializeField] private Animator _doorAnimation;
        [SerializeField] private PlayableDirector _playableDirector;
        [SerializeField] private GameObject _fogRevealer;
        [Space] [SerializeField] private Ivy[] _ivyPreset;
        [Space] [SerializeField] private ParticleSystem[] _purificationParticle;

        private SphereCollider _sphereCollider;
        private PlayerV2 _player;
        private int _currentKnotInTheZone;
        private bool _canPurify;
        private bool _isPurified;
    }
}