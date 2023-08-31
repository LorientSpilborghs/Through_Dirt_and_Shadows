using Cinemachine;
using UnityEngine;

namespace CameraFeature.Runtime
{
    public class CameraFollow : MonoBehaviour
    {
        private void Awake()
        {
            _cinemachineFreeLook = GetComponent<CinemachineFreeLook>();
        }

        private void Start()
        {
            PlayerRuntime.Player.Instance.m_onInterpolateStart += CameraStartFollowing;
            PlayerRuntime.Player.Instance.m_onInterpolateEnd += CameraIsNotFollowing;
        }
        
        
        public void CameraStartFollowing()
        {
            _cinemachineFreeLook.Priority = 100;
        }
        
        public void CameraIsNotFollowing()
        {
            _cinemachineFreeLook.Priority = 0;
        }
        
        [SerializeField] private Transform _anchor;
        private CinemachineFreeLook _cinemachineFreeLook;
    }
}
