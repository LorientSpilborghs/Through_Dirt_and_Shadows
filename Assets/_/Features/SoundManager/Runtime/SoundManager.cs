using FMODUnity;
using GameManagerFeature.Runtime;
using ResourcesManagerFeature.Runtime;
using UnityEngine;

namespace SoundManagerFeature.Runtime
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            GameManager.Instance.m_onGameOver += OnGameOverEventHandler;
            GameManager.Instance.m_onEndGameCinematic += OnGameEndEventHandler;
            GameManager.Instance.m_onShowEnd += OnShowEndEventHandler;
            ResourcesManager.Instance.m_onChangeMaxHealthTier += OnChangeMaxHealthTierEventHandler;
        }

        private void OnDestroy()
        {
            GameManager.Instance.m_onGameOver -= OnGameOverEventHandler;
            GameManager.Instance.m_onEndGameCinematic -= OnGameEndEventHandler;
            ResourcesManager.Instance.m_onChangeMaxHealthTier -= OnChangeMaxHealthTierEventHandler;
        }

        private void OnChangeMaxHealthTierEventHandler(int arg1, float arg2)
        {
            RuntimeManager.PlayOneShot(_soundOnTierChange);
        }

        private void OnGameOverEventHandler()
        {
            RuntimeManager.PlayOneShot(_soundOnGameOver);
        }

        private void OnGameEndEventHandler()
        {
            RuntimeManager.PlayOneShot(_soundOnGameEnd);
        }

        private void OnShowEndEventHandler()
        {
            RuntimeManager.PlayOneShot(_soundOnShowEnd);
        }


        [SerializeField] private EventReference _soundOnTierChange;
        [SerializeField] private EventReference _soundOnGameOver;
        [SerializeField] private EventReference _soundOnGameEnd;
        [SerializeField] private EventReference _soundOnShowEnd;
    }
}
