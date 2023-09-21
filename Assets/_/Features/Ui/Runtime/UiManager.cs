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
            _player = PlayerV2.Instance;
            _resourcesManager = ResourcesManager.Instance;
            PlayerV2.Instance.m_onNewKnotSelected += UpdateGrowCostTextOnMouseOver;
            ResourcesManager.Instance.m_onResourcesChange += UpdateGrowCostTextOnMouseHold;
            ResourcesManager.Instance.m_onResourcesChange += UpdateHealthText;
            StartCoroutine(WaitForInitialize());
        }

        private void UpdateHealthText()
        {
            _health.text = $"Current Health = {_resourcesManager.CurrentResources}";
        }
        
        private void UpdateGrowCostTextOnMouseOver(bool isLastKnotFromSpline)
        {
            if (isLastKnotFromSpline)
            {
                _growCost.text =
                    $"{(_player.CurrentClosestSpline.Count - 1 + _player.CurrentClosestRoot.InitialGrowCost) * (_player.CurrentClosestSpline.Count + _player.CurrentClosestRoot.InitialGrowCost) / _resourcesManager.ResourcesCostDivider}";
            }
            else if (_player.CurrentClosestKnotIndex <
                     _player.CurrentClosestRoot.MinimumNumberOfKnotsForCostReduction)
            {
                _growCost.text =
                    $"{(_player.CurrentClosestKnotIndex - 1 + _player.CurrentClosestRoot.InitialGrowCost) * (_player.CurrentClosestKnotIndex + _player.CurrentClosestRoot.InitialGrowCost) + _player.CurrentClosestRoot.SupplementalCostForNewRoot / _resourcesManager.ResourcesCostDivider}";
            }
            else 
            {
                _growCost.text = $"{((_player.CurrentClosestKnotIndex - 1 + _player.CurrentClosestRoot.InitialGrowCost) * (_player.CurrentClosestKnotIndex + _player.CurrentClosestRoot.InitialGrowCost) + _player.CurrentClosestRoot.SupplementalCostForNewRoot - _player.CurrentClosestRoot.CostReduction) / _resourcesManager.ResourcesCostDivider}";
            }
        }

        private void UpdateGrowCostTextOnMouseHold()
        {
            if (!_player.IsInterpolating) return;
            _growCost.text = 
                $"{(_player.RootToModify.Container.Spline.Count - 1 + _player.RootToModify.InitialGrowCost) * (_player.RootToModify.Container.Spline.Count + _player.RootToModify.InitialGrowCost) / _resourcesManager.ResourcesCostDivider}";
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
        private ResourcesManager _resourcesManager;
    }
}
