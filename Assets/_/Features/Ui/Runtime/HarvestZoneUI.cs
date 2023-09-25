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
            _zoneHarvest = GetComponentInChildren<ZoneHarvestV2>();
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
                _canvasHealthBar.gameObject.SetActive(true);
                _uiManager.CanvasHealthBarList.Add(_canvasHealthBar);
                _text = GetComponentInChildren<TextMeshProUGUI>();
                _isEnabled = true;
            }
            m_slider.value = _zoneHarvest.CurrentResources;
            _text.text = $"{_zoneHarvest.CurrentResources} / {_zoneHarvest.BaseResources}";
        }
        
        
        [SerializeField] private GameObject _canvasHealthBar;
        
        private ZoneHarvestV2 _zoneHarvest;
        private UIManager _uiManager;
        private TextMeshProUGUI _text;
        private bool _isEnabled;
    }
}
