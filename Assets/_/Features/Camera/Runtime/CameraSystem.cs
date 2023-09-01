using Cinemachine;
using UnityEngine;

namespace CameraFeature.Runtime
{
    public class CameraSystem : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
        [SerializeField] private bool useEdgeScrolling;
        [SerializeField] private bool useDragPan;
        [SerializeField] private float cameraMoveSpeed;
        [SerializeField] private float fieldOfViewMax = 50;
        [SerializeField] private float fieldOfViewMin = 10;
        [SerializeField] private float followOffsetMin = 5f;
        [SerializeField] private float followOffsetMax = 50f;
        [SerializeField] private float followOffsetMinY = 10f;
        [SerializeField] private float followOffsetMaxY = 50f;
        
        [Space]
        [SerializeField] private float _timeToReset = 1f;
        
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
        public AnimationCurve curve;

        private void Awake()
        {
            followOffset = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        }

        private void Start()
        {
            followOffset.y = followOffsetMaxY;
            _basePosition = transform.position;
            _baseRotation = transform.rotation;
            PlayerRuntime.Player.Instance.m_onResetCameraPos = OnResetCameraPosEventHandler;
        }

        private void Update()
        {
            if (PlayerRuntime.Player.Instance.IsInterpolating)
            {
                _resetPos = false;
                return;
            }
            HandleCameraMovement();
            if (useEdgeScrolling)
            {
                HandleCameraMovementEdgeScrolling();
            }

            if (useDragPan)
            {
                HandleCameraMovementDragPan();
            }

            HandleCameraRotation();
            //HandleCameraZoom_FieldOfView();
            //HandleCameraZoom_MoveForward();
            HandleCameraZoom_LowerY();

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
            if (Input.GetKey(KeyCode.Z)) inputDir.z = +1f;
            if (Input.GetKey(KeyCode.S)) inputDir.z = -1f;
            if (Input.GetKey(KeyCode.Q)) inputDir.x = -1f;
            if (Input.GetKey(KeyCode.D)) inputDir.x = +1f;
            Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;

            transform.position += moveDir * (cameraMoveSpeed * Time.deltaTime);
        }

        private void HandleCameraMovementEdgeScrolling()
        {
            Vector3 inputDir = new Vector3(0, 0, 0);
            int edgeScrollSize = 20;
            if (Input.mousePosition.x < edgeScrollSize)
            {
                inputDir.x = -1f;
            }

            if (Input.mousePosition.y < edgeScrollSize)
            {
                inputDir.z = -1f;
            }

            if (Input.mousePosition.x > Screen.width - edgeScrollSize)
            {
                inputDir.x = +1f;
            }

            if (Input.mousePosition.y > Screen.height - edgeScrollSize)
            {
                inputDir.z = +1f;
            }

            Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;
            float moveSpeed = 50f;
            transform.position += moveDir * (moveSpeed * Time.deltaTime);
        }

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

        private void HandleCameraRotation()
        {
            float rotateDir = 0f;
            if (Input.GetKey(KeyCode.E)) rotateDir = +1f;
            if (Input.GetKey(KeyCode.A)) rotateDir = -1f;
            float rotateSpeed = 100f;
            transform.eulerAngles += new Vector3(0, rotateDir * rotateSpeed * Time.deltaTime, 0);
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
            if (Input.mouseScrollDelta.y > 0)
            {
                followOffset.y -= zoomAmount;
            }

            if (Input.mouseScrollDelta.y < 0)
            {
                followOffset.y += zoomAmount;
            }

            followOffset.y = Mathf.Clamp(followOffset.y, followOffsetMinY, followOffsetMaxY);
            float zoomSpeed = 10f;
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
                Vector3.Lerp(cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset,
                    followOffset, Time.deltaTime * zoomSpeed);
        }
    }
}