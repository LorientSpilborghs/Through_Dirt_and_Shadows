using Cinemachine;
using GameManagerFeature.Runtime;
using PlayerRuntime;
using UnityEngine;

namespace CameraFeature.Runtime
{
    public class CameraSystem : MonoBehaviour
    {
        public bool UseEdgeScrolling
        {
            get => useEdgeScrolling;
            set => useEdgeScrolling = value;
        }

        public bool UseDragPanMove
        {
            get => useDragPanMove;
            set => useDragPanMove = value;
        }
        
        [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
        [SerializeField] private float fieldOfViewMax = 50;
        [SerializeField] private float fieldOfViewMin = 10;
        [SerializeField] private float followOffsetMin = 5f;
        [SerializeField] private float followOffsetMax = 50f;
        [SerializeField] private float followOffsetMinY = 10f;
        [SerializeField] private float followOffsetMaxY = 50f;
        
        [Space] [Header("TDS Camera Option")]
        private bool useEdgeScrolling;
        private bool useDragPanMove;
        [SerializeField] private float _cameraMoveSpeed = 1000f;
        [SerializeField] private float _edgeScrollingSpeed = 0.5f;
        [SerializeField] private float _dragPanSpeed = 0.5f;
        [SerializeField] private float _timeToReset = 1.2f;
        [SerializeField] private float _rotationSpeedInTopView = 2;
        [SerializeField] private float _rotationSpeedInThirdPerson = 4;

        private GameManager _gameManager;
        private CameraManager _cameraManager;
        private PlayerV2 _player;
        private Vector2 lastMousePosition;
        private float targetFieldOfView = 50;
        private Vector3 followOffset;
        private Vector3 _basePosition;
        private Quaternion _baseRotation;
        private Vector3 _currentPosition;
        private Quaternion _currentRotation;
        private float _resetPosDelta;
        private Rigidbody _rigidbody;

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _cameraManager = CameraManager.Instance;
            _player = PlayerV2.Instance;
            _rigidbody = GetComponent<Rigidbody>();
            followOffset.y = followOffsetMaxY;
            _basePosition = _rigidbody.position;
            _baseRotation = _rigidbody.rotation;
            _player.m_onResetCameraPos += OnResetCameraPosEventHandler;
            _player.m_onCameraRotate += OnCameraRotateEventHandler;
        }

        private void LateUpdate()
        {
            if (!_gameManager.IsTutorialOver || _gameManager.IsGamePause || _gameManager.IsGameEnd) return;
            
            if (_player.m_isInThirdPerson?.Invoke() is true)
            {
                MoveTopCameraWhileInterpolating();
                _gameManager.IsResettingPosition = false;
                if (_player.m_isCameraBlendingOver?.Invoke() is false) return;
                return;
            }

            if (!_gameManager.IsResettingPosition)
            {
                HandleCameraMovement();
            }
            
            ResetCameraPos();
        }

        private void ResetCameraPos()
        {
            if (!_gameManager.IsResettingPosition) return;

            _resetPosDelta += Time.deltaTime / _timeToReset;
            _rigidbody.position = Vector3.Lerp(_currentPosition, _basePosition, _resetPosDelta);
            _rigidbody.rotation = Quaternion.Lerp(_currentRotation, _baseRotation, _resetPosDelta);

            if (_resetPosDelta < 1) return; 
            _gameManager.IsResettingPosition = false;
        }

        private void OnResetCameraPosEventHandler()
        {
            if (_gameManager.IsResettingPosition) return;
            
            _resetPosDelta = 0;
            _currentPosition = _rigidbody.position;
            _currentRotation = _rigidbody.rotation;
            _gameManager.IsResettingPosition = true;
        }
        
        private void HandleCameraMovement()
        {
            Vector3 inputDir = new Vector3(0, 0, 0);
            int edgeScrollSize = 20;
            
            if (useEdgeScrolling)
            {
                if (Input.mousePosition.x < edgeScrollSize)
                {
                    inputDir.x -= _edgeScrollingSpeed;
                }
            
                if (Input.mousePosition.y < edgeScrollSize)
                {
                    inputDir.z -= _edgeScrollingSpeed;
                }
            
                if (Input.mousePosition.x > Screen.width - edgeScrollSize)
                {
                    inputDir.x += _edgeScrollingSpeed;
                }
            
                if (Input.mousePosition.y > Screen.height - edgeScrollSize)
                {
                    inputDir.z += _edgeScrollingSpeed;
                }
            }

            if (useDragPanMove)
            {
                if (Input.GetKeyDown(KeyCode.Mouse2))
                {
                    lastMousePosition = Input.mousePosition;
                }
                
                if (Input.GetKey(KeyCode.Mouse2))
                {
                    Vector2 mouseMovementDelta = (Vector2)Input.mousePosition - lastMousePosition;
                    inputDir.x = -mouseMovementDelta.x * _dragPanSpeed;
                    inputDir.z = -mouseMovementDelta.y * _dragPanSpeed;
                    lastMousePosition = Input.mousePosition;
                }
            }
            
            if (Input.GetKey(KeyCode.Z)) inputDir.z = +1f;
            if (Input.GetKey(KeyCode.S)) inputDir.z = -1f;
            if (Input.GetKey(KeyCode.Q)) inputDir.x = -1f;
            if (Input.GetKey(KeyCode.D)) inputDir.x = +1f;

            Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;
            _rigidbody.velocity = Time.smoothDeltaTime * _cameraMoveSpeed * moveDir;
        }

        private void OnCameraRotateEventHandler(bool isInThirdPerson)
        {
            Transform transformToRotate = new RectTransform();

            if (isInThirdPerson)
            {
                transformToRotate = _cameraManager.FollowCameraAnchor.transform;
                transformToRotate.eulerAngles += new Vector3(0,Input.GetAxis("Mouse X") * _rotationSpeedInThirdPerson, 0);
                transform.rotation = transformToRotate.rotation;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Vector3 rotation = new Vector3(0,Input.GetAxis("Mouse X") * _rotationSpeedInTopView, 0);
                Vector3 newRotation = rotation + _rigidbody.rotation.eulerAngles;
                _rigidbody.rotation = Quaternion.Euler(newRotation);
            }
        }

        private void MoveTopCameraWhileInterpolating()
        {
            var followCameraAnchorTransform = _cameraManager.FollowCameraAnchor;
            transform.position = new Vector3(followCameraAnchorTransform.position.x,transform.position.y,followCameraAnchorTransform.position.z);
            transform.rotation = followCameraAnchorTransform.rotation;
        }
    }
}