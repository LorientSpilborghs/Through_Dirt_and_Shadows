using GameManagerFeature.Runtime;
using PlayerRuntime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UiFeature.Runtime
{
    public class PauseMenu : MonoBehaviour
    {
        public static PauseMenu Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
        
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
            UiManager.Instance.PauseMenuUI.SetActive(true);
            Time.timeScale = 0;
            GameManager.Instance.IsGamePause = true;
        }

        public void Resume()
        {
            UiManager.Instance.PauseMenuUI.SetActive(false);
            Time.timeScale = 1;
            GameManager.Instance.IsGamePause = false;
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
    }
}
