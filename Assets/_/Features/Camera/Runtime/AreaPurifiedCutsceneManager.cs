using System.Collections;
using GameManagerFeature.Runtime;
using UnityEngine;
using UnityEngine.Playables;
using ZoneFeature.Runtime;

namespace CameraFeature.Runtime
{
    public class AreaPurifiedCutsceneManager : MonoBehaviour
    {
        private void Start()
        {
            _playableDirector = GetComponent<PlayableDirector>();
            _globalPurification = GlobalPurification.Instance;
            _gameManager = GameManager.Instance;
            _globalPurification.m_onAreaPurified += AreaPurifiedCutScene;
        }

        private void AreaPurifiedCutScene()
        {
            _playableDirector.Play();
            _gameManager.IsCutScenePlaying = true;
            StartCoroutine(WaitForCutSceneToEnd());
        }

        private IEnumerator WaitForCutSceneToEnd()
        {
            yield return new WaitForSeconds((float)_playableDirector.duration);
            _gameManager.IsCutScenePlaying = false;
        }

        private PlayableDirector _playableDirector;
        private GameManager _gameManager;
        private GlobalPurification _globalPurification;
    }
} 
