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
            PlayerV2.Instance.m_onCameraBlendingStart += CameraStartFollowing;
            PlayerV2.Instance.m_onInterpolate += FollowInterpolatingKnot;
            PlayerV2.Instance.m_onCameraBlendingStop += CameraIsNotFollowing;
            PlayerV2.Instance.m_isCameraBlendingOver += IsCameraBlendingOverEventHandler;
        }

        private void OnDestroy()
        {
            PlayerV2.Instance.m_onCameraBlendingStart -= CameraStartFollowing;
            PlayerV2.Instance.m_onInterpolate -= FollowInterpolatingKnot;
            PlayerV2.Instance.m_onCameraBlendingStop -= CameraIsNotFollowing;
            PlayerV2.Instance.m_isCameraBlendingOver -= IsCameraBlendingOverEventHandler;
        }
        
        private void CameraStartFollowing(Vector3 lastKnotPos)
        {
            _followAnchor.position = lastKnotPos;
            _cinemachineFreeLook.Priority = 100;
            CameraManager.Instance.IsInThirdPerson = true;
            CameraManager.Instance.VirtualCamera.LookAt = null;
            CameraManager.Instance.VirtualCamera.Follow = null;
        }
        
        private void FollowInterpolatingKnot(Vector3 lastKnotPos)
        {
            _followAnchor.position = lastKnotPos;
        }
        
        private void CameraIsNotFollowing()
        {
            if (CameraManager.Instance.PlayerCameraManager.IsInterpolating) return;
            _cinemachineFreeLook.Priority = 0;
            CameraManager.Instance.IsInThirdPerson = false;
            CameraManager.Instance.VirtualCamera.LookAt = CameraManager.Instance.CameraSystem.transform;
            CameraManager.Instance.VirtualCamera.Follow = CameraManager.Instance.CameraSystem.transform;
        }

        private bool IsCameraBlendingOverEventHandler()
        {
            return !_cinemachineBrain.IsBlending;
        }
        
        [SerializeField] private Transform _followAnchor;
        [SerializeField] private CinemachineBrain _cinemachineBrain;
        private CinemachineFreeLook _cinemachineFreeLook;
    }
}
