using System.Collections;
using PlayerRuntime;
using ResourcesManagerFeature.Runtime;
using TMPro;
using UnityEngine;

namespace UiFeature.Runtime
{
    public class UiManager : MonoBehaviour
    {
        public static UiManager Instance { get; private set; }
        
        public GameObject PauseMenuUI
        {
            get => _pauseMenuUI;
            set => _pauseMenuUI = value;
        }
        
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
        
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
                ? $"{(PlayerV2.Instance.CurrentClosestSpline.Count - 1 + PlayerV2.Instance.CurrentClosestRoot.InitialGrowCost) * (PlayerV2.Instance.CurrentClosestSpline.Count + PlayerV2.Instance.CurrentClosestRoot.InitialGrowCost) / ResourcesManager.Instance.ResourcesCostDivider}" 
                : $"{(PlayerV2.Instance.CurrentClosestKnotIndex - 1 + PlayerV2.Instance.CurrentClosestRoot.InitialGrowCost) * (PlayerV2.Instance.CurrentClosestKnotIndex + PlayerV2.Instance.CurrentClosestRoot.InitialGrowCost) / ResourcesManager.Instance.ResourcesCostDivider}";
        }

        private void UpdateGrowCostTextOnMouseHold()
        {
            if (!PlayerV2.Instance.IsInterpolating) return;
            _growCost.text = 
                $"{(PlayerV2.Instance.RootToModify.Container.Spline.Count - 1 + PlayerV2.Instance.RootToModify.InitialGrowCost) * (PlayerV2.Instance.RootToModify.Container.Spline.Count + PlayerV2.Instance.RootToModify.InitialGrowCost) / ResourcesManager.Instance.ResourcesCostDivider}";
        }

        private IEnumerator WaitForInitialize()
        {
            yield return new WaitForSeconds(0.1f);
            UpdateHealthText();
        }
        
        
        [SerializeField] private TextMeshProUGUI _health;
        [SerializeField] private TextMeshProUGUI _growCost;
        [SerializeField] private GameObject _pauseMenuUI;

        private PlayerV2 _player;
    }
}
