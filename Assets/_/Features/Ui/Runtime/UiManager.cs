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
            _player.m_onNewKnotSelected += UpdateGrowCostTextOnMouseOver;
            _player.m_onUIShow += OnUIShowEventHandler;
            _player.m_onNewKnotInstantiate += UpdateGrowCostTextOnMouseHold;
            _resourcesManager.m_onResourcesChange += UpdateHealthText;
            StartCoroutine(WaitForInitialize());
        }

        private void OnDestroy()
        {
            _player.m_onNewKnotSelected -= UpdateGrowCostTextOnMouseOver;
            _player.m_onUIShow -= OnUIShowEventHandler;
            _resourcesManager.m_onResourcesChange -= UpdateGrowCostTextOnMouseHold;
            _resourcesManager.m_onResourcesChange -= UpdateHealthText;
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
            _healthUI.UpdateHealth(_resourcesManager.CurrentResources, _resourcesManager.TotalUpcomingResources);
        }
        
        private void UpdateGrowCostTextOnMouseOver(bool isLastKnotFromSpline)
        {
            if (_player.IsInterpolating) return;
            
            if (isLastKnotFromSpline)
            {
                float value = (float)((_player.CurrentClosestSpline.Count - 1 + _player.CurrentClosestRoot.InitialGrowCost) *
                               (_player.CurrentClosestSpline.Count + _player.CurrentClosestRoot.InitialGrowCost)) /
                              _resourcesManager.ResourcesCostDivider;
                
                UpdateTextVisual(value);
            }
            else if (_player.CurrentClosestKnotIndex <
                     _player.CurrentClosestRoot.MinimumNumberOfKnotsForCostReduction)
            {
                float value = (float)((_player.CurrentClosestKnotIndex - 1 + _player.CurrentClosestRoot.InitialGrowCost) *
                                      (_player.CurrentClosestKnotIndex + _player.CurrentClosestRoot.InitialGrowCost) +
                                      _player.CurrentClosestRoot.SupplementalCostForNewRoot) / _resourcesManager.ResourcesCostDivider;
                
                UpdateTextVisual(value);
            }
            else
            {
                float value = ((float)((_player.CurrentClosestKnotIndex - 1 + _player.CurrentClosestRoot.InitialGrowCost) *
                                    (_player.CurrentClosestKnotIndex + _player.CurrentClosestRoot.InitialGrowCost) +
                                    _player.CurrentClosestRoot.SupplementalCostForNewRoot) /
                            _resourcesManager.ResourcesCostDivider) - _player.CurrentClosestRoot.CostReduction;
                
                UpdateTextVisual(value);
            }
        }

        private void UpdateGrowCostTextOnMouseHold()
        {
            if (!_player.IsInterpolating) return;

            float value = (float)(_player.RootToModify.Container.Spline.Count - 1 + _player.RootToModify.InitialGrowCost) *
                          (_player.RootToModify.Container.Spline.Count + _player.RootToModify.InitialGrowCost) /
                          _resourcesManager.ResourcesCostDivider;

            UpdateTextVisual(value);
            
            if (_scaleCoroutine is not null)
            {
                StopCoroutine(_scaleCoroutine);
            }
            _scaleCoroutine = StartCoroutine(TextScaleCoroutine());
        }

        private void UpdateTextVisual(float value)
        {
            if (value < 1)
            {
                _growCost.text = null;
            }
            else
            {
                _growCost.text = $"{(int)-value}";
                _growCost.color = _gradient.Evaluate(value / _growCostToReachLimit);
            
                if (_growCost.fontSize < (value / _growCostToReachLimit) * _maxTextFontSize)
                {
                    _growCost.fontSize = (value / _growCostToReachLimit) * _maxTextFontSize;
                }
                Debug.Log(_growCost.fontSize);
            }
        }

        private IEnumerator WaitForInitialize()
        {
            yield return new WaitForEndOfFrame();
            UpdateHealthText();
            _healthUI.SetHealth(_resourcesManager.CurrentResources, _resourcesManager.MaxResources);
        }

        private IEnumerator TextScaleCoroutine()
        {
            _scaleTimer = 0;
            while (_scaleTimer < _scaleDuration)
            {
                _growCost.transform.localScale =
                    Mathf.Lerp(_minMaxTextSize.y, _minMaxTextSize.x, EaseOutQuint(_scaleTimer / _scaleDuration)) * Vector3.one;
                _scaleTimer += Time.deltaTime;
                yield return null;
            }

            _scaleCoroutine = null;
        }
        
        private float EaseOutQuint(float number)  
        {
            return 1 - Mathf.Pow(1 - number, 5);
        }
        
        
        [SerializeField] private TextMeshProUGUI _growCost;
        [SerializeField] private GameObject _pauseMenuUI;
        [SerializeField] private HealthUI _healthUI;
        [Space] [Header("Player Indicator Settings")]
        [SerializeField] private Gradient _gradient;
        [SerializeField] private int _maxTextFontSize = 36;
        [SerializeField] private int _growCostToReachLimit = 50;
        [SerializeField] private Vector2 _minMaxTextSize = new (1,2);
        [SerializeField] private float _scaleDuration;

        private List<CanvasGroup> _canvasGroups = new List<CanvasGroup>();
        private PlayerV2 _player;
        private ResourcesManager _resourcesManager;
        private bool _isUIShowed = true;
        private float _scaleTimer;
        private Coroutine _scaleCoroutine;
    }
}
