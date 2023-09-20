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
        }

        private void Update()
        {
            if (PlayerV2.Instance.RootToModify is null) return;
            
            _colliderPosition = PlayerV2.Instance.RootToModify.Container.Spline[^1].Position;
            var directionRight = Quaternion.AngleAxis(_angleRight, Vector3.up) * -transform.forward;
            var directionLeft = Quaternion.AngleAxis(_angleLeft, Vector3.up) * -transform.forward;
            Ray rightRay = new Ray(_colliderPosition, directionRight);
            Ray leftRay = new Ray(_colliderPosition, directionLeft);
            IsBlocked = Physics.Raycast(rightRay, _range, LayerMask.GetMask("Environment", "Root"))
                         || Physics.Raycast(leftRay, _range, LayerMask.GetMask("Environment", "Root"));
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
        
        private bool _isBlocked;
        private float _angleRight;
        private float _angleLeft;
        private Vector3 _colliderPosition;
    }
}