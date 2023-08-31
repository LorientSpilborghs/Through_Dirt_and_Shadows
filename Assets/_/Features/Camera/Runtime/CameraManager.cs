using System.Collections;
using Cinemachine;
using GameManagerFeature.Runtime;
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

        public CinemachineFreeLook FreeLook
        {
            get => _cinemachineFreeLook;
            set => _cinemachineFreeLook = value;
        }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
            FreeLook = GetComponent<CinemachineFreeLook>();
        }

        private void Start()
        {
            StartCoroutine(WaitForPlayerToInitialize());
            
            if (!_isWaitForPlayerToInitializeOver) return;
            _player = GameManager.Instance.PlayerTransform.GetComponent<PlayerRuntime.Player>();
            if (_player == null)
            {
                Debug.LogError("Variable <color=cyan>PlayerTransform</color> in GameManager is null");
                return;
            }
            
            _player.m_onInterpolateStart += CameraStartFollowing;
            _player.m_onInterpolate += CameraIsFollowing;
            _player.m_onInterpolateEnd += CameraIsNotFollowing;

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
        }
        
        [SerializeField] private Transform _anchor;
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        
        private PlayerRuntime.Player _player;
        private CinemachineFreeLook _cinemachineFreeLook;
        private bool _isWaitForPlayerToInitializeOver;
    }
}
