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
            PlayerV2.Instance.m_onNewKnotSelected += UpdateGrowCostTextOnMouseOver;
            ResourcesManager.Instance.m_onResourcesChange += UpdateGrowCostTextOnMouseHold;
            ResourcesManager.Instance.m_onResourcesChange += UpdateHealthText;
            StartCoroutine(WaitForInitialize());
        }

        private void UpdateHealthText()
        {
            _health.text = $"Current Health = {ResourcesManager.Instance.CurrentResources}";
        }
        
        private void UpdateGrowCostTextOnMouseOver(bool isLastKnotFromSpline)
        {
            if (PlayerV2.Instance.IsInterpolating) return;
            _growCost.text = isLastKnotFromSpline 
                ? $"{(PlayerV2.Instance.CurrentClosestSpline.Count - 1) * PlayerV2.Instance.CurrentClosestSpline.Count / ResourcesManager.Instance.ResourcesCostDivider}" 
                : $"{2 * ResourcesManager.Instance.ResourcesCostDivider}";
        }

        private void UpdateGrowCostTextOnMouseHold()
        {
            _growCost.text = 
                $"{(PlayerV2.Instance.RootToModify.Container.Spline.Count - 1) * PlayerV2.Instance.RootToModify.Container.Spline.Count / ResourcesManager.Instance.ResourcesCostDivider}";
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
