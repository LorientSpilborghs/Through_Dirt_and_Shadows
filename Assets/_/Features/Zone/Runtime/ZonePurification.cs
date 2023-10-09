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
            _instantiatedPrefab = Instantiate(_zonePurificationIndicator, transform.position, Quaternion.identity, transform);
            var localScale = _instantiatedPrefab.transform.localScale;
            localScale = new Vector3(_sphereCollider.radius,
                0, _sphereCollider.radius);
            _instantiatedPrefab.transform.localScale = localScale * 2;
            var position = _instantiatedPrefab.transform.position;
            _instantiatedPrefab.transform.SetPositionAndRotation(new Vector3(position.x, _purificationIndicatorHeight, position.z), Quaternion.identity);
            _instantiatedPrefab.gameObject.SetActive(false);
        }

        protected override void OnEnterZone()
        {
            if (_isPurified) return;
            _player.m_onNewKnotInstantiate += Purifying;
            if (_isZoneEnteredOnce) return;
            _isZoneEnteredOnce = true;
            _instantiatedPrefab.gameObject.SetActive(true);
            // StartCoroutine(WaitForBlinkToEnd());
        }

        protected override void OnExitZone()
        {
            if (_isPurified) return;
            _player.m_onNewKnotInstantiate -= Purifying;
        }
        
        private void Purifying()
        {
            if (_isPurified) return;

            CurrentKnotInTheZone++;
            m_onValueChange?.Invoke();

            if (CurrentKnotInTheZone < KnotsNeedForPurification) return;
            
            _isPurified = true;
            // StartCoroutine(WaitForBlinkToEnd());
            _instantiatedPrefab.gameObject.SetActive(false);
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
            if (_particleBeforePurification.Length != 0)
            {
                foreach (var particleSystem in _particleBeforePurification)
                {
                    particleSystem.gameObject.SetActive(false);
                }
            }

            if (_particleAfterPurification.Length != 0)
            {
                foreach (var particleSystem in _particleAfterPurification)
                {
                    Instantiate(particleSystem, transform.position, Quaternion.identity, transform);
                }
            }
            
            if (_doorAnimation == null || _playableDirector == null) return;

            _doorAnimation.Play("OpenDoor");
            _playableDirector.Play();
            _fogRevealer.transform.SetPositionAndRotation(_doorAnimation.gameObject.gameObject.transform.position, Quaternion.identity);
            if (_screenLightRenderer == null) return;
            _screenLightRenderer.materials[0].renderQueue = 0;
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

        // private IEnumerator WaitForBlinkToEnd()
        // {
        //     for (float timer = 0; timer < _blinkingDuration; timer += Time.deltaTime)
        //     {
        //         yield return new WaitForSeconds(_blinkingDuration);
        //         _instantiatedPrefab.SetActive(true);
        //         yield return new WaitForSeconds(_timeBetweenBlink);
        //         _instantiatedPrefab.SetActive(false);
        //     }
        //
        //     yield return null;
        // }

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
        [SerializeField] private float _purificationIndicatorHeight = 0.1f;
        // [SerializeField] private float _blinkingDuration = 1;
        // [SerializeField] private float _timeBetweenBlink = 0.5f;
        [Space] [Header("Manage Door Animation")]
        [SerializeField] private Animator _doorAnimation;
        [SerializeField] private PlayableDirector _playableDirector;
        [SerializeField] private GameObject _fogRevealer;
        [SerializeField] private MeshRenderer _screenLightRenderer;
        [SerializeField] private GameObject _zonePurificationIndicator;
        [Space] [SerializeField] private Ivy[] _ivyPreset;
        [Space] [SerializeField] private ParticleSystem[] _particleBeforePurification;
        [Space] [SerializeField] private ParticleSystem[] _particleAfterPurification;

        private SphereCollider _sphereCollider;
        private PlayerV2 _player;
        private GameObject _instantiatedPrefab;
        private int _currentKnotInTheZone;
        private bool _canPurify;
        private bool _isPurified;
        private bool _isZoneEnteredOnce;
    }
}