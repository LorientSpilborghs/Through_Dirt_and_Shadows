using UnityEngine;

namespace GameManagerFeature.Runtime
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        public Transform PlayerTransform
        {
            get => _playerTransform;
            set => _playerTransform = value;
        }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private Transform _playerTransform;
    }
}
