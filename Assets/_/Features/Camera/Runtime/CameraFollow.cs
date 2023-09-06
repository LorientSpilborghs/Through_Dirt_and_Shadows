using Cinemachine;
using PlayerRuntime;
using UnityEngine;

namespace CameraFeature.Runtime
{
    [RequireComponent(typeof(CinemachineFreeLook))]
    public class CameraFollow : MonoBehaviour
    {
        private void Awake()
        {
            _cinemachineFreeLook = GetComponent<CinemachineFreeLook>();
        }

        private void Start()
        {
            PlayerV2.Instance.m_onInterpolateStart += CameraStartFollowing;
            PlayerV2.Instance.m_onInterpolate += FollowInterpolatingKnot;
            PlayerV2.Instance.m_onInterpolationStop += CameraIsNotFollowing;
        }

        private void OnDestroy()
        {
            PlayerV2.Instance.m_onInterpolateStart -= CameraStartFollowing;
            PlayerV2.Instance.m_onInterpolate -= FollowInterpolatingKnot;
            PlayerV2.Instance.m_onInterpolationStop -= CameraIsNotFollowing;
        }
        
        private void CameraStartFollowing()
        {
            _cinemachineFreeLook.Priority = 100;
        }
        
        private void FollowInterpolatingKnot(Vector3 pos)
        {
            _anchor.position = pos;
        }
        
        private void CameraIsNotFollowing()
        {
            if (CameraManager.Instance.PlayerCameraManager.IsInterpolating) return;
            _cinemachineFreeLook.Priority = 0;
        }
        
        [SerializeField] private Transform _anchor;
        private CinemachineFreeLook _cinemachineFreeLook;
    }
}
