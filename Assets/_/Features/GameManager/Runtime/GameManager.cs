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

        public bool IsGamePause
        {
            get => _isGamePause;
            set => _isGamePause = value;
        }

        public bool IsCutScenePlaying
        {
            get => _isCutScenePlaying;
            set => _isCutScenePlaying = value;
        }

        public bool IsGameEnd
        {
            get => _isGameEnd;
            set => _isGameEnd = value;
        }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            IsGamePause = true;
            Time.timeScale = 0;
        }

        private Transform _playerTransform;
        private bool _isGamePause;
        private bool _isCutScenePlaying;
        private bool _isGameEnd;
    }
}
