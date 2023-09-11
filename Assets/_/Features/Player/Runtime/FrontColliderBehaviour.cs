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

        private void Update()
        {
            if (PlayerV2.Instance.RootToModify is null) return;
            _colliderPosition = PlayerV2.Instance.RootToModify.Container.Spline[^1].Position;
            
            Ray ray = new Ray(_colliderPosition,-transform.forward);
            IsBlocked = Physics.Raycast(ray, _range, LayerMask.GetMask("Environment", "Root"));
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(_colliderPosition, -transform.forward * _range);
        }

        [SerializeField] [Range(0.1f, 5f)] private float _range;
        
        private bool _isBlocked;
        private Vector3 _colliderPosition;
    }
}