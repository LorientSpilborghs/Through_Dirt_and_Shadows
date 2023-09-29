using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZoneFeature.Runtime;

namespace UIFeature.Runtime
{
    public class HarvestZoneUI : MonoBehaviour
    {
        private void Start()
        {
            _zoneHarvest = GetComponent<ZoneHarvestV2>();
            _canvasGroup = GetComponentInChildren<CanvasGroup>();
            _image = GetComponentsInChildren<Image>()[1];
            _text = GetComponentInChildren<TextMeshProUGUI>();
            _uiManager = UIManager.Instance;
            _zoneHarvest.m_onValueChange += OnValueChangeEventHandler;
            _image.fillAmount = 1;
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
            _image.fillAmount = (float)_zoneHarvest.CurrentResources / _zoneHarvest.BaseResources;
            if (_text is null) return;
            _text.text = $"{_zoneHarvest.CurrentResources} / {_zoneHarvest.BaseResources}";
        }
        
        [SerializeField] private Image _image;
        
        private ZoneHarvestV2 _zoneHarvest;
        private UIManager _uiManager;
        private TextMeshProUGUI _text;
        private CanvasGroup _canvasGroup;
        private bool _isEnabled;
    }
}
