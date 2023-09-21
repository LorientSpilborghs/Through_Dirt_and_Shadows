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
            _cineMachineFreeLook = GetComponent<CinemachineFreeLook>();
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
            CameraManager.Instance.FollowCameraAnchor.position = lastKnotPos;
            CameraManager.Instance.FollowCameraAnchor.transform.LookAt(PlayerV2.Instance.PointerPosition);
            _cineMachineFreeLook.Priority = 100;
            CameraManager.Instance.VirtualCamera.LookAt = null;
            CameraManager.Instance.VirtualCamera.Follow = null;
        }
        
        private void FollowInterpolatingKnot(Vector3 lastKnotPos)
        {
            CameraManager.Instance.FollowCameraAnchor.position = lastKnotPos;
        }
        
        private void CameraIsNotFollowing()
        {
            if (CameraManager.Instance.PlayerCameraManager.IsInterpolating) return;
            _cineMachineFreeLook.Priority = 0;
            CameraManager.Instance.VirtualCamera.LookAt = CameraManager.Instance.CameraSystem.transform;
            CameraManager.Instance.VirtualCamera.Follow = CameraManager.Instance.CameraSystem.transform;
        }

        private bool IsCameraBlendingOverEventHandler()
        {
            return !_cineMachineBrain.IsBlending;
        }
        
        [SerializeField] private CinemachineBrain _cineMachineBrain;
        private CinemachineFreeLook _cineMachineFreeLook;
    }
}
