using System.Collections;
using PlayerRuntime;
using ResourcesManagerFeature.Runtime;
using TMPro;
using UnityEngine;

namespace UiFeature.Runtime
{
    public class UiManager : MonoBehaviour
    {
        private void Start()
        {
            PlayerV2.Instance.m_onNewKnotInstantiate += UpdateText;
            StartCoroutine(WaitForInitialize());
        }

        private void UpdateText()
        {
            _health.text = $"Current Health = {ResourcesManager.Instance.CurrentResources}";
            if (PlayerV2.Instance.RootToModify is null) return;
            _growCost.text =
                $"GrowCost = {PlayerV2.Instance.RootToModify.Container.Spline.Count * PlayerV2.Instance.ResourcesCostMultiplier}";
        }

        private IEnumerator WaitForInitialize()
        {
            yield return new WaitForSeconds(0.1f);
            UpdateText();
        }

        [SerializeField] private TextMeshProUGUI _health;
        [SerializeField] private TextMeshProUGUI _growCost;

        private PlayerV2 _player;
    }
}
