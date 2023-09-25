using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZoneFeature.Runtime;

namespace UIFeature.Runtime
{
    public class PurificationUI : MonoBehaviour
    {
        public Slider m_slider;
   
        private void Start()
        {
            _zonePurification = GetComponent<ZonePurification>();
            _canvasGroup = GetComponentInChildren<CanvasGroup>();
            _text = GetComponentInChildren<TextMeshProUGUI>();
            _uiManager = UIManager.Instance;
            _zonePurification.m_onValueChange += OnValueChangeEventHandler;
            m_slider.maxValue = _zonePurification.KnotsNeedForPurification;
            m_slider.value = _zonePurification.CurrentKnotInTheZone;
        }

        private void OnDestroy()
        {
            _zonePurification.m_onValueChange -= OnValueChangeEventHandler;
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
            m_slider.value = _zonePurification.CurrentKnotInTheZone;
            _text.text = $"{_zonePurification.CurrentKnotInTheZone} / {_zonePurification.KnotsNeedForPurification}";
        }
        
        private ZonePurification _zonePurification;
        private UIManager _uiManager;
        private TextMeshProUGUI _text;
        private CanvasGroup _canvasGroup;
        private bool _isEnabled;
    }
}
