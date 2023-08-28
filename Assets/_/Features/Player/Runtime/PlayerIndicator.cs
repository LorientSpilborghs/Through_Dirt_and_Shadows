using System;
using UnityEngine;

namespace Player.Runtime
{
    public class PlayerIndicator : MonoBehaviour
    {
        public SplineRootControllerV2 m_splineRootControllerV2;
        
        private void Update()
        {
            Vector3 rootEndPosition = m_splineRootControllerV2.m_previousKnotPosition;
            _arrow.gameObject.transform.position = rootEndPosition + new Vector3(0,_heightOffSet,0);
            Vector3 direction = Input.mousePosition - _arrow.transform.position;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        [SerializeField] private GameObject _arrow;
        [SerializeField] private float _heightOffSet;
    }
}
