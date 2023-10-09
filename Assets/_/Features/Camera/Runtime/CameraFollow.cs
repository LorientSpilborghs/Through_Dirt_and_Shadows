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
            _cameraManager = CameraManager.Instance;
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
            _cameraManager.FollowCameraAnchor.position = lastKnotPos;
            _cameraManager.FollowCameraAnchor.transform.LookAt(PlayerV2.Instance.PointerPosition);
            _cineMachineFreeLook.Priority = 100;
            _cameraManager.VirtualCamera.LookAt = null;
            _cameraManager.VirtualCamera.Follow = null;
        }
        
        private void FollowInterpolatingKnot(Vector3 lastKnotPos)
        {
            _cameraManager.FollowCameraAnchor.position = lastKnotPos;
        }
        
        private void CameraIsNotFollowing()
        {
            if (_cameraManager.PlayerCameraManager.IsInterpolating) return;
            _cineMachineFreeLook.Priority = 0;
            _cameraManager.VirtualCamera.LookAt = _cameraManager.CameraSystemField.transform;
            _cameraManager.VirtualCamera.Follow = _cameraManager.CameraSystemField.transform;
        }

        private bool IsCameraBlendingOverEventHandler()
        {
            return !_cineMachineBrain.IsBlending;
        }
        
        [SerializeField] private CinemachineBrain _cineMachineBrain;
        private CinemachineFreeLook _cineMachineFreeLook;
        private CameraManager _cameraManager;
    }
}
