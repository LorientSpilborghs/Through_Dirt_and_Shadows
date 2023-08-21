using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player.Runtime
{
    public class CameraController : MonoBehaviour
    {
        public SplineRootController m_splineRootController;

        private void Awake()
        {
            m_splineRootController.onRootControlling += OnRootControlling;
        }

        private void Start()
        {
            _initialPosition = transform.position;
            _initialPosition = transform.eulerAngles;
            _currentZoom = _initialZoom;
        } 

        private void Update()
        {
            CameraMovement();

            CameraZoom();
        }

        private void CameraMovement()
        {
            if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width * (1 - _screenEdge))
            {
                transform.Translate(Vector3.right * _cameraTranslationSpeed, Space.Self);
            }
            else if (Input.GetKey(KeyCode.Q) || Input.mousePosition.x <= Screen.width * _screenEdge)
            {
                transform.Translate(Vector3.right * -_cameraTranslationSpeed, Space.Self);
            }
            if (Input.GetKey(KeyCode.Z) || Input.mousePosition.y >= Screen.height * (1 - _screenEdge))
            {
                transform.Translate(Vector3.forward * _cameraTranslationSpeed, Space.Self);
            }
            else if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= Screen.height * _screenEdge)
            {
                transform.Translate(Vector3.forward * -_cameraTranslationSpeed, Space.Self);
            }
            if (Input.GetKey(KeyCode.E))
            {
                transform.Rotate(0, _cameraRotationSpeed, 0, Space.World); 
            }
            else if (Input.GetKey(KeyCode.A))
            {
                transform.Rotate(0, -_cameraRotationSpeed, 0, Space.World); 
            }
        }
        
        private void CameraZoom()
        {
            _currentZoom -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 1000 * _zoomSpeed;
   
            _currentZoom = Mathf.Clamp(_currentZoom,_zoomRange.x,_zoomRange.y);
   
            transform.position = new Vector3( transform.position.x, transform.position.y - (transform.position.y - (_initialPosition.y + _currentZoom)) * 0.1f, transform.position.z );
 
            float x = transform.eulerAngles.x - (transform.eulerAngles.x - (_initialPosition.x + _currentZoom * _zoomRotation)) * 0.1f;
            x = Mathf.Clamp( x, _zoomAngleRange.x, _zoomAngleRange.y );
 
            transform.eulerAngles = new Vector3( x, transform.eulerAngles.y, transform.eulerAngles.z );

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _currentZoom = _initialZoom;
            }
        }

        private void OnRootControlling(object sender, RootInfo e)
        {
            var position = transform.position;
            position = new Vector3(e.m_rootTransform.x,position.y,e.m_rootTransform.z - _offSetWithRootWhileControlling);
            transform.position = position;
        }

        [SerializeField] private float _cameraTranslationSpeed;
        [SerializeField] private float _cameraRotationSpeed;
        [SerializeField] private float _offSetWithRootWhileControlling;
        [SerializeField] private float _screenEdge;
        [SerializeField] private Vector2 _zoomRange;
        [SerializeField] private Vector2 _zoomAngleRange;
        [SerializeField] private float _initialZoom;
        [SerializeField] private float _zoomSpeed;
        [SerializeField] private float _zoomRotation;

        private Vector3 _initialPosition;
        private Vector3 _initialRotation;
        private float _currentZoom;



    }
}
