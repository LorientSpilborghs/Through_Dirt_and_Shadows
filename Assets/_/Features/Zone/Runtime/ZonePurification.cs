using PlayerRuntime;
using UnityEngine;
using UnityEngine.Splines;

namespace ZoneFeature.Runtime
{
    public class ZonePurification : Zone
    {
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

            _currentKnotInTheZone++;

            if (_currentKnotInTheZone < _knotsNeedForPurification) return;
            
            _completed = true;
            PlayerV2.Instance.m_onNewKnotInstantiate -= Purifying;
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
        
        private int _currentKnotInTheZone;
        private bool _canPurify;
        private bool _completed;
        
    }
}