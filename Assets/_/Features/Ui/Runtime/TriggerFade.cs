using System.Collections;
using GameManagerFeature.Runtime;
using UnityEngine;
using ZoneFeature.Runtime;

namespace UIFeature.Runtime
{
    public class TriggerFade : MonoBehaviour
    {
        private void Start()
        {
            _animator = GetComponent<Animator>();
            _gameManager = GameManager.Instance;
            _zoneEndGame = ZoneEndGame.Instance;
            _gameManager.m_onGameOver += ToggleFade;
            // _zoneEndGame.m_onEnterZoneEndGame += ToggleFade;
        }

        private void ToggleFade()
        {
            _animator.SetBool("Change", true);
        }

        private ZoneEndGame _zoneEndGame;
        private GameManager _gameManager;
        private Animator _animator;
    }
}
