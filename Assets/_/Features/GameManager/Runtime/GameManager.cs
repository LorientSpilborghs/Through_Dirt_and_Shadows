using System;
using System.Collections;
using UnityEngine;

namespace GameManagerFeature.Runtime
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public Func<CanvasGroup> m_onShowTutorial;
        public Action m_onGameOver;
        public Action m_onEndGame;
        public Action m_onEndGameCinematic;
        public Action m_onShowEnd; 
        
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

        public bool IsTutorialOver
        {
            get => _isTutorialOver;
            set => _isTutorialOver = value;
        }

        public bool UseTutorial
        {
            get => _useTutorial;
            set => _useTutorial = value;
        }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private IEnumerator Start()
        {
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            yield return new WaitForEndOfFrame();
            StartCoroutine(OnShowTutorialEventHandler(m_onShowTutorial?.Invoke()));
        }

        private IEnumerator OnShowTutorialEventHandler(CanvasGroup canvasGroup)
        {
            if (!UseTutorial) yield break;
            _timer = 0;
            while (_timer < _durationBeforeShowingTutorial)
            {
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1, _timer / _durationBeforeShowingTutorial);
                _timer += Time.deltaTime;
                yield return null;
            }

            IsTutorialOver = true;
        }

        [SerializeField] private float _durationBeforeShowingTutorial = 100f;
        [SerializeField] private bool _useTutorial = true;

        private Transform _playerTransform;
        private bool _isGamePause = true;
        private bool _isTutorialOver;
        private bool _isCutScenePlaying;
        private bool _isGameEnd;
        private float _timer;
    }
}
