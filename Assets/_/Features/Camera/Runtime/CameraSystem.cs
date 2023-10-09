using Cinemachine;
using GameManagerFeature.Runtime;
using PlayerRuntime;
using UnityEngine;

namespace CameraFeature.Runtime
{
    public class CameraSystem : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
        // [SerializeField] private bool useDragPan;
        [SerializeField] private float fieldOfViewMax = 50;
        [SerializeField] private float fieldOfViewMin = 10;
        [SerializeField] private float followOffsetMin = 5f;
        [SerializeField] private float followOffsetMax = 50f;
        [SerializeField] private float followOffsetMinY = 10f;
        [SerializeField] private float followOffsetMaxY = 50f;
        
        [Space] [Header("TDS Camera Option")]
        private bool useEdgeScrolling = false;
        [SerializeField] private float cameraMoveSpeed;
        [SerializeField] [Range(0,1)] private float edgeScrollingSpeed = 0.5f;
        [SerializeField] private float _timeToReset = 1.2f;
        [SerializeField] private float _rotationSpeedInTopView = 2;
        [SerializeField] private float _rotationSpeedInThirdPerson = 4;

        private GameManager _gameManager;
        private CameraManager _cameraManager;
        private PlayerV2 _player;
        private bool dragPanMoveActive;
        private Vector2 lastMousePosition;
        private float targetFieldOfView = 50;
        private Vector3 followOffset;
        private Vector3 _basePosition;
        private Quaternion _baseRotation;
        private Vector3 _currentPosition;
        private Quaternion _currentRotation;
        private bool _resetPos;
        private float _resetPosDelta;
        private Rigidbody _rigidbody;

        public bool UseEdgeScrolling
        {
            get => useEdgeScrolling;
            set => useEdgeScrolling = value;
        }

        private void Awake()
        {
            followOffset = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        }

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _cameraManager = CameraManager.Instance;
            _player = PlayerV2.Instance;
            followOffset.y = followOffsetMaxY;
            _basePosition = transform.position;
            _baseRotation = transform.rotation;
            _player.m_onResetCameraPos += OnResetCameraPosEventHandler;
            _player.m_onCameraRotate += OnCameraRotateEventHandler;
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            HandleCameraZoom_LowerY();
            if (!_gameManager.IsTutorialOver || _gameManager.IsGamePause) return;
            
            if (_player.m_isInThirdPerson?.Invoke() is true)
            {
                MoveTopCameraWhileInterpolating();
                _resetPos = false;
                if (_player.m_isCameraBlendingOver?.Invoke() is false) return;
                return;
            }
            
            HandleCameraMovement();
            
            // if (useDragPan)
            // {
            //     HandleCameraMovementDragPan();
            // }

            //HandleCameraZoom_FieldOfView();
            //HandleCameraZoom_MoveForward();

