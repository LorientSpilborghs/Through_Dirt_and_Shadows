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
            _zonePurification = GetComponentInChildren<ZonePurification>();
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
                _canvasGaugeBar.gameObject.SetActive(true);
                _uiManager.CanvasHealthBarList.Add(_canvasGaugeBar);
                _text = GetComponentInChildren<TextMeshProUGUI>();
                _isEnabled = true;
            }
            m_slider.value = _zonePurification.CurrentKnotInTheZone;
            _text.text = $"{_zonePurification.CurrentKnotInTheZone} / {_zonePurification.KnotsNeedForPurification}";
        }
        
        
        [SerializeField] private GameObject _canvasGaugeBar;
        
        private ZonePurification _zonePurification;
        private UIManager _uiManager;
        private TextMeshProUGUI _text;
        private bool _isEnabled;
    }
}
