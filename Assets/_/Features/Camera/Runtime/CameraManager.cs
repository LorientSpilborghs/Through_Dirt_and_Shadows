using System.Collections;
using Cinemachine;
using GameManagerFeature.Runtime;
using PlayerRuntime;
using UnityEngine;

namespace CameraFeature.Runtime
{
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance { get; private set; }

        public CinemachineVirtualCamera VirtualCamera
        {
            get => _virtualCamera;
            set => _virtualCamera = value;
        }

        public PlayerV2 PlayerCameraManager
        {
            get => _player;
            set => _player = value;
        }

        public CinemachineFreeLook FreeLook
        {
            get => _cinemachineFreeLook;
            set => _cinemachineFreeLook = value;
        }

        public bool IsInThirdPerson
        {
            get => _isInThirdPerson;
            set => _isInThirdPerson = value;
        }

        public Transform CameraSystem
        {
            get => _cameraSystem;
            set => _cameraSystem = value;
        }

        public Transform FollowCameraAnchor
        {
            get => _followCameraAnchor;
            set => _followCameraAnchor = value;
        }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            StartCoroutine(WaitForPlayerToInitialize());
            
            if (!_isWaitForPlayerToInitializeOver) return;
            PlayerCameraManager = GameManager.Instance.PlayerTransform.GetComponent<PlayerV2>();
            if (PlayerCameraManager == null)
            {
                Debug.LogError("Variable <color=cyan>PlayerTransform</color> in GameManager is null");
                return;
            }
            
            // _player.m_onInterpolateStart += CameraStartFollowing;
            // _player.m_onInterpolate += CameraIsFollowing;
            // _player.m_onInterpolateEnd += CameraIsNotFollowing;
            PlayerV2.Instance.m_isInThirdPerson += isInThirdPersonEventHandler;
        }
        
        
        
        public void CameraStartFollowing()
        {
            FreeLook.Priority = 100;
        }
        
        public void CameraIsFollowing(Vector3 pos)
        {
            _anchor.position = pos;
        }
        
        public void CameraIsNotFollowing()
        {
            FreeLook.Priority = 0;
        }

        private IEnumerator WaitForPlayerToInitialize()
        {
            yield return new WaitForSeconds(0.1f);
            _isWaitForPlayerToInitializeOver = true;
            Start();
        }

        private bool isInThirdPersonEventHandler()
        {
            return Camera.main.transform.position == _cinemachineFreeLook.transform.position;
        }
        
        [SerializeField] private Transform _anchor;
        [SerializeField] private Transform _cameraSystem;
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        [SerializeField] private CinemachineFreeLook _cinemachineFreeLook;
        [SerializeField] private Transform _followCameraAnchor;
        
        private PlayerV2 _player;
        private bool _isWaitForPlayerToInitializeOver;
        private bool _isInThirdPerson;
    }
}
