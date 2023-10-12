using System.Collections;
using CameraFeature.Runtime;
using GameManagerFeature.Runtime;
using PlayerRuntime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UIFeature.Runtime
{
    public class PauseMenu : MonoBehaviour
    {
        private void Start()
        {
            _player = PlayerV2.Instance;
            _gameManager = GameManager.Instance;
            _uiManager = UIManager.Instance;
            _cameraManager = CameraManager.Instance;
            _player.m_onPauseMenu += OnPauseMenuEventHandler;
            _gameManager.m_onShowTutorial += GetPlayerUICanvasGroup;
            _gameManager.m_onGameOver += OnGameOverEventHandler;
            _gameManager.m_onEndGame += OnEndGameEventHandler;
            _gameManager.m_onShowEnd += OnShowEndEventHandler;
            
            if (_tutorialCanvasGroup.gameObject.activeInHierarchy) return;
            QuitTutorial();
        }

        private void OnDestroy()
        {
            _player.m_onPauseMenu -= OnPauseMenuEventHandler;
            _gameManager.m_onShowTutorial -= GetPlayerUICanvasGroup;
            _gameManager.m_onGameOver -= OnGameOverEventHandler;
        }

        private void OnPauseMenuEventHandler()
        {
            switch (_gameManager.IsGamePause)
            {
                case false when _uiManager.PauseMenuUI.activeInHierarchy is false:
                    Pause();
                    break;
                case true when _uiManager.PauseMenuUI.activeInHierarchy:
                    Resume();
                    break;
            }
        }
        
        private void OnGameOverEventHandler()
        {
            _gameManager.PlayerLost = true;
            StartCoroutine(WaitForFadeIn(_gameOverUI));
        }

        private void OnEndGameEventHandler()
        {
            _endGameUI.SetActive(true);
            _gameManager.m_onEndGameCinematic?.Invoke();
            StartCoroutine(WaitForFadeIn(null));
        }
        
        private void OnShowEndEventHandler()
        {
            _showUI.SetActive(true);
            StartCoroutine(WaitForFadeIn(null));
        }

        private IEnumerator WaitForFadeIn(GameObject gameObject)
        {
            yield return new WaitForSeconds(_timeForFade);
            _gameManager.IsGameEnd = true;
            _gameManager.IsGamePause = true;
            _gameManager.m_onStopAudio?.Invoke();
            _playerUICanvasGroup.alpha = 0;
            gameObject?.SetActive(true);

            if (!_gameManager.PlayerLost) yield break;
            
            StartCoroutine(WaitForFadeOut());
        }

        private IEnumerator WaitForFadeOut()
        {
            yield return new WaitForSeconds(_timeForFade);
            Time.timeScale = 0;
        }

        private void Pause()
        {
            _uiManager.PauseMenuUI.SetActive(true);
            _gameManager.IsGamePause = true;
            _playerUICanvasGroup.alpha = 0;
            Time.timeScale = 0;
        }

        public void Resume()
        {
            _uiManager.PauseMenuUI.SetActive(false);
            Time.timeScale = 1;
            _gameManager.IsGamePause = false;
            _playerUICanvasGroup.alpha = 1;
        }
        
        public void LoadMenu()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(0);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void QuitTutorial()
        {
            _cameraManager.ToggleEdgeScrolling();
            _cameraManager.ToggleDragPanMove();
            _gameManager.IsTutorialOver = true;
            _gameManager.IsGamePause = false;
            _playerUICanvasGroup.alpha = 1;
        }

        private CanvasGroup GetPlayerUICanvasGroup()
        {
            return _tutorialCanvasGroup;
        }

        [SerializeField] private CanvasGroup _playerUICanvasGroup;
        [SerializeField] private CanvasGroup _tutorialCanvasGroup;
        [SerializeField] private GameObject _gameOverUI;
        [SerializeField] private GameObject _endGameUI;
        [SerializeField] private GameObject _showUI;

        [Space] [Header("Option")] [SerializeField]
        private float _timeForFade = 1;

        private CameraManager _cameraManager;
        private UIManager _uiManager;
        private GameManager _gameManager;
        private PlayerV2 _player;
    }
}
