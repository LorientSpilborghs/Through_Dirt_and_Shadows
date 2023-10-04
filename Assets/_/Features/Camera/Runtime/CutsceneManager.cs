using System.Collections;
using GameManagerFeature.Runtime;
using UnityEngine;
using UnityEngine.Playables;

namespace CameraFeature.Runtime
{
    public class CutsceneManager : MonoBehaviour
    {
        private void Start()
        {
            _playableDirector = GetComponent<PlayableDirector>();
            _gameManager = GameManager.Instance;
        }

        public void StartCutScene()
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
    }
}
