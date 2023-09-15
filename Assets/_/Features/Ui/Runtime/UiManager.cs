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
            PlayerV2.Instance.m_onNewKnotInstantiate += UpdateHealthText;
            PlayerV2.Instance.m_onNewKnotInstantiate += UpdateGrowCostTextOnMouseDown;
            PlayerV2.Instance.m_onNewKnotSelected += UpdateGrowCostTextOnMouse;
            StartCoroutine(WaitForInitialize());
        }

        private void UpdateHealthText()
        {
            _health.text = $"Current Health = {ResourcesManager.Instance.CurrentResources}";
        }
        
        private void UpdateGrowCostTextOnMouse(bool isLastKnotFromSpline)
        {
            if (PlayerV2.Instance.IsInterpolating) return;
            _growCost.text = isLastKnotFromSpline 
                ? $"{(PlayerV2.Instance.CurrentClosestSpline.Count - 1) * PlayerV2.Instance.CurrentClosestSpline.Count / PlayerV2.Instance.ResourcesCostDivider}" 
                : $"{2 * PlayerV2.Instance.ResourcesCostDivider}";
        }

        private void UpdateGrowCostTextOnMouseDown()
        {
            _growCost.text = 
                $"{(PlayerV2.Instance.RootToModify.Container.Spline.Count - 1) * PlayerV2.Instance.RootToModify.Container.Spline.Count / PlayerV2.Instance.ResourcesCostDivider}";
        }

        private IEnumerator WaitForInitialize()
        {
            yield return new WaitForSeconds(0.1f);
            UpdateHealthText();
        }
        
        
        [SerializeField] private TextMeshProUGUI _health;
        [SerializeField] private TextMeshProUGUI _growCost;

        private PlayerV2 _player;
    }
}