            ResetCameraPos();
        }

        private void ResetCameraPos()
        {
            if (!_resetPos) return;

            _resetPosDelta += Time.deltaTime / _timeToReset;

            transform.position = Vector3.Lerp(_currentPosition, _basePosition, _resetPosDelta);
            transform.rotation = Quaternion.Lerp(_currentRotation, _baseRotation, _resetPosDelta);

            if (_resetPosDelta >= 1) _resetPos = false;
        }

        private void OnResetCameraPosEventHandler()
        {
            _resetPosDelta = 0;
            _currentPosition = transform.position;
            _currentRotation = transform.rotation;
            _resetPos = true;
        }
        
        private void HandleCameraMovement()
        {
            Vector3 inputDir = new Vector3(0, 0, 0);
            int edgeScrollSize = 20;

            if (useEdgeScrolling)
            {
                if (Input.mousePosition.x < edgeScrollSize)
                {
                    inputDir.x -= edgeScrollingSpeed;
                }

                if (Input.mousePosition.y < edgeScrollSize)
                {
                    inputDir.z -= edgeScrollingSpeed;
                }

                if (Input.mousePosition.x > Screen.width - edgeScrollSize)
                {
                    inputDir.x += edgeScrollingSpeed;
                }

                if (Input.mousePosition.y > Screen.height - edgeScrollSize)
                {
                    inputDir.z += edgeScrollingSpeed;
                }
            }
            
            if (Input.GetKey(KeyCode.Z)) inputDir.z = +1f;
            if (Input.GetKey(KeyCode.S)) inputDir.z = -1f;
            if (Input.GetKey(KeyCode.Q)) inputDir.x = -1f;
            if (Input.GetKey(KeyCode.D)) inputDir.x = +1f;

            Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;
            _rigidbody.velocity = Time.fixedDeltaTime * cameraMoveSpeed * moveDir;
        }

        // private void HandleCameraMovementEdgeScrolling()
        // {
        //     Vector3 inputDir = new Vector3(0, 0, 0);
        //     int edgeScrollSize = 20;
        //     if (Input.mousePosition.x < edgeScrollSize)
        //     {
        //         inputDir.x -= edgeScrollingSpeed;
        //     }
        //
        //     if (Input.mousePosition.y < edgeScrollSize)
        //     {
        //         inputDir.z -= edgeScrollingSpeed;
        //     }
        //
        //     if (Input.mousePosition.x > Screen.width - edgeScrollSize)
        //     {
        //         inputDir.x += edgeScrollingSpeed;
        //     }
        //
        //     if (Input.mousePosition.y > Screen.height - edgeScrollSize)
        //     {
        //         inputDir.z += edgeScrollingSpeed;
        //     }
        //
        //     Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;
        //     _rigidbody.velocity = Time.fixedDeltaTime * cameraMoveSpeed * moveDir;
        // }

        private void HandleCameraMovementDragPan()
        {
            Vector3 inputDir = new Vector3(0, 0, 0);
            if (Input.GetMouseButtonDown(1))
            {
                dragPanMoveActive = true;
                lastMousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(1))
            {
                dragPanMoveActive = false;
            }

            if (dragPanMoveActive)
            {
                Vector2 mouseMovementDelta = (Vector2)Input.mousePosition - lastMousePosition;
                float dragPanSpeed = 1f;
                inputDir.x = mouseMovementDelta.x * dragPanSpeed;
                inputDir.z = mouseMovementDelta.y * dragPanSpeed;
                lastMousePosition = Input.mousePosition;
            }

            Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;
            float moveSpeed = 50f;
            transform.position += moveDir * (moveSpeed * Time.deltaTime);
        }

        private void OnCameraRotateEventHandler(bool isInThirdPerson)
        {
            Transform transformToRotate = new RectTransform();

            if (isInThirdPerson)
            {
                transformToRotate = _cameraManager.FollowCameraAnchor.transform;
                // float rotateDir = 0f;
                // if (Input.GetKey(KeyCode.E)) rotateDir = +1f;
                // if (Input.GetKey(KeyCode.A)) rotateDir = -1f;
                // transformToRotate.eulerAngles += new Vector3(0, rotateDir * _rotationSpeed * Time.deltaTime, 0);
                transformToRotate.eulerAngles += new Vector3(0,Input.GetAxis("Mouse X") * _rotationSpeedInThirdPerson, 0);
                transform.rotation = transformToRotate.rotation;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                transformToRotate = transform;
                // float rotateDir = 0f;
                // if (Input.GetKey(KeyCode.E)) rotateDir = +1f;
                // if (Input.GetKey(KeyCode.A)) rotateDir = -1f;
                // transformToRotate.eulerAngles += new Vector3(0, rotateDir * _rotationSpeed * Time.deltaTime, 0);
                transformToRotate.eulerAngles += new Vector3(0,Input.GetAxis("Mouse X") * _rotationSpeedInTopView, 0);
            }
        }

        private void MoveTopCameraWhileInterpolating()
        {
            var followCameraAnchorTransform = _cameraManager.FollowCameraAnchor;
            transform.position = new Vector3(followCameraAnchorTransform.position.x,transform.position.y,followCameraAnchorTransform.position.z);
            transform.rotation = followCameraAnchorTransform.rotation;
        }


        private void HandleCameraZoom_FieldOfView()
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                targetFieldOfView -= 5;
            }

            if (Input.mouseScrollDelta.y < 0)
            {
                targetFieldOfView += 5;
            }

            targetFieldOfView = Mathf.Clamp(targetFieldOfView, fieldOfViewMin, fieldOfViewMax);
            float zoomSpeed = 10f;
            cinemachineVirtualCamera.m_Lens.FieldOfView =
                Mathf.Lerp(cinemachineVirtualCamera.m_Lens.FieldOfView, targetFieldOfView, Time.deltaTime * zoomSpeed);
        }

        private void HandleCameraZoom_MoveForward()
        {
            Vector3 zoomDir = followOffset.normalized;
            float zoomAmount = 3f;
            if (Input.mouseScrollDelta.y > 0)
            {
                followOffset -= zoomDir * zoomAmount;
            }

            if (Input.mouseScrollDelta.y < 0)
            {
                followOffset += zoomDir * zoomAmount;
            }

            if (followOffset.magnitude < followOffsetMin)
            {
                followOffset = zoomDir * followOffsetMin;
            }

            if (followOffset.magnitude > followOffsetMax)
            {
                followOffset = zoomDir * followOffsetMax;
            }

            float zoomSpeed = 10f;
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
                Vector3.Lerp(cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset,
                    followOffset, Time.deltaTime * zoomSpeed);
        }

        private void HandleCameraZoom_LowerY()
        {
            float zoomAmount = 3f;
            // if (Input.mouseScrollDelta.y > 0)
            // {
            //     followOffset.y -= zoomAmount;
            // }
            //
            // if (Input.mouseScrollDelta.y < 0)
            // {
            //     followOffset.y += zoomAmount;
            // }

            followOffset.y = Mathf.Clamp(followOffset.y, followOffsetMinY, followOffsetMaxY);
            float zoomSpeed = 10f;
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
                Vector3.Lerp(cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset,
                    followOffset, Time.deltaTime * zoomSpeed);
        }
    }
}