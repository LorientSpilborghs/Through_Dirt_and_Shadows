using UnityEngine;
using UnityEngine.UI;
using ZoneFeature.Runtime;

namespace UIFeature.Runtime
{
    public class PurificationUI : MonoBehaviour
    {
   
        private void Start()
        {
            _zonePurification = GetComponent<ZonePurification>();
            _image = GetComponentsInChildren<Image>()[1];
            _canvasGroup = GetComponentInChildren<CanvasGroup>();
            _uiManager = UIManager.Instance;
            _zonePurification.m_onValueChange += OnValueChangeEventHandler;
            _image.fillAmount = 0;
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
            _image.fillAmount = (float)_zonePurification.CurrentKnotInTheZone / _zonePurification.KnotsNeedForPurification;
            
            if (_image.fillAmount < 1) return;
            _canvasGroup.gameObject.SetActive(false);
        }
        
        private ZonePurification _zonePurification;
        private UIManager _uiManager;
        private Image _image;
        private CanvasGroup _canvasGroup;
        private bool _isEnabled;
    }
}
