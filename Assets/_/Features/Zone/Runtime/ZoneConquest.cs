using PlayerRuntime;
using UnityEngine;

namespace ZoneFeature.Runtime
{
    public class ZoneConquest : Zone
    {
        private void Start()
        {
            PlayerV2.Instance.m_onInterpolate += Conquering;
        }

        private void OnDestroy()
        {
            PlayerV2.Instance.m_onInterpolate -= Conquering;
        }

        protected override void OnEnterZone()
        {
            _canConquer = true;
        }

        protected override void OnExitZone()
        {
            _canConquer = false;
        }

        private void Conquering(Vector3 pos)
        {
            if (!_canConquer || _completed) return;

            _currentTime += Time.deltaTime;
            if (!(_currentTime >= _timeBetweenConquer)) return;
            
            _gauge += _gaugePercentageOverTime;
            if (_gauge >= 1)
            {
                _completed = true;
                PlayerV2.Instance.m_onInterpolate -= Conquering;
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
        [SerializeField] private float _timeBetweenConquer;
        
        private float _gauge;
        private bool _canConquer;
        private bool _completed;
        
        private float _currentTime;
    }
}