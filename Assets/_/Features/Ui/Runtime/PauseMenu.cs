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
            PlayerV2.Instance.m_onPauseMenu += OnPauseMenuEventHandler;
        }

        private void OnDestroy()
        {
            PlayerV2.Instance.m_onPauseMenu -= OnPauseMenuEventHandler;
        }

        private void OnPauseMenuEventHandler()
        {
            if (!GameManager.Instance.IsGamePause)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }

        private void Pause()
        {
            UIManager.Instance.PauseMenuUI.SetActive(true);
            Time.timeScale = 0;
            GameManager.Instance.IsGamePause = true;
            _playerUICanvasGroup.alpha = 0;
        }

        public void Resume()
        {
            UIManager.Instance.PauseMenuUI.SetActive(false);
            Time.timeScale = 1;
            GameManager.Instance.IsGamePause = false;
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

        [SerializeField] private CanvasGroup _playerUICanvasGroup;
    }
}
