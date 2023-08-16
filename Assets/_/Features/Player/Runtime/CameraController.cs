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
                _cameraPosition.x += _cameraSpeed / 50;
            }
            if (Input.GetKey(KeyCode.Q))
            {
                _cameraPosition.x -= _cameraSpeed / 50;
            }
            if (Input.GetKey(KeyCode.Z))
            {
                _cameraPosition.z += _cameraSpeed / 50;
            }
            if (Input.GetKey(KeyCode.S))
            {
                _cameraPosition.z -= _cameraSpeed / 50;
            }

            transform.position = _cameraPosition;
        }

        [SerializeField] private float _cameraSpeed;
        private Vector3 _cameraPosition;
    }
}
