using System.Collections;
using System.Collections.Generic;
using PlayerRuntime;
using ResourcesManagerFeature.Runtime;
using TMPro;
using UnityEngine;

namespace UIFeature.Runtime
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        
        public GameObject PauseMenuUI
        {
            get => _pauseMenuUI;
            set => _pauseMenuUI = value;
        }
        
        public bool IsUIShowed
        {
            get => _isUIShowed;
            set => _isUIShowed = value;
        }

        public List<CanvasGroup> CanvasGroups
        {
            get => _canvasGroups;
            set => _canvasGroups = value;
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
            PlayerV2.Instance.m_onUIShow += OnUIShowEventHandler;
            ResourcesManager.Instance.m_onResourcesChange += UpdateGrowCostTextOnMouseHold;
            ResourcesManager.Instance.m_onResourcesChange += UpdateHealthText;
            StartCoroutine(WaitForInitialize());
        }

        private void OnUIShowEventHandler()
        {
            if (IsUIShowed)
            {
                foreach (var canvasGroup in CanvasGroups)
                {
                    canvasGroup.alpha = 0;
                }

                IsUIShowed = false;
            }
            else
            {
                foreach (var canvasGroup in CanvasGroups)
                {
                    canvasGroup.alpha = 1;
                }

                IsUIShowed = true;
            }
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
                    $"{((_player.CurrentClosestKnotIndex - 1 + _player.CurrentClosestRoot.InitialGrowCost) * (_player.CurrentClosestKnotIndex + _player.CurrentClosestRoot.InitialGrowCost) + _player.CurrentClosestRoot.SupplementalCostForNewRoot) / _resourcesManager.ResourcesCostDivider}";
            }
            else 
            {
                _growCost.text = $"{(((_player.CurrentClosestKnotIndex - 1 + _player.CurrentClosestRoot.InitialGrowCost) * (_player.CurrentClosestKnotIndex + _player.CurrentClosestRoot.InitialGrowCost) + _player.CurrentClosestRoot.SupplementalCostForNewRoot) / _resourcesManager.ResourcesCostDivider) - _player.CurrentClosestRoot.CostReduction}";
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
        [SerializeField] private TextMeshProUGUI _globalPurification;
        [SerializeField] private TextMeshProUGUI _growCost;
        [SerializeField] private GameObject _pauseMenuUI;

        private List<CanvasGroup> _canvasGroups = new List<CanvasGroup>();
        private PlayerV2 _player;
        private ResourcesManager _resourcesManager;
        private bool _isUIShowed = true;
    }
}
