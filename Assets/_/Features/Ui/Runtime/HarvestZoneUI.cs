using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZoneFeature.Runtime;

namespace UIFeature.Runtime
{
    public class HarvestZoneUI : MonoBehaviour
    {
        public Slider m_slider;
        
        private void Start()
        {
            _zoneHarvest = GetComponent<ZoneHarvestV2>();
            _canvasGroup = GetComponentInChildren<CanvasGroup>();
            _text = GetComponentInChildren<TextMeshProUGUI>();
            _uiManager = UIManager.Instance;
            _zoneHarvest.m_onValueChange += OnValueChangeEventHandler;
            m_slider.maxValue = _zoneHarvest.BaseResources;
            m_slider.value = _zoneHarvest.BaseResources;
        }

        private void OnDestroy()
        {
            _zoneHarvest.m_onValueChange -= OnValueChangeEventHandler;
        }

        private void OnValueChangeEventHandler()
        {
            if (!_isEnabled)
            {
                _uiManager.CanvasGroups.Add(_canvasGroup);
                _isEnabled = true;
                if (_uiManager.IsUIShowed)
                {
                    _canvasGroup.alpha = 1;
                }
            }
            m_slider.value = _zoneHarvest.CurrentResources;
            _text.text = $"{_zoneHarvest.CurrentResources} / {_zoneHarvest.BaseResources}";
        }
        
        private ZoneHarvestV2 _zoneHarvest;
        private UIManager _uiManager;
        private TextMeshProUGUI _text;
        private CanvasGroup _canvasGroup;
        private bool _isEnabled;
    }
}
