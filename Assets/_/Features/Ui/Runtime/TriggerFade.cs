using GameManagerFeature.Runtime;
using UnityEngine;

namespace UIFeature.Runtime
{
    public class TriggerFade : MonoBehaviour
    {
        private void Start()
        {
            _animator = GetComponent<Animator>();
            _gameManager = GameManager.Instance;
            _gameManager.m_onGameOver += ToggleFade;
        }

        private void ToggleFade()
        {
            _animator.SetTrigger("Change");
        }
        
        
        private GameManager _gameManager;
        private Animator _animator;
    }
}
