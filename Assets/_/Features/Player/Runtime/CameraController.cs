using UnityEngine;

namespace Player.Runtime
{
    public class CameraController : MonoBehaviour
    {
        void Start()
        {
            _cameraPosition = transform.position;
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.D))
            {
                _cameraPosition += transform.right * (_cameraSpeed / 50);
                //_cameraPosition.x += _cameraSpeed / 50;
            }
            if (Input.GetKey(KeyCode.Q))
            {
                _cameraPosition -= transform.right * (_cameraSpeed / 50);
                // _cameraPosition.x -= _cameraSpeed / 50;
            }
            if (Input.GetKey(KeyCode.Z))
            {
                _cameraPosition += transform.forward * (_cameraSpeed / 50);
                // _cameraPosition.z += _cameraSpeed / 50;
            }
            if (Input.GetKey(KeyCode.S))
            {
                _cameraPosition -= transform.forward * (_cameraSpeed / 50);
                // _cameraPosition.z -= _cameraSpeed / 50;
            }
            if (Input.GetKey(KeyCode.E))
            {
                transform.Rotate(0, _cameraSpeed/10, 0, Space.World); 
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.Rotate(0, -_cameraSpeed/10, 0, Space.World); 
            }

            transform.position = _cameraPosition;
        }

        [SerializeField] private float _cameraSpeed;
        private Vector3 _cameraPosition;
    }
}
