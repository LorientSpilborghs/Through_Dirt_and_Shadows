using UnityEngine;

namespace PlayerRuntime
{
    public class PlayerIndicatorV2 : MonoBehaviour
    {
        private void Update()
        {
            PlayerIndicator();
        }

        private void PlayerIndicator()
        {
            PlayerV2 player = PlayerV2.Instance;
            _closestKnot = player.IsInterpolating ? player.RootToModify.Container.Spline[^1].Position : player.CurrentClosestKnot.Position;
            Vector3 mousePosition = player.PointerPosition;
            Vector3 direction = (_closestKnot - mousePosition).normalized;
            _targetIndicatorPrefab.transform.position = Vector3.Scale(_closestKnot + (-direction * _indicatorDistance), new Vector3 (1,0,1) + Vector3.up * _yAxisOffset);
            _targetIndicatorPrefab.transform.rotation = Quaternion.LookRotation(direction);
        }
        
        [SerializeField] private GameObject _targetIndicatorPrefab;
        [SerializeField] private float _indicatorDistance = 1f;
        [SerializeField] private float _yAxisOffset = 0.5f;
        
        private Vector3 _closestKnot;
        private bool _isActive = true;
    }
}
