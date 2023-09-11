using PlayerRuntime;
using UnityEngine;

namespace ZoneFeature.Runtime
{
    public class ZonePurification : Zone
    {
        private void Start()
        {
            PlayerV2.Instance.m_onInterpolate += Purifying;
        }

        private void OnDestroy()
        {
            PlayerV2.Instance.m_onInterpolate -= Purifying;
        }

        protected override void OnEnterZone()
        {
            _canPurify = true;
        }

        protected override void OnExitZone()
        {
            _canPurify = false;
        }

        private void Purifying(Vector3 pos)
        {
            if (!_canPurify || _completed) return;

            _currentTime += Time.deltaTime;
            if (!(_currentTime >= _timeBetweenPurification)) return;
            
            _gauge += _gaugePercentageOverTime;
            if (_gauge >= 1)
            {
                _completed = true;
                PlayerV2.Instance.m_onInterpolate -= Purifying;
            }
            _currentTime = 0;
        }

        private void OnDrawGizmos()
        {
            if (_completed)
            {
                Gizmos.color = Color.green;
            }
            Gizmos.DrawWireSphere(transform.position, 5);
        }

        [SerializeField] [Range(0,1)] private float _gaugePercentageOverTime;
        [SerializeField] private float _timeBetweenPurification;
        
        private float _gauge;
        private bool _canPurify;
        private bool _completed;
        
        private float _currentTime;
    }
}