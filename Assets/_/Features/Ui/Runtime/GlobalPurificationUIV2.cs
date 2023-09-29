using UnityEngine;
using UnityEngine.UI;
using ZoneFeature.Runtime;

namespace UIFeature.Runtime
{
    public class GlobalPurificationUIV2 : MonoBehaviour
    {
        private void Start()
        {
            _globalPurification = GlobalPurification.Instance;
            _canvasGroup = GetComponentInChildren<CanvasGroup>();
            _globalPurification.m_onValueChange += OnValueChangeUpdateEventHandler;
        }

        private void OnValueChangeUpdateEventHandler(float globalPurificationPercentage)
        {
            if (!_isEnable)
            {
                _canvasGroup.alpha = 1;
                _isEnable = true;
            }
            if (!_firstGaugeDone)
            {
                if (!FillGauge(_images[0], globalPurificationPercentage)) return;
                _firstGaugeDone = true;
            }
            if (!_secondGaugeDone)
            {
                if (!FillGauge(_images[1], globalPurificationPercentage)) return;
                _secondGaugeDone = true;
            }
            
            FillGauge(_images[2], globalPurificationPercentage);
        }

        private bool FillGauge(Image image, float globalPurificationPercentage)
        {
            if (_savedAmount > 0)
            {
                image.fillAmount += _savedAmount;
                _savedAmount = 0;
                return false;
            }
            
            float newAmount = (globalPurificationPercentage / 100) * _images.Length;
            
            if (newAmount + image.fillAmount >= 1)
            {
                _savedAmount = newAmount + image.fillAmount - 1;
                image.fillAmount = 1;
                return true;
            }
            else
            {
                image.fillAmount += newAmount;
                return false;
            }
        }
        
        
        
        [SerializeField] private Image[] _images;
        
        private GlobalPurification _globalPurification;
        private CanvasGroup _canvasGroup;
        private bool _isEnable;
        private bool _firstGaugeDone;
        private bool _secondGaugeDone;
        private float _savedAmount;
    }
}
