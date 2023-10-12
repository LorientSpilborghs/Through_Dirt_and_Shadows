using UnityEngine;

namespace PlayerRuntime
{
    public class FrontColliderBehaviour : MonoBehaviour
    {
        public bool IsBlocked
        {
            get => _isBlocked;
            private set => _isBlocked = value;
        }

        private void Start()
        {
            _angleRight = _angle;
            _angleLeft = -_angle;
            _player = PlayerV2.Instance;
        }

        private void Update()
        {
            if (_player.RootToModify is null) return;
            
            _colliderPosition = PlayerV2.Instance.RootToModify.Container.Spline[^1].Position;
            var directionRight = Quaternion.AngleAxis(_angleRight, Vector3.up) * -transform.forward;
            var directionLeft = Quaternion.AngleAxis(_angleLeft, Vector3.up) * -transform.forward;
            Ray rightRay = new Ray(_colliderPosition, directionRight);
            Ray leftRay = new Ray(_colliderPosition, directionLeft);
            RaycastHit _raycastHit = new RaycastHit();
            
            IsBlocked = Physics.Raycast(rightRay,out _raycastHit, _range, LayerMask.GetMask("Environment", "Root"))
                         || Physics.Raycast(leftRay,out _raycastHit, _range, LayerMask.GetMask("Environment", "Root"));

            if (_player.m_isInThirdPerson?.Invoke() is false) return;
            WarningUIBehaviour(_raycastHit);
        }

        private void WarningUIBehaviour(RaycastHit _raycastHit)
        {
            if (!_player.RootToModify.RootWarningUI.gameObject.activeInHierarchy 
                || !_player.RootToModify.EnvironmentWarningUI.gameObject.activeInHierarchy) return;
            
            if (_isBlocked && _raycastHit.transform.gameObject.layer == LayerMask.NameToLayer("Root"))
            {
                _player.RootToModify.RootWarningUI.alpha = 1;
            }
            else
            {
                _player.RootToModify.RootWarningUI.alpha = 0;
            }

            if (_isBlocked && _raycastHit.transform.gameObject.layer == LayerMask.NameToLayer("Environment"))
            {
                _player.RootToModify.EnvironmentWarningUI.alpha = 1;
            }
            else
            {
                _player.RootToModify.EnvironmentWarningUI.alpha = 0;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            var directionRight = Quaternion.AngleAxis(_angleRight, Vector3.up) * -transform.forward;
            var directionLeft = Quaternion.AngleAxis(_angleLeft, Vector3.up) * -transform.forward;
            Gizmos.DrawRay(_colliderPosition, directionRight * _range);
            Gizmos.DrawRay(_colliderPosition, directionLeft * _range);
        }

        [SerializeField] [Range(0.1f, 5f)] private float _range;
        [SerializeField] [Range(0, 90)] private float _angle = 15;

        private PlayerV2 _player;
        private Vector3 _colliderPosition;
        private bool _isBlocked;
        private float _angleRight;
        private float _angleLeft;
    }
}