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
            PlayerV2.Instance.m_isAllowedToGrow += IsCameraBlendingOver;
        }

        private void OnDestroy()
        {
            PlayerV2.Instance.m_onCameraBlendingStart -= CameraStartFollowing;
            PlayerV2.Instance.m_onInterpolate -= FollowInterpolatingKnot;
            PlayerV2.Instance.m_onCameraBlendingStop -= CameraIsNotFollowing;
            PlayerV2.Instance.m_isAllowedToGrow -= IsCameraBlendingOver;
        }
        
        private void CameraStartFollowing(Vector3 pos)
        {
            _cinemachineFreeLook.Priority = 100;
            _anchor.position = pos;
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

        private bool IsCameraBlendingOver()
        {
            return !_cinemachineBrain.IsBlending;
        }
        
        [SerializeField] private Transform _anchor;
        [SerializeField] private CinemachineBrain _cinemachineBrain;
        private CinemachineFreeLook _cinemachineFreeLook;
    }
}
