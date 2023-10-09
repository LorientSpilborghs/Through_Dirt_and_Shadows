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
            get => _cineMachineFreeLook;
            set => _cineMachineFreeLook = value;
        }

        public bool IsInThirdPerson
        {
            get => _isInThirdPerson;
            set => _isInThirdPerson = value;
        }
        

        public Transform FollowCameraAnchor
        {
            get => _followCameraAnchor;
            set => _followCameraAnchor = value;
        }

        public CameraSystem CameraSystemField
        {
            get => _cameraSystem;
            set => _cameraSystem = value;
        }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            _cameraSystem = GetComponentInChildren<CameraSystem>();
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
            PlayerV2.Instance.m_isInTopView += isInTopViewEventHandler;
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

        public void ToggleEdgeScrolling()
        {
            CameraSystemField.UseEdgeScrolling = !CameraSystemField.UseEdgeScrolling;
        }

        private IEnumerator WaitForPlayerToInitialize()
        {
            yield return new WaitForSeconds(0.1f);
            _isWaitForPlayerToInitializeOver = true;
            Start();
        }

        private bool isInThirdPersonEventHandler()
        {
            return Camera.main.transform.position == _cineMachineFreeLook.transform.position;
        }
        
        private bool isInTopViewEventHandler()
        {
            return Camera.main.transform.position == _virtualCamera.transform.position;
        }
        
        [SerializeField] private Transform _anchor;
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        [SerializeField] private CinemachineFreeLook _cineMachineFreeLook;
        [SerializeField] private Transform _followCameraAnchor;
        
        private CameraSystem _cameraSystem;
        private PlayerV2 _player;
        private bool _isWaitForPlayerToInitializeOver;
        private bool _isInThirdPerson;
    }
}
