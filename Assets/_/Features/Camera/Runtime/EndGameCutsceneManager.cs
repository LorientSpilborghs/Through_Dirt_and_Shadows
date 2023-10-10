using GameManagerFeature.Runtime;
using UnityEngine;

namespace CameraFeature.Runtime
{
    public class EndGameCutsceneManager : MonoBehaviour
    {
        private void OnEnable()
        {
            _animator = GetComponent<Animator>();
            GameManager.Instance.m_onEndGameCinematic += OnEnterZoneEndGameEventHandler;
        }

        private void OnEnterZoneEndGameEventHandler()
        {
            _animator.Play("EndGameCinematic");
        }
        
        
        private Animator _animator;
    }
}
