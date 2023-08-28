using UnityEngine;
using UnityEngine.Serialization;

namespace Player.Runtime
{
    public class PlayerIndicatorV2 : MonoBehaviour
    {
        public SplineRootControllerV2 m_splineControllerV2;

        private void Update()
        {
            _endRootPosition = m_splineControllerV2.GetClosestSpline(m_splineControllerV2.m_hitData.point).m_knot.Position;
            if (Input.GetKey(KeyCode.Mouse0))
            {
                _endRootPosition = m_splineControllerV2.m_previousKnotPosition;
            }
            Vector3 mousePosition = m_splineControllerV2.m_hitData.point;
            Vector3 direction = (mousePosition - _endRootPosition).normalized;
            _targetIndicatorPrefab.transform.position = _endRootPosition + direction + new Vector3(0,_yAxisOffset,0) * _indicatorDistance;
            _targetIndicatorPrefab.transform.rotation = Quaternion.LookRotation(direction);
        }
        
        [SerializeField] private GameObject _targetIndicatorPrefab;
        [SerializeField] private float _indicatorDistance = 1f;
        [SerializeField] private float _yAxisOffset = 0.5f;

        private Vector3 _endRootPosition;
    }
}
