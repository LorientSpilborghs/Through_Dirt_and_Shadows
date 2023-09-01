using UnityEngine;

namespace PlayerRuntime
{
    public class PlayerIndicatorV2 : MonoBehaviour
    {
        private void Start()
        {
            Player.Instance.m_onMouseMove += PlayerIndicator;
        }
        
        private void PlayerIndicator()
        {
            Player player = Player.Instance;
            _closestKnot = player.IsInterpolating ? player.SplineToModify[^1].Position : player.CurrentClosestKnot.Position;
            Vector3 mousePosition = player.PointerPosition;
            Vector3 direction = (_closestKnot - mousePosition).normalized;
            _targetIndicatorPrefab.transform.position = _closestKnot + direction + new Vector3(0,_yAxisOffset,0) * _indicatorDistance;
            _targetIndicatorPrefab.transform.rotation = Quaternion.LookRotation(direction);
        }
        
        [SerializeField] private GameObject _targetIndicatorPrefab;
        [SerializeField] private float _indicatorDistance = 1f;
        [SerializeField] private float _yAxisOffset = 0.5f;
        
        private Vector3 _closestKnot;
    }
}
